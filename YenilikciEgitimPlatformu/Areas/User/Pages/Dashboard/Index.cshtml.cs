using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YenilikciEgitimPlatformu.Models.Identity;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.Dashboard;

namespace YenilikciEgitimPlatformu.Areas.User.Pages.Dashboard;

/// <summary>
/// User Dashboard ana sayfasý PageModel
/// Kullanýcýnýn kiþisel istatistikleri ve aktiviteleri
/// </summary>
[Authorize]
public class IndexModel : PageModel
{
    #region Fields & Constructor

    private readonly IDashboardService _dashboardService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IDashboardService dashboardService,
        UserManager<ApplicationUser> userManager,
        ILogger<IndexModel> logger)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
        _logger = logger;
    }

    #endregion

    #region Properties

    public UserDashboardViewModel Data { get; set; } = new();

    #endregion

    #region Handlers

    public async Task OnGetAsync()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Dashboard: Kullanýcý bulunamadý");
                return;
            }

            // Servis üzerinden verileri çek
            Data = await _dashboardService.GetUserDashboardDataAsync(user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User dashboard yüklenirken hata oluþtu");
            // Hata durumunda boþ model döner, sayfa patlamaz.
        }

        // UI tarafýnda Substring(0,1) hatasý almamak için (ArgumentOutOfRangeException)
        // Eðer veri gelmezse veya hata olursa KullaniciAdi boþ kalabilir.
        // Bu durumda User.Identity.Name veya varsayýlan bir deðer atýyoruz.
        if (string.IsNullOrEmpty(Data.KullaniciAdi))
        {
            Data.KullaniciAdi = User.Identity?.Name ?? "Kullanýcý";
        }
    }

    #endregion
}

/*
 * SAYFA AÇIKLAMASI:
 * ==================
 * Bu sayfa oturum açmýþ kullanýcýlar için kiþisel dashboard'dur.
 * * Ýçerik:
 * - Hoþ geldin baþlýðý (Tam geniþlik)
 * - Ýstatistik kartlarý (Glassmorphism tasarým)
 * - Aylýk aktivite grafiði (Chart.js Bar Chart - Dark Mode uyumlu)
 * - Proje durum daðýlýmý (Chart.js Doughnut Chart)
 * - Son projeler listesi (Ýlerleme halkasý ile)
 * - Son aktiviteler zaman çizelgesi
 * * Yetkilendirme:
 * - Tüm oturum açmýþ kullanýcýlar eriþebilir
 * * Layout:
 * - _LayoutDashboard.cshtml kullanýlýr (User sidebar gösterilir)
 * * Tasarým:
 * - Admin paneli ile tutarlý "Glassmorphism" ve "Dot Pattern" arkaplaný kullanýldý.
 * - Negatif margin kullanýlarak Header tam geniþliðe yayýldý.
 */