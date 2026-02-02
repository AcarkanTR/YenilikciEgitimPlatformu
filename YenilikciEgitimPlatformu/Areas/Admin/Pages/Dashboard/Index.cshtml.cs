using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.Dashboard;

namespace YenilikciEgitimPlatformu.Areas.Admin.Pages.Dashboard;

/// <summary>
/// Admin Dashboard ana sayfası PageModel
/// Sistem geneli istatistikler ve grafikler
/// </summary>
[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    #region Fields & Constructor

    private readonly IDashboardService _dashboardService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IDashboardService dashboardService, ILogger<IndexModel> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Dashboard istatistik ve grafik verileri
    /// Tek bir ViewModel ile tüm veriler çekilir (Optimize edilmiş yapı)
    /// </summary>
    public AdminDashboardViewModel Data { get; set; } = new();

    #endregion

    #region HTTP Handlers

    /// <summary>
    /// Dashboard verilerini yükler
    /// Optimize edilmiş servis çağrısı ile tüm istatistikler tek seferde çekilir
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            // [Mimari Notu]
            // Bu metot eventual consistency kabul eder.
            // Dashboard verileri read-only'dir ve cache'lenebilir.
            // Kısa süreli cache tutarsızlıkları kabul edilir.
            // Cache TTL: 5 dakika (DashboardService içinde yönetilir)

            // [Performans]
            // Tek servis çağrısı ile tüm veriler çekilir (N+1 problem yok)
            // AsNoTracking() ve Projection serviste uygulanır
            Data = await _dashboardService.GetAdminDashboardDataAsync();

            // [Güvenlik Notu]
            // Admin rolü zaten [Authorize] ile kontrol ediliyor
            // Ek yetki kontrolü gerekmez
        }
        catch (Exception ex)
        {
            // [Hata Yönetimi]
            // Dashboard yükleme hatası kritik değildir
            // UI tarafında null check olduğu için boş model ile devam edebiliriz
            // Kullanıcıya boş bir dashboard gösterilir ancak sayfa patlamaz

            _logger.LogError(ex, "Dashboard yüklenirken kritik hata oluştu.");

            // [Graceful Degradation]
            // Boş model ile devam et, sayfa çalışmaya devam eder
            Data = new AdminDashboardViewModel();

            // [Kullanıcı Geri Bildirimi]
            // TempData ile hata mesajı gösterilir
            TempData["ErrorMessage"] = "Dashboard verileri yüklenirken bir hata oluştu. Lütfen sayfayı yenileyin.";
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Yüzde hesaplama helper metodu
    /// Dashboard kartlarında oran göstermek için kullanılır
    /// </summary>
    /// <param name="current">Mevcut değer</param>
    /// <param name="previous">Önceki değer</param>
    /// <returns>Yüzde değişim (-100 ile +∞ arası)</returns>
    public double CalculatePercentageChange(int current, int previous)
    {
        if (previous == 0) return current > 0 ? 100 : 0;
        return ((double)(current - previous) / previous) * 100;
    }

    #endregion
}

/*
 * ════════════════════════════════════════════════════════════════════════════
 * SAYFA AÇIKLAMASI - Dashboard Index.cshtml.cs
 * ════════════════════════════════════════════════════════════════════════════
 * 
 * [AMAÇ]
 * Admin kullanıcıları için sistem geneli istatistikleri gösterir.
 * Dashboard verileri tek bir servis çağrısı ile optimize şekilde çekilir.
 * 
 * [İÇERİK]
 * - Toplam kullanıcı, çağrı, proje sayıları
 * - Bekleyen onay sayıları (DashboardService üzerinden hesaplanır)
 * - Aylık kullanıcı kayıt grafiği (Chart.js Line Chart verisi)
 * - Proje durum dağılımı (Chart.js Doughnut Chart verisi)
 * - Son sistem aktiviteleri (AuditLog entegrasyonu ile son 5 işlem)
 * 
 * [YETKİLENDİRME]
 * - Sadece Admin rolündeki kullanıcılar erişebilir
 * - [Authorize(Roles = "Admin")] ile korunmaktadır
 * - Identity tabanlı rol kontrolü
 * 
 * [LAYOUT]
 * - _LayoutDashboard.cshtml kullanılır
 * - Modern glassmorphism tasarım dili
 * - Responsive grid layout
 * - Dark mode desteği
 * 
 * [SERVİS KULLANIMI]
 * - IDashboardService üzerinden tüm veriler tek bir ViewModel 
 *   (AdminDashboardViewModel) içinde çekilir
 * - Performans için AsNoTracking() ve Projection (Select) yöntemleri 
 *   serviste uygulanmıştır
 * - Cache kullanımı servis katmanında yönetilir (5 dakika TTL)
 * 
 * [HATA YÖNETİMİ]
 * - Try-catch bloğu ile servis hataları yakalanır ve loglanır (Serilog)
 * - Kritik hata durumunda sayfa çalışmaya devam eder (Graceful Degradation)
 * - Boş model ile UI render edilir, kullanıcı TempData ile bilgilendirilir
 * - ❌ ASLA inline Swal kullanılmaz
 * - ✅ TempData["ErrorMessage"] -> Layout'ta AlertService.error() tetiklenir
 * 
 * [PERFORMANS]
 * - Tek servis çağrısı ile tüm veriler alınır (N+1 problem yok)
 * - AsNoTracking() ile read-only sorgular
 * - Projection ile sadece gerekli alanlar çekilir
 * - Cache TTL: 5 dakika (servis katmanında)
 * - Lazy loading görseller için (frontend'de)
 * 
 * [TRANSACTION STRATEJİSİ]
 * - Eventual Consistency modeli
 * - Dashboard verileri read-only ve cache'lenebilir
 * - Kısa süreli tutarsızlıklar kabul edilir
 * - Cache invalidation gerçek zamanlı değildir
 * - SignalR ile anlık güncellemeler yapılabilir (V2 kapsamında)
 * 
 * [GRAFİKLER VE VERİ GÖRSELLEŞTİRME]
 * - Chart.js 4.4.1 kullanılır
 * - Aylık kullanıcı kayıt grafiği (Line Chart)
 * - Proje durum dağılımı (Doughnut Chart)
 * - Responsive chart yapıları
 * - Dark mode uyumlu renkler
 * 
 * [BAĞIMLILIKLAR]
 * - IDashboardService (İstatistik ve özet bilgiler)
 * - AdminDashboardViewModel (ViewModel)
 * - Chart.js (Grafik gösterimi)
 * - _LayoutDashboard.cshtml (Layout)
 * - AlertService.js (Kullanıcı geri bildirimi)
 * 
 * [VERİ AKIŞI]
 * 1. OnGetAsync() -> DashboardService çağrısı
 * 2. Servis -> Database sorguları (AsNoTracking + Projection)
 * 3. Servis -> Cache kontrolü (5 dakika TTL)
 * 4. ViewModel -> PageModel.Data property
 * 5. Razor View -> Chart.js ile görselleştirme
 * 
 * [REGION YAPISI]
 * - Fields & Constructor: Dependency injection
 * - Properties: Public ve bindable özellikler
 * - HTTP Handlers: OnGetAsync, OnPostAsync vb.
 * - Helper Methods: Hesaplama ve yardımcı fonksiyonlar
 * 
 * [TÜRKÇE AÇIKLAMALAR]
 * - Tüm kod blokları Türkçe açıklanmıştır
 * - Region başlıkları Türkçe/İngilizce karışık (Okunabilirlik için)
 * - Dosya sonu genel açıklama bloğu Türkçe
 */