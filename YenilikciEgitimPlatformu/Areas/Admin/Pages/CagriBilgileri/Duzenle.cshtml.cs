using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YenilikciEgitimPlatformu.Data;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.CagriBilgisi;
using System.Security.Claims;

namespace YenilikciEgitimPlatformu.Areas.Admin.Pages.CagriBilgileri;

/// <summary>
/// Admin - Çaðrý Düzenleme Sayfasý
/// 
/// Özellikler:
/// - Mevcut veri yükleme
/// - Form pre-fill
/// - Güncelleme iþlemi
/// - Silme iþlemi
/// </summary>
[Authorize(Roles = "Admin")]
public class DuzenleModel : PageModel
{
    #region Fields & Constructor

    private readonly ICagriBilgisiService _cagriService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DuzenleModel> _logger;

    public DuzenleModel(
        ICagriBilgisiService cagriService,
        ApplicationDbContext context,
        ILogger<DuzenleModel> logger)
    {
        _cagriService = cagriService;
        _context = context;
        _logger = logger;
    }

    #endregion

    #region Properties

    [BindProperty]
    public int CagriId { get; set; }

    [BindProperty]
    public CagriGuncelleViewModel Input { get; set; } = new();

    public SelectList Iller { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
    public SelectList Ilceler { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    // Meta bilgiler (read-only)
    public DateTime OlusturmaTarihi { get; set; }
    public DateTime? GuncellemeTarihi { get; set; }
    public int GoruntulenmeSayisi { get; set; }
    public int TakipciSayisi { get; set; }

    #endregion

    #region Handlers

    public async Task<IActionResult> OnGetAsync(int id)
    {
        CagriId = id;

        var cagri = await _cagriService.GetByIdAsync(id);
        if (cagri == null)
        {
            return NotFound();
        }

        // ViewModel'e mapping
        Input = new CagriGuncelleViewModel
        {
            Baslik = cagri.Baslik,
            //KisaAciklama = cagri.KisaAciklama,
            Aciklama = cagri.Aciklama,
            Turu = cagri.Turu,
            Kurum = cagri.Kurum,
            BaslangicTarihi = cagri.BaslangicTarihi,
            BitisTarihi = cagri.BitisTarihi,
            BasvuruLinki = cagri.BasvuruLinki,
            IletisimEmail = cagri.IletisimEmail,
            IletisimTelefon = cagri.IletisimTelefon,
            HedefKitle = cagri.HedefKitle,
            HedefIlId = cagri.HedefIlId,
            HedefIlceId = cagri.HedefIlceId
        };

        // Meta bilgiler
        OlusturmaTarihi = cagri.OlusturmaTarihi;
        GuncellemeTarihi = cagri.GuncellemeTarihi;
        GoruntulenmeSayisi = cagri.GoruntulenmeSayisi;
        TakipciSayisi = cagri.TakipciSayisi;

        await LoadDropdownsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return Page();
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "Kullanýcý kimliði bulunamadý");
                await LoadDropdownsAsync();
                return Page();
            }

            var (success, message) = await _cagriService.UpdateAsync(CagriId, Input, userId);

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, message);
                await LoadDropdownsAsync();
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çaðrý güncellenirken hata: {Id}", CagriId);
            ModelState.AddModelError(string.Empty, "Beklenmeyen bir hata oluþtu");
            await LoadDropdownsAsync();
            return Page();
        }
    }

    #endregion

    #region Private Methods

    private async Task LoadDropdownsAsync()
    {
        // Ýller
        var iller = await _context.Iller
            .OrderBy(i => i.Ad)
            .Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Ad
            })
            .ToListAsync();

        Iller = new SelectList(iller, "Value", "Text", Input.HedefIlId);

        // Ýlçeler
        if (Input.HedefIlId.HasValue)
        {
            var ilceler = await _context.Ilceler
                .Where(i => i.IlId == Input.HedefIlId.Value)
                .OrderBy(i => i.Ad)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Ad
                })
                .ToListAsync();

            Ilceler = new SelectList(ilceler, "Value", "Text", Input.HedefIlceId);
        }
    }

    #endregion
}

/*
 * PAGEMODEL AÇIKLAMASI:
 * ======================
 * Admin Çaðrý Düzenleme Backend
 * 
 * Özellikler:
 * - Mevcut veri yükleme (OnGetAsync)
 * - Form pre-fill
 * - ViewModel mapping
 * - Update iþlemi (OnPostAsync)
 * - Dropdown cascade (Ýl-Ýlçe)
 * - Meta bilgi gösterimi
 * 
 * Güvenlik:
 * - [Authorize(Roles = "Admin")]
 * - NotFound() kontrolü
 * - UserId validation
 * 
 * Ýþ Akýþý:
 * 1. GET: Çaðrý verisi yükle
 * 2. Formu doldur
 * 3. POST: Validate
 * 4. Service çaðýr
 * 5. Redirect veya hata göster
 */