using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.Dashboard;

namespace YenilikciEgitimPlatformu.Areas.Admin.Pages.Dashboard;

/// <summary>
/// Admin Dashboard ana sayfasý PageModel
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
            // Optimize edilmiþ servis çaðrýsý ile verileri çek
            Data = await _dashboardService.GetAdminDashboardDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dashboard yüklenirken kritik hata oluþtu.");
            // UI tarafýnda null check olduðu için boþ model ile devam edebiliriz, 
            // kullanýcýya boþ bir dashboard gösterilir ancak sayfa patlamaz.
        }
    }
}

/*
 * SAYFA AÇIKLAMASI:
 * ==================
 * Bu sayfa Admin kullanýcýlarý için sistem geneli istatistikleri gösterir.
 * * Ýçerik:
 * - Toplam kullanýcý, çaðrý, proje sayýlarý
 * - Bekleyen onay sayýlarý (DashboardService üzerinden hesaplanýr)
 * - Aylýk kullanýcý kayýt grafiði (Chart.js Line Chart verisi)
 * - Proje durum daðýlýmý (Chart.js Doughnut Chart verisi)
 * - Son sistem aktiviteleri (AuditLog entegrasyonu ile son 5 iþlem)
 * * Yetkilendirme:
 * - Sadece Admin rolündeki kullanýcýlar eriþebilir [Authorize(Roles = "Admin")]
 * * Layout:
 * - _LayoutDashboard.cshtml kullanýlýr.
 * * Servis Kullanýmý:
 * - IDashboardService üzerinden tüm veriler tek bir ViewModel (AdminDashboardViewModel) içinde çekilir.
 * - Performans için AsNoTracking() ve Projection (Select) yöntemleri serviste uygulanmýþtýr.
 * * Hata Yönetimi:
 * - Try-catch bloðu ile servis hatalarý yakalanýr ve loglanýr (Serilog).
 * - Kritik hata durumunda sayfa çalýþmaya devam eder (Graceful Degradation).
 */