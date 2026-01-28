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
    #region Fields & Constructor

    private readonly IDashboardService _dashboardService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IDashboardService dashboardService,
        ILogger<IndexModel> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    #endregion

    #region Properties

    public AdminDashboardViewModel Data { get; set; } = new();

    #endregion

    #region Handlers

    public async Task OnGetAsync()
    {
        try
        {
            Data = await _dashboardService.GetAdminDashboardDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Admin dashboard yüklenirken hata oluþtu");
            // Hata durumunda boþ model döner, UI'da "Veri yüklenemedi" mesajý gösterilebilir
        }
    }

    #endregion
}

/*
 * SAYFA AÇIKLAMASI:
 * ==================
 * Bu sayfa Admin kullanýcýlarý için sistem geneli istatistikleri gösterir.
 * 
 * Ýçerik:
 * - Toplam kullanýcý, çaðrý, proje sayýlarý
 * - Bekleyen onay sayýlarý
 * - Aylýk kullanýcý kayýt grafiði (Chart.js Line Chart)
 * - Proje durum daðýlýmý (Chart.js Pie Chart)
 * - En aktif kullanýcýlar tablosu
 * 
 * Yetkilendirme:
 * - Sadece Admin rolündeki kullanýcýlar eriþebilir
 * 
 * Layout:
 * - _LayoutDashboard.cshtml kullanýlýr (Conditional rendering ile Admin sidebar gösterilir)
 * 
 * Servis Kullanýmý:
 * - IDashboardService üzerinden tüm veriler çekilir
 * - Eventual consistency kabul edilir
 * 
 * Chart.js Entegrasyonu:
 * - Layout'ta Chart.js CDN yüklüdür
 * - Scripts section'da chart'lar initialize edilir
 * - Veriler Razor'dan JSON.serialize ile aktarýlýr
 */