using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.CagriBilgisi;

namespace YenilikciEgitimPlatformu.Areas.Admin.Pages.CagriBilgileri;

/// <summary>
/// Admin - Çaðrý Detay Görünümü
/// 
/// Özellikler:
/// - Read-only görünüm
/// - Tüm bilgileri gösterme
/// - Ýstatistikler
/// </summary>
[Authorize(Roles = "Admin")]
public class DetayModel : PageModel
{
    #region Fields & Constructor

    private readonly ICagriBilgisiService _cagriService;
    private readonly ILogger<DetayModel> _logger;

    public DetayModel(
        ICagriBilgisiService cagriService,
        ILogger<DetayModel> logger)
    {
        _cagriService = cagriService;
        _logger = logger;
    }

    #endregion

    #region Properties

    public CagriDetayViewModel Cagri { get; set; } = new();

    #endregion

    #region Handlers

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var cagri = await _cagriService.GetByIdAsync(id);

        if (cagri == null)
        {
            return NotFound();
        }

        Cagri = cagri;

        // Görüntülenme sayýsýný artýr (admin görüntülemesi sayýlmasýn isterseniz kaldýrýn)
        // await _cagriService.IncrementViewCountAsync(id);

        return Page();
    }

    #endregion
}

/*
 * PAGEMODEL AÇIKLAMASI:
 * ======================
 * Admin Çaðrý Detay Backend
 * 
 * Basit ve temiz:
 * - Sadece veri yükleme
 * - NotFound kontrolü
 * - Read-only
 * 
 * Not:
 * Admin görüntülemesi görüntülenme sayýsýna
 * eklenmez (yorum satýrýnda býrakýldý)
 */