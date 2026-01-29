using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YenilikciEgitimPlatformu.Data;
using YenilikciEgitimPlatformu.Models;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.CagriBilgisi;
using System.Security.Claims;

namespace YenilikciEgitimPlatformu.Areas.Admin.Pages.CagriBilgileri;

/// <summary>
/// Admin - Yeni Çaðrý Oluþturma Sayfasý
/// 
/// Özellikler:
/// - Form validasyonu
/// - Ýl-Ýlçe dropdown
/// - Slug otomatik oluþturma
/// - SweetAlert2 feedback
/// </summary>
[Authorize(Roles = "Admin")]
public class OlusturModel : PageModel
{
    #region Fields & Constructor

    private readonly ICagriBilgisiService _cagriService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OlusturModel> _logger;

    public OlusturModel(
        ICagriBilgisiService cagriService,
        ApplicationDbContext context,
        ILogger<OlusturModel> logger)
    {
        _cagriService = cagriService;
        _context = context;
        _logger = logger;
    }

    #endregion

    #region Properties

    [BindProperty]
    public CagriOlusturViewModel Input { get; set; } = new();

    public SelectList Iller { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
    public SelectList Ilceler { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    #endregion

    #region Handlers

    public async Task OnGetAsync()
    {
        await LoadDropdownsAsync();
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

            var (success, id, message) = await _cagriService.CreateAsync(Input, userId);

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
            _logger.LogError(ex, "Çaðrý oluþturulurken hata oluþtu");
            ModelState.AddModelError(string.Empty, "Beklenmeyen bir hata oluþtu. Lütfen tekrar deneyin.");
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

        Iller = new SelectList(iller, "Value", "Text");

        // Ýlçeler (Eðer il seçiliyse)
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

            Ilceler = new SelectList(ilceler, "Value", "Text");
        }
    }

    #endregion
}

/*
 * PAGEMODEL AÇIKLAMASI:
 * ======================
 * Admin Çaðrý Oluþturma Backend
 * 
 * Sorumluluklar:
 * - Form validasyonu
 * - Dropdown yükleme (Ýl-Ýlçe)
 * - Service çaðrýsý
 * - Hata yönetimi
 * - Redirect yönlendirmesi
 * 
 * Güvenlik:
 * - [Authorize(Roles = "Admin")] ile korunmuþ
 * - UserId claim kontrolü
 * - Model validation
 * 
 * Kullanýlan Servisler:
 * - ICagriBilgisiService (CRUD)
 * - ApplicationDbContext (Dropdown)
 */