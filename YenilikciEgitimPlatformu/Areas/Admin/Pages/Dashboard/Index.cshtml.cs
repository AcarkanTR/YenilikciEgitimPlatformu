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
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IDashboardService dashboardService, ILogger<IndexModel> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    public AdminDashboardViewModel Data { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            // Optimize edilmiş servis çağrısı ile verileri çek
            Data = await _dashboardService.GetAdminDashboardDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dashboard yüklenirken kritik hata oluştu.");
            // UI tarafında null check olduğu için boş model ile devam edebiliriz, 
            // kullanıcıya boş bir dashboard gösterilir ancak sayfa patlamaz.
        }
    }
}

/*
 * SAYFA AÇIKLAMASI:
 * ==================
 * Bu sayfa Admin kullanıcıları için sistem geneli istatistikleri gösterir.
 * * İçerik:
 * - Toplam kullanıcı, çağrı, proje sayıları
 * - Bekleyen onay sayıları (DashboardService üzerinden hesaplanır)
 * - Aylık kullanıcı kayıt grafiği (Chart.js Line Chart verisi)
 * - Proje durum dağılımı (Chart.js Doughnut Chart verisi)
 * - Son sistem aktiviteleri (AuditLog entegrasyonu ile son 5 işlem)
 * * Yetkilendirme:
 * - Sadece Admin rolündeki kullanıcılar erişebilir [Authorize(Roles = "Admin")]
 * * Layout:
 * - _LayoutDashboard.cshtml kullanılır.
 * * Servis Kullanımı:
 * - IDashboardService üzerinden tüm veriler tek bir ViewModel (AdminDashboardViewModel) içinde çekilir.
 * - Performans için AsNoTracking() ve Projection (Select) yöntemleri serviste uygulanmıştır.
 * * Hata Yönetimi:
 * - Try-catch bloğu ile servis hataları yakalanır ve loglanır (Serilog).
 * - Kritik hata durumunda sayfa çalışmaya devam eder (Graceful Degradation).
 */