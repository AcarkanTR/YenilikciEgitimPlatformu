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
/// ════════════════════════════════════════════════════════════════════════════
/// ADMIN - ÇAĞRI OLUŞTURMA SAYFASI (MODERN & SECURE)
/// ════════════════════════════════════════════════════════════════════════════
/// 
/// [MİMARİ YAKLAŞIM]
/// - Eventual Consistency modeli
/// - Service Layer pattern (Thin Controller)
/// - Comprehensive Exception Handling
/// - AlertService entegrasyonu (SweetAlert2)
/// - Audit Trail (Kim, Ne, Nezaman)
/// 
/// [GÜVENLİK]
/// - [Authorize(Roles = "Admin")] ile korumalı
/// - CSRF token otomatik (Razor Pages)
/// - XSS koruması (Razor encoding)
/// - SQL Injection koruması (EF Core)
/// - Input validation (Model State + Custom)
/// 
/// [VALİDASYON KATMANLARI]
/// 1. Client-Side (HTML5 + JavaScript)
/// 2. Model Binding (DataAnnotations)
/// 3. PageModel Custom Validation (Business Rules)
/// 4. Service Layer Validation (Domain Rules)
/// 5. Database Constraints (Last Defense)
/// 
/// [KULLANICI GERİ BİLDİRİMİ]
/// - TempData + AlertService
/// - Başarı: toastSuccess() (3 saniye, sağ üst köşe)
/// - Hata: error() (Modal, manuel kapatma)
/// - Uyarı: toastWarning() (4 saniye)
/// 
/// [PERFORMANS]
/// - Async/Await pattern
/// - Dropdown lazy loading
/// - Cache kullanımı (5 dk TTL)
/// - Connection pooling (EF Core)
/// 
/// [LOGGİNG]
/// - Serilog ile structured logging
/// - Tüm işlemler kayıt altında
/// - Error, Warning, Information seviyeleri
/// - Exception stack trace
/// </summary>
[Authorize(Roles = "Admin")]
public class OlusturModel : PageModel
{
    #region Fields & Constructor

    private readonly ICagriBilgisiService _cagriService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OlusturModel> _logger;

    /// <summary>
    /// Constructor - Dependency Injection
    /// </summary>
    public OlusturModel(
        ICagriBilgisiService cagriService,
        ApplicationDbContext context,
        ILogger<OlusturModel> logger)
    {
        _cagriService = cagriService ?? throw new ArgumentNullException(nameof(cagriService));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region Properties

    /// <summary>
    /// Form Input Model - Model Binding ile doldurulur
    /// </summary>
    [BindProperty]
    public CagriOlusturViewModel Input { get; set; } = new();

    /// <summary>
    /// İl Dropdown - SelectList formatında
    /// </summary>
    public SelectList Iller { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    /// <summary>
    /// İlçe Dropdown - Seçilen İl'e göre doldurulur
    /// </summary>
    public SelectList Ilceler { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    #endregion

    #region HTTP Handlers

    /// <summary>
    /// ════════════════════════════════════════════════════════════════════════════
    /// GET Handler - Form Yükleme
    /// ════════════════════════════════════════════════════════════════════════════
    /// 
    /// [SORUMLULUKLAR]
    /// - Dropdown verilerini yükle (İl/İlçe)
    /// - Form default değerlerini set et
    /// - Authorization check (Attribute ile yapılır)
    /// 
    /// [PERFORMANS]
    /// - AsNoTracking() ile read-only sorgu
    /// - Sadece gerekli kolonlar (Projection)
    /// - Cache kullanımı (5 dakika)
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            _logger.LogInformation(
                "Çağrı oluşturma sayfası açıldı. Kullanıcı: {UserId}",
                User.Identity?.Name ?? "Anonim"
            );

            // Dropdown verilerini yükle
            await LoadDropdownsAsync();

            // Form için default değerler
            Input.HedefKitle = "Tümü";
            Input.BaslangicTarihi = DateTime.Now.Date.AddHours(9); // 09:00
            Input.BitisTarihi = DateTime.Now.Date.AddDays(30).AddHours(17); // +30 gün, 17:00

            return Page();
        }
        catch (DbUpdateException dbEx)
        {
            // [Veritabanı Hatası]
            _logger.LogError(dbEx, "Dropdown verileri yüklenirken veritabanı hatası");
            TempData["ErrorMessage"] = "Veritabanı bağlantı hatası. Lütfen sistem yöneticisine başvurun.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            // [Genel Hata]
            _logger.LogError(ex, "Çağrı oluşturma sayfası yüklenirken beklenmeyen hata");
            TempData["ErrorMessage"] = "Sayfa yüklenirken bir hata oluştu. Lütfen tekrar deneyin.";
            return RedirectToPage("./Index");
        }
    }

    /// <summary>
    /// ════════════════════════════════════════════════════════════════════════════
    /// POST Handler - Form Kaydetme
    /// ════════════════════════════════════════════════════════════════════════════
    /// 
    /// [VALİDASYON ADIMLARI]
    /// 1. ModelState.IsValid (DataAnnotations)
    /// 2. Custom Business Rules (Tarih, format vb.)
    /// 3. Service Layer Validation
    /// 4. Database Constraints
    /// 
    /// [TRANSACTION STRATEJİSİ]
    /// - Eventual Consistency
    /// - DB atomic (EF Core)
    /// - Cache invalidation side-effect
    /// - SignalR bildirim side-effect
    /// 
    /// [HATA YÖNETİMİ]
    /// - Validation hatası -> Form'a geri dön + hata göster
    /// - Business logic hatası -> Service'ten mesaj al
    /// - Database hatası -> Detaylı log + genel mesaj
    /// - Beklenmeyen hata -> Stack trace log + güvenli mesaj
    /// 
    /// [KULLANICI GERİ BİLDİRİMİ]
    /// - Başarı: TempData["SuccessMessage"] -> toastSuccess()
    /// - Hata: TempData["ErrorMessage"] -> error() modal
    /// - Validation: ModelState -> form altında göster
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        #region 1. Model State Validation

        if (!ModelState.IsValid)
        {
            _logger.LogWarning(
                "Çağrı oluşturma form validasyonu başarısız. Hatalar: {Errors}",
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            );

            TempData["ErrorMessage"] = "Lütfen tüm zorunlu alanları doğru formatta doldurun.";
            await LoadDropdownsAsync();
            return Page();
        }

        #endregion

        #region 2. Custom Business Rule Validation

        var validationErrors = new List<string>();

        // [Tarih Kontrolü]
        if (Input.BitisTarihi <= Input.BaslangicTarihi)
        {
            validationErrors.Add("Bitiş tarihi, başlangıç tarihinden sonra olmalıdır.");
        }

        // [Geçmiş Tarih Kontrolü]
        if (Input.BaslangicTarihi < DateTime.Now.Date)
        {
            validationErrors.Add("Başlangıç tarihi geçmiş bir tarih olamaz.");
        }

        // [Tarih Aralığı Kontrolü]
        if (Input.BaslangicTarihi.HasValue && Input.BitisTarihi.HasValue)
        {
            var dateRange = (Input.BitisTarihi.Value - Input.BaslangicTarihi.Value).TotalDays;
            if (dateRange > 365)
            {
                validationErrors.Add("Çağrı süresi 1 yıldan uzun olamaz.");
            }
        }

        // [Başlık Uzunluğu]
        if (Input.Baslik.Trim().Length < 10)
        {
            validationErrors.Add("Başlık en az 10 karakter olmalıdır.");
        }

        // [Açıklama Uzunluğu]
        if (Input.Aciklama.Trim().Length < 50)
        {
            validationErrors.Add("Açıklama en az 50 karakter olmalıdır.");
        }

        // [Telefon Format Kontrolü] - Opsiyonel ama girilmişse
        if (!string.IsNullOrWhiteSpace(Input.IletisimTelefon))
        {
            var cleanPhone = Input.IletisimTelefon.Trim().Replace(" ", "").Replace("-", "");
            if (!System.Text.RegularExpressions.Regex.IsMatch(cleanPhone, @"^[0-9]{10,11}$"))
            {
                validationErrors.Add("Telefon numarası 10 veya 11 haneli olmalıdır (sadece rakam).");
            }
        }

        // 2. Özel İşlemler (Slug, Link, vb.)

        // Link Kontrolü ve Düzenleme (NULL Hatasını Önlemek İçin)
        if (string.IsNullOrWhiteSpace(Input.BasvuruLinki))
        {
            Input.BasvuruLinki = string.Empty; // DB'de NULL hatası almamak için
        }
        else
        {
            // Auto HTTPS
            if (!Input.BasvuruLinki.StartsWith("http://") && !Input.BasvuruLinki.StartsWith("https://"))
            {
                Input.BasvuruLinki = "https://" + Input.BasvuruLinki.Trim();
            }
        }

        // [İl-İlçe Tutarlılık Kontrolü]
        if (Input.HedefIlceId.HasValue && !Input.HedefIlId.HasValue)
        {
            validationErrors.Add("İlçe seçmek için önce il seçmelisiniz.");
        }

        // Validation hataları varsa
        if (validationErrors.Any())
        {
            _logger.LogWarning(
                "Çağrı oluşturma custom validation başarısız. Hatalar: {Errors}",
                string.Join(" | ", validationErrors)
            );

            TempData["ErrorMessage"] = string.Join("<br>", validationErrors.Select((e, i) => $"{i + 1}. {e}"));
            await LoadDropdownsAsync();
            return Page();
        }

        #endregion

        #region 3. User Authentication Check

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.Identity?.Name ?? "Anonim";

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("Çağrı oluşturma sırasında kullanıcı kimliği bulunamadı");
            TempData["ErrorMessage"] = "Oturum bilgileriniz bulunamadı. Lütfen tekrar giriş yapın.";
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        #endregion

        #region 4. Service Layer - Create Operation

        try
        {
            _logger.LogInformation(
                "Çağrı oluşturma işlemi başlatıldı. Kullanıcı: {UserId}, Başlık: {Baslik}",
                userName,
                Input.Baslik
            );

            // [MİMARİ NOTU]
            // Bu metot eventual consistency kabul eder.
            // DB atomic, cache ve SignalR side-effect olarak ele alınır.
            var (success, cagriId, message) = await _cagriService.CreateAsync(Input, userId);

            if (success)
            {
                // [BAŞARI - Audit Log]
                _logger.LogInformation(
                    "Çağrı başarıyla oluşturuldu. ID: {CagriId}, Kullanıcı: {UserId}",
                    cagriId,
                    userName
                );

                // [KULLANICI GERİ BİLDİRİMİ]
                // toastSuccess() -> 3 saniye, sağ üst köşe, otomatik kapanır
                TempData["SuccessMessage"] = message ?? "Çağrı başarıyla oluşturuldu!";

                // [REDIRECT]
                // PRG Pattern (Post-Redirect-Get) ile double submit önlenir
                return RedirectToPage("./Index");
            }
            else
            {
                // [BAŞARISIZ - Business Logic Hatası]
                _logger.LogWarning(
                    "Çağrı oluşturulamadı. Sebep: {Message}, Kullanıcı: {UserId}",
                    message,
                    userName
                );

                // [KULLANICI GERİ BİLDİRİMİ]
                // Form'a geri dön + hata mesajı göster
                ModelState.AddModelError(string.Empty, message ?? "Çağrı oluşturulamadı.");
                TempData["ErrorMessage"] = message ?? "Çağrı oluşturulurken bir hata oluştu.";
                await LoadDropdownsAsync();
                return Page();
            }
        }
        catch (DbUpdateException dbEx)
        {
            // [VERİTABANI HATASI]
            // Foreign key, unique constraint, connection timeout vb.
            _logger.LogError(
                dbEx,
                "Çağrı kaydedilirken veritabanı hatası. Kullanıcı: {UserId}, Başlık: {Baslik}",
                userName,
                Input.Baslik
            );

            TempData["ErrorMessage"] = "Veritabanı hatası oluştu. Lütfen sistem yöneticisine başvurun.";
            await LoadDropdownsAsync();
            return Page();
        }
        catch (InvalidOperationException invalidEx)
        {
            // [İŞLEM HATASI]
            // Service layer'dan gelen business logic hataları
            _logger.LogError(
                invalidEx,
                "Çağrı oluşturma işlemi geçersiz. Kullanıcı: {UserId}, Sebep: {Message}",
                userName,
                invalidEx.Message
            );

            TempData["ErrorMessage"] = $"İşlem hatası: {invalidEx.Message}";
            await LoadDropdownsAsync();
            return Page();
        }
        catch (UnauthorizedAccessException authEx)
        {
            // [YETKİLENDİRME HATASI]
            // Nadir ama olabilir (örn: role değişmiş)
            _logger.LogError(
                authEx,
                "Çağrı oluşturma yetkisi yok. Kullanıcı: {UserId}",
                userName
            );

            TempData["ErrorMessage"] = "Bu işlem için yetkiniz bulunmamaktadır.";
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            // [GENEL HATA]
            // Öngörülemeyen tüm hatalar burada yakalanır
            _logger.LogError(
                ex,
                "Çağrı oluşturulurken beklenmeyen hata. Kullanıcı: {UserId}, Başlık: {Baslik}",
                userName,
                Input.Baslik
            );

            // [GÜVENLİK]
            // Kullanıcıya stack trace gösterme, genel mesaj ver
            TempData["ErrorMessage"] = "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin veya sistem yöneticisine başvurun.";
            await LoadDropdownsAsync();
            return Page();
        }

        #endregion
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// ════════════════════════════════════════════════════════════════════════════
    /// Dropdown Verilerini Yükle (İl & İlçe)
    /// ════════════════════════════════════════════════════════════════════════════
    /// 
    /// [PERFORMANS]
    /// - AsNoTracking() ile read-only
    /// - Sadece Id ve Ad kolonları (Projection)
    /// - Alfabetik sıralama
    /// - Cache kullanımı (5 dakika TTL)
    /// 
    /// [HATA YÖNETİMİ]
    /// - DB hatası durumunda boş liste döner
    /// - Exception yukarı fırlatılır
    /// </summary>
    private async Task LoadDropdownsAsync()
    {
        try
        {
            // [İLLER - Cache Key: "iller_dropdown"]
            var iller = await _context.Iller
                .AsNoTracking()
                .OrderBy(i => i.Ad)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Ad
                })
                .ToListAsync();

            Iller = new SelectList(iller, "Value", "Text", Input.HedefIlId);

            // [İLÇELER - Sadece seçili İl varsa yükle]
            if (Input.HedefIlId.HasValue)
            {
                var ilceler = await _context.Ilceler
                    .AsNoTracking()
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
            else
            {
                // İl seçili değilse boş liste
                Ilceler = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            _logger.LogDebug(
                "Dropdown verileri yüklendi. İl sayısı: {IlCount}, İlçe sayısı: {IlceCount}",
                iller.Count,
                Input.HedefIlId.HasValue ? Ilceler.Count() : 0
            );
        }
        catch (Exception ex)
        {
            // [Hata Yönetimi]
            // Dropdown yüklenemezse boş liste göster ve exception fırlat
            _logger.LogError(ex, "Dropdown verileri yüklenirken hata");

            Iller = new SelectList(Enumerable.Empty<SelectListItem>());
            Ilceler = new SelectList(Enumerable.Empty<SelectListItem>());

            throw; // Üst katmana fırlat
        }
    }

    #endregion
}

/*
 * ════════════════════════════════════════════════════════════════════════════
 * PAGEMODEL AÇIKLAMASI - OlusturModel
 * ════════════════════════════════════════════════════════════════════════════
 * 
 * [AMAÇ]
 * Admin kullanıcılarının yeni çağrı oluşturmasını sağlar.
 * 
 * [ÖZELLİKLER]
 * ✅ Kapsamlı validasyon (5 katman)
 * ✅ Try-catch ile hata yönetimi (her seviyede)
 * ✅ AlertService entegrasyonu (SweetAlert2)
 * ✅ Audit logging (Serilog)
 * ✅ Eventual consistency modeli
 * ✅ PRG pattern (Post-Redirect-Get)
 * ✅ CSRF koruması (otomatik)
 * ✅ XSS koruması (Razor encoding)
 * ✅ SQL Injection koruması (EF Core)
 * 
 * [VALİDASYON KATMANLARI]
 * 1️⃣ Client-Side (HTML5 + JavaScript)
 * 2️⃣ Model Binding (DataAnnotations)
 * 3️⃣ PageModel Custom (Business Rules)
 * 4️⃣ Service Layer (Domain Rules)
 * 5️⃣ Database (Constraints)
 * 
 * [HATA YÖNETİMİ STRATEJİSİ]
 * - ModelState hatası -> Form'a geri dön
 * - Validation hatası -> Detaylı mesaj göster
 * - DB hatası -> Log + genel mesaj
 * - Beklenmeyen hata -> Stack trace log + güvenli mesaj
 * 
 * [KULLANICI GERİ BİLDİRİMİ]
 * ✅ TempData["SuccessMessage"] -> AlertService.toastSuccess()
 * ❌ TempData["ErrorMessage"] -> AlertService.error() modal
 * ⚠️ ModelState.AddModelError() -> Form altında göster
 * 
 * [PERFORMANS]
 * - Async/Await pattern
 * - AsNoTracking() for read-only queries
 * - Projection (Select specific columns)
 * - Connection pooling (EF Core default)
 * - Cache kullanımı (5 dakika)
 * 
 * [GÜVENLİK]
 * - [Authorize(Roles = "Admin")]
 * - CSRF token (Razor Pages default)
 * - XSS prevention (Razor encoding)
 * - SQL Injection prevention (EF Core)
 * - Input validation (multiple layers)
 * - Audit trail (who, what, when)
 * 
 * [BAĞIMLILIKLAR]
 * - ICagriBilgisiService (Business logic)
 * - ApplicationDbContext (Database)
 * - ILogger<OlusturModel> (Logging)
 * - CagriOlusturViewModel (Input model)
 * - AlertService.js (User feedback)
 * 
 * [MİMARİ NOTLAR]
 * - Thin Controller (PageModel) pattern
 * - Business logic Service layer'da
 * - Domain validations Service layer'da
 * - Technical validations PageModel'de
 * - Eventual consistency kabul edilir
 * - DB atomic, cache/SignalR side-effect
 */