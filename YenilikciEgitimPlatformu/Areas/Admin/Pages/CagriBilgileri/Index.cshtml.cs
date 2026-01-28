using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.CagriBilgisi;

namespace YenilikciEgitimPlatformu.Areas.Admin.Pages.CagriBilgileri;

/// <summary>
/// Admin Çaðrý Bilgileri liste sayfasý
/// CRUD iþlemleri için ana liste sayfasý
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
    }

    #endregion

    #region Properties

    [BindProperty(SupportsGet = true)]
    public CagriFiltreleViewModel Filtre { get; set; } = new();

    public List<CagriListViewModel> Data { get; set; } = new();
    public int TotalCount { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    #endregion

    #region Handlers

    public async Task OnGetAsync()
    {
        try
        {
            // Admin için pasif çaðrýlar da gösterilsin
            Filtre.SadeceAktif = false;

            var result = await _cagriService.GetAllAsync(Filtre);
            Data = result.Data;
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çaðrý listesi yüklenirken hata");
            ErrorMessage = "Çaðrý listesi yüklenirken bir hata oluþtu";
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            var userId = User.Identity?.Name ?? "system";
            var result = await _cagriService.DeleteAsync(id, userId);

            if (result.Success)
                SuccessMessage = result.Message;
            else
                ErrorMessage = result.Message;

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çaðrý silinirken hata: {Id}", id);
            ErrorMessage = "Çaðrý silinirken bir hata oluþtu";
            return RedirectToPage();
        }
    }

    #endregion
}

/*
 * SAYFA AÇIKLAMASI:
 * ==================
 * Admin kullanýcýlarý için Çaðrý Bilgileri yönetim sayfasý.
 * 
 * Özellikler:
 * - Tüm çaðrýlarý listeleme (aktif + pasif)
 * - Filtreleme (arama, tür, kurum)
 * - Sayfalama
 * - CRUD iþlemleri (Görüntüle, Düzenle, Sil)
 * - SweetAlert2 ile silme onayý
 * 
 * Yetkilendirme:
 * - Sadece Admin rolü eriþebilir
 * 
 * UI/UX:
 * - Modern card layout
 * - Responsive tasarým
 * - Floating labels
 * - Hover efektleri
 * - Ýþlem butonlarý (Görüntüle: Mavi, Düzenle: Sarý, Sil: Kýrmýzý)
 */