using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.CagriBilgisi;

namespace YenilikciEgitimPlatformu.Areas.Admin.Pages.CagriBilgileri;

/// <summary>
/// Admin Çağrı Bilgileri liste sayfası
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
    /// </summary>
    [TempData]
    public string? SuccessMessage { get; set; }

    /// <summary>
    /// Hata mesajı
    /// </summary>
    [TempData]
    public string? ErrorMessage { get; set; }

    #endregion

    #region HTTP Handlers

    /// <summary>
    /// Sayfa yükleme - Filtreleme ve listeleme
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            // [Konfigürasyon]
            // Admin için pasif çağrılar da gösterilir
            Filtre.SadeceAktif = false;

            // [Servis Çağrısı]
            // Filtreleme, sayfalama ve sıralama serviste uygulanır
            var result = await _cagriService.GetAllAsync(Filtre);

            Data = result.Data;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            // [Hata Yönetimi]
            // Listeleme hatası kritik değildir, boş liste gösterilir
            _logger.LogError(ex, "Çağrı listesi yüklenirken hata");
            ErrorMessage = "Çağrı listesi yüklenirken bir hata oluştu. Lütfen tekrar deneyin.";
        }
    }

    /// <summary>
    /// Silme işlemi (Soft Delete)
    /// </summary>
    /// <param name="id">Silinecek çağrı ID</param>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            // [Mimari Notu]
            // Bu metot eventual consistency kabul eder.
            // DB atomic, cache ve SignalR side-effect olarak ele alınır.

            // [Audit Trail]
            // Kullanıcı bilgisi audit için servise gönderilir
            var userId = User.Identity?.Name ?? "system";

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
    public int GetTotalPages()
    {
        return (int)Math.Ceiling((double)TotalCount / Filtre.PageSize);
    }

    /// <summary>
    /// Mevcut sayfanın son kayıt numarasını döner
    /// </summary>
    public int GetEndRecord()
    {
        var end = Filtre.Page * Filtre.PageSize;
        return end > TotalCount ? TotalCount : end;
    }

    /// <summary>
    /// Mevcut sayfanın ilk kayıt numarasını döner
    /// </summary>
    public int GetStartRecord()
    {
        return TotalCount == 0 ? 0 : ((Filtre.Page - 1) * Filtre.PageSize) + 1;
    }

    #endregion
}

/*
 * ════════════════════════════════════════════════════════════════════════════
 * SAYFA AÇIKLAMASI
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
 * 
 * [GÜVENLİK]
 * - CSRF token koruması
 * - XSS koruması (Razor encoding)
 * - SQL Injection koruması (EF Core)
 * - Audit logging (Kim, Ne Zaman, Ne Yaptı)
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
 * 
 * [ALERTSERVICE ENTEGRASYONU]
 * - ❌ ASLA alert(), confirm() kullanılmaz
 * - ✅ HER ZAMAN AlertService.confirmDelete() kullanılır
 * - TempData ile server-side mesajlar otomatik gösterilir
 * - Toast: Başarılı işlemler (3 saniye)
 * - Modal: Hatalar (Manuel kapatma)
 * 
 * [BAĞIMLILIKLAR]
 * - ICagriBilgisiService (İş mantığı)
 * - CagriFiltreleViewModel (Filtre modeli)
 * - CagriListViewModel (Liste görünümü)
 * - AlertService.js (Kullanıcı geri bildirimi)
 */