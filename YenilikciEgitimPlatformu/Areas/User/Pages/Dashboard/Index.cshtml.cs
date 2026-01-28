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

            Data = await _dashboardService.GetUserDashboardDataAsync(user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User dashboard yüklenirken hata oluþtu");
            // Hata durumunda boþ model döner
        }
    }

    #endregion
}

/*
 * SAYFA AÇIKLAMASI:
 * ==================
 * Bu sayfa oturum açmýþ kullanýcýlar için kiþisel dashboard'dur.
 * 
 * Ýçerik:
 * - Hoþ geldin kartý (Profil fotoðrafý ile)
 * - Ýstatistik kartlarý (Proje, Gönderi, Rozet, XP)
 * - Aylýk aktivite grafiði (Chart.js Bar Chart)
 * - Proje durum daðýlýmý (Chart.js Doughnut Chart)
 * - Son projeler listesi (Ýlerleme çubuðu ile)
 * - Son aktiviteler zaman çizelgesi
 * 
 * Yetkilendirme:
 * - Tüm oturum açmýþ kullanýcýlar eriþebilir
 * 
 * Layout:
 * - _LayoutDashboard.cshtml kullanýlýr (User sidebar gösterilir)
 * 
 * Servis Kullanýmý:
 * - IDashboardService üzerinden kullanýcýya özel veriler çekilir
 * - UserManager ile mevcut kullanýcý bilgisi alýnýr
 * 
 * Özel Özellikler:
 * - Eðer kullanýcýnýn projesi yoksa "Ýlk Projeyi Oluþtur" CTA gösterilir
 * - Aktivite boþsa "Henüz aktivite yok" mesajý gösterilir
 * - Responsive tasarým (Mobile-first)
 */