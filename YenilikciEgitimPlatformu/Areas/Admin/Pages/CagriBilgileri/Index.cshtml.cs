using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.CagriBilgisi;

namespace YenilikciEgitimPlatformu.Areas.Admin.Pages.CagriBilgileri;

/// <summary>
/// Admin Çağrı Bilgileri liste sayfası PageModel
/// CRUD işlemleri için ana yönetim sayfası
/// </summary>
[Authorize(Roles = "Admin")]

public class IndexModel : PageModel
{
    #region Fields & Constructor

    private readonly ICagriBilgisiService _cagriService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        ICagriBilgisiService cagriService,
        ILogger<IndexModel> logger)
    {
        _cagriService = cagriService;
        _logger = logger;

        // [Konfigürasyon]
        // Türkçe tarih formatı için global ayar
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("tr-TR");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("tr-TR");
    }

    #endregion

    #region Properties

    /// <summary>
    /// Filtreleme ve sayfalama parametreleri
    /// Query string'den otomatik bind edilir
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public CagriFiltreleViewModel Filtre { get; set; } = new();

    /// <summary>
    /// Filtrelenmiş ve sayfalanmış çağrı listesi
    /// </summary>
    public List<CagriListViewModel> Data { get; set; } = new();

    /// <summary>
    /// Toplam kayıt sayısı (sayfalama için)
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Başarı mesajı (silme işlemi sonrası)
    /// TempData ile Layout'ta AlertService.toastSuccess() tetiklenir
    /// </summary>
    [TempData]
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Hata mesajı
    /// TempData ile Layout'ta AlertService.error() tetiklenir
    /// </summary>
    [TempData]
    public string? ErrorMessage { get; set; }

    #endregion

    #region HTTP Handlers

    /// <summary>
    /// Sayfa yükleme - Filtreleme ve listeleme
    /// GET: /Admin/CagriBilgileri
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            // [Mimari Notu]
            // Bu metot eventual consistency kabul eder.
            // Listeleme read-only işlemdir ve cache'lenebilir.
            // Kısa süreli cache tutarsızlıkları kabul edilir.

            // [Konfigürasyon]
            // Admin için pasif çağrılar da gösterilir
            Filtre.SadeceAktif = false;

            // [Servis Çağrısı]
            // Filtreleme, sayfalama ve sıralama serviste uygulanır
            // AsNoTracking() ve Projection ile optimize edilmiştir
            var result = await _cagriService.GetAllAsync(Filtre);

            Data = result.Data;
            TotalCount = result.TotalCount;

            // [Performans Logging]
            _logger.LogInformation(
                "Çağrı listesi yüklendi. Toplam: {TotalCount}, Sayfa: {Page}, PageSize: {PageSize}",
                TotalCount, Filtre.Page, Filtre.PageSize
            );
        }
        catch (Exception ex)
        {
            // [Hata Yönetimi]
            // Listeleme hatası kritik değildir, boş liste gösterilir
            _logger.LogError(ex, "Çağrı listesi yüklenirken hata");
            ErrorMessage = "Çağrı listesi yüklenirken bir hata oluştu. Lütfen tekrar deneyin.";

            // [Graceful Degradation]
            // Boş liste ile devam et
            Data = new List<CagriListViewModel>();
            TotalCount = 0;
        }
    }

    /// <summary>
    /// Silme işlemi (Soft Delete)
    /// POST: /Admin/CagriBilgileri?handler=Delete&id=123
    /// </summary>
    /// <param name="id">Silinecek çağrı ID</param>
    /// <returns>Redirect to Index</returns>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            // [Mimari Notu]
            // Bu metot eventual consistency kabul eder.
            // DB atomic, cache ve SignalR side-effect olarak ele alınır.

            // [Güvenlik]
            // Admin rolü zaten [Authorize] ile kontrol ediliyor
            // Ek yetki kontrolü gerekmez

            // [Audit Trail]
            // Kullanıcı bilgisi audit için servise gönderilir
            var userId = User.Identity?.Name ?? "system";

            // [Validasyon]
            if (id <= 0)
            {
                ErrorMessage = "Geçersiz çağrı ID.";
                _logger.LogWarning("Geçersiz çağrı ID ile silme denemesi: {Id}", id);
                return RedirectToPage();
            }

            // [Silme İşlemi]
            // Soft Delete: SilindiMi = true, SilinmeTarihi = DateTime.UtcNow
            var result = await _cagriService.DeleteAsync(id, userId);

            // [Cache Invalidation]
            // Servis içinde cache temizlenir (eventual consistency)
            // Kullanıcı bir sonraki yüklemede güncel veriyi görür

            if (result.Success)
            {
                SuccessMessage = result.Message;
                _logger.LogInformation("Çağrı başarıyla silindi. ID: {Id}, Kullanıcı: {UserId}", id, userId);
            }
            else
            {
                ErrorMessage = result.Message;
                _logger.LogWarning("Çağrı silinemedi. ID: {Id}, Sebep: {Message}", id, result.Message);
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            // [Kritik Hata]
            // Silme işlemi başarısız, kullanıcı bilgilendirilir
            _logger.LogError(ex, "Çağrı silinirken beklenmeyen hata: {Id}", id);
            ErrorMessage = "Çağrı silinirken bir hata oluştu. Lütfen sistem yöneticisine başvurun.";
            return RedirectToPage();
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Sayfa sayısını hesaplar (Pagination için)
    /// </summary>
    /// <returns>Toplam sayfa sayısı</returns>
    public int GetTotalPages()
    {
        return (int)Math.Ceiling((double)TotalCount / Filtre.PageSize);
    }

    /// <summary>
    /// Mevcut sayfanın son kayıt numarasını döner
    /// </summary>
    /// <returns>Son kayıt numarası</returns>
    public int GetEndRecord()
    {
        var end = Filtre.Page * Filtre.PageSize;
        return end > TotalCount ? TotalCount : end;
    }

    /// <summary>
    /// Mevcut sayfanın ilk kayıt numarasını döner
    /// </summary>
    /// <returns>İlk kayıt numarası</returns>
    public int GetStartRecord()
    {
        return TotalCount == 0 ? 0 : ((Filtre.Page - 1) * Filtre.PageSize) + 1;
    }

    #endregion
}

/*
 * ════════════════════════════════════════════════════════════════════════════
 * SAYFA AÇIKLAMASI - CagriBilgileri Index.cshtml.cs
 * ════════════════════════════════════════════════════════════════════════════
 * 
 * [AMAÇ]
 * Admin kullanıcıları için Çağrı Bilgileri yönetim sayfası.
 * Tüm çağrıları listeleme, filtreleme, görüntüleme ve silme işlemlerini sağlar.
 * 
 * [ÖZELLİKLER]
 * - Tüm çağrıları listeleme (aktif + pasif)
 * - Gelişmiş filtreleme (arama, tür, kurum)
 * - Sayfalama (default: 12 kayıt/sayfa)
 * - Sıralama (oluşturma tarihi, güncelleme tarihi)
 * - CRUD işlemleri (Görüntüle, Düzenle, Sil)
 * - Soft Delete (Geri alınabilir silme)
 * - Merkezi AlertService ile kullanıcı geri bildirimi
 * 
 * [YETKİLENDİRME]
 * - Sadece Admin rolü erişebilir
 * - [Authorize(Roles = "Admin")] ile korunmaktadır
 * - Identity tabanlı audit trail
 * 
 * [UI/UX]
 * - Modern card layout (Grid sistem)
 * - Responsive tasarım (Mobile-first)
 * - Floating labels (Material Design)
 * - Glassmorphism tasarım dili
 * - Hover efektleri ve smooth transitions
 * - İşlem butonları renk kodlu:
 *   * Görüntüle: Mavi (Petrol)
 *   * Düzenle: Sarı (Amber)
 *   * Sil: Kırmızı (Red)
 * 
 * [PERFORMANS]
 * - AsNoTracking() ile read-only sorgular
 * - Projection ile sadece gerekli alanlar
 * - Sayfalama ile veri sınırlaması
 * - Cache kullanımı (5 dakika TTL)
 * - Lazy loading görseller için
 * 
 * [GÜVENLİK]
 * - CSRF token koruması
 * - XSS koruması (Razor encoding)
 * - SQL Injection koruması (EF Core)
 * - Audit logging (Kim, Ne Zaman, Ne Yaptı)
 * - Input validation (Model binding)
 * 
 * [VERİ AKIŞI]
 * 1. OnGetAsync() -> Filtre uygula -> Servisten veri çek -> View'e aktar
 * 2. OnPostDeleteAsync() -> Kullanıcı doğrula -> Soft delete -> Cache invalidate -> Redirect
 * 
 * [TRANSACTION STRATEJİSİ]
 * - Eventual Consistency modeli
 * - DB işlemi atomic (EF Core)
 * - Cache ve SignalR side-effect
 * - Kısa süreli tutarsızlık kabul edilir
 * - Cache invalidation gerçek zamanlı değildir
 * 
 * [ALERTSERVICE ENTEGRASYONU]
 * - ❌ ASLA alert(), confirm() kullanılmaz
 * - ✅ HER ZAMAN AlertService.confirmDelete() kullanılır
 * - TempData ile server-side mesajlar otomatik gösterilir:
 *   * TempData["SuccessMessage"] -> AlertService.toastSuccess() (3 saniye)
 *   * TempData["ErrorMessage"] -> AlertService.error() (Modal)
 * - Frontend'de JavaScript ile AlertService import edilir (ES Module)
 * 
 * [FİLTRELEME VE SAYFALAMA]
 * - Query string ile filtre değerleri korunur
 * - [BindProperty(SupportsGet = true)] ile otomatik model binding
 * - Filtre değişikliğinde sayfa 1'e döner
 * - Pagination linkleri filtre parametrelerini korur
 * - Debounced auto-submit (600ms) ile kullanıcı deneyimi optimize edilir
 * 
 * [BAĞIMLILIKLAR]
 * - ICagriBilgisiService (İş mantığı)
 * - CagriFiltreleViewModel (Filtre modeli)
 * - CagriListViewModel (Liste görünümü)
 * - AlertService.js (Kullanıcı geri bildirimi)
 * - _LayoutDashboard.cshtml (Layout)
 * 
 * [LOGGING]
 * - ILogger ile tüm işlemler loglanır
 * - Serilog ile structured logging
 * - Error, Warning, Information seviyeleri
 * - Exception stack trace ile detaylı hata kayıtları
 * 
 * [REGION YAPISI]
 * - Fields & Constructor: Dependency injection ve field tanımları
 * - Properties: Public ve bindable özellikler
 * - HTTP Handlers: OnGetAsync, OnPostDeleteAsync vb.
 * - Helper Methods: Hesaplama ve yardımcı fonksiyonlar
 */