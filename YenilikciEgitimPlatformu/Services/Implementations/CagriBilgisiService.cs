using Microsoft.EntityFrameworkCore;
using YenilikciEgitimPlatformu.Data;
using YenilikciEgitimPlatformu.Models;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.CagriBilgisi;
using System.Text.RegularExpressions;

namespace YenilikciEgitimPlatformu.Services.Implementations;

/// <summary>
/// Çağrı Bilgisi (Sistem 1) servisi implementasyonu
/// Resmi kurum çağrılarının CRUD, takip ve arama işlemleri
/// 
/// [Mimari Notu]
/// Bu servis eventual consistency kabul eder.
/// DB atomic, cache ve SignalR side-effect olarak ele alınır.
/// 
/// [DÜZELTME NOTU - 2026-01-27]
/// Tüm property isimleri mevcut model yapısıyla uyumlu hale getirildi.
/// </summary>
public class CagriBilgisiService : ICagriBilgisiService
{
    #region Fields & Constructor

    private readonly ApplicationDbContext _context;
    private readonly ILogger<CagriBilgisiService> _logger;

    public CagriBilgisiService(
        ApplicationDbContext context,
        ILogger<CagriBilgisiService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #endregion

    #region CRUD İşlemleri

    public async Task<(List<CagriListViewModel> Data, int TotalCount)> GetAllAsync(CagriFiltreleViewModel filtre)
    {
        try
        {
            var query = _context.CagriBilgileri
                .Include(c => c.HedefIl)
                .Include(c => c.HedefIlce)
                .Where(c => !c.SilindiMi);

            // Filtreleme
            if (filtre.SadeceAktif)
                query = query.Where(c => c.YayindaMi);

            if (!string.IsNullOrWhiteSpace(filtre.Arama))
            {
                var searchTerm = filtre.Arama.ToLower();
                query = query.Where(c =>
                    c.Baslik.ToLower().Contains(searchTerm) ||
                    c.KurumAdi.ToLower().Contains(searchTerm) ||
                    (c.KisaAciklama != null && c.KisaAciklama.ToLower().Contains(searchTerm)));
            }

            if (filtre.Turu.HasValue)
                query = query.Where(c => c.CagriTuru == filtre.Turu.Value);

            if (!string.IsNullOrWhiteSpace(filtre.Kurum))
                query = query.Where(c => c.KurumAdi.ToLower().Contains(filtre.Kurum.ToLower()));

            if (filtre.IlId.HasValue)
                query = query.Where(c => c.HedefIlId == filtre.IlId.Value);

            if (filtre.IlceId.HasValue)
                query = query.Where(c => c.HedefIlceId == filtre.IlceId.Value);

            if (filtre.BaslangicTarihiMin.HasValue)
                query = query.Where(c => c.CagriBaslangicTarihi >= filtre.BaslangicTarihiMin.Value);

            if (filtre.BaslangicTarihiMax.HasValue)
                query = query.Where(c => c.CagriBaslangicTarihi <= filtre.BaslangicTarihiMax.Value);

            // Toplam sayı
            var totalCount = await query.CountAsync();

            // Sıralama
            query = filtre.SortBy switch
            {
                "Baslik" => filtre.SortDescending ? query.OrderByDescending(c => c.Baslik) : query.OrderBy(c => c.Baslik),
                "Kurum" => filtre.SortDescending ? query.OrderByDescending(c => c.KurumAdi) : query.OrderBy(c => c.KurumAdi),
                "BitisTarihi" => filtre.SortDescending ? query.OrderByDescending(c => c.CagriBitisTarihi) : query.OrderBy(c => c.CagriBitisTarihi),
                _ => filtre.SortDescending ? query.OrderByDescending(c => c.CagriBaslangicTarihi) : query.OrderBy(c => c.CagriBaslangicTarihi)
            };

            // Sayfalama
            var data = await query
                .Skip((filtre.Page - 1) * filtre.PageSize)
                .Take(filtre.PageSize)
                .Select(c => new CagriListViewModel
                {
                    Id = c.Id,
                    Baslik = c.Baslik,
                    KisaAciklama = c.KisaAciklama,
                    KapakResmiUrl = c.KapakGorseliUrl,
                    Turu = c.CagriTuru,
                    TuruText = c.CagriTuru.ToString(),
                    BaslangicTarihi = c.CagriBaslangicTarihi,
                    BitisTarihi = c.CagriBitisTarihi,
                    Kurum = c.KurumAdi,
                    TakipciSayisi = c.Takipciler.Count,
                    GoruntulenmeSayisi = c.GoruntulenmeSayisi,
                    Aktif = c.YayindaMi,
                    Slug = c.Slug
                })
                .ToListAsync();

            return (data, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çağrılar listelenirken hata oluştu");
            throw;
        }
    }

    public async Task<CagriDetayViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var cagri = await _context.CagriBilgileri
                .Include(c => c.HedefIl)
                .Include(c => c.HedefIlce)
                .Include(c => c.EkDosyalar)
                .FirstOrDefaultAsync(c => c.Id == id && !c.SilindiMi);

            if (cagri == null)
                return null;

            return await MapToDetayViewModelAsync(cagri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çağrı detayı getirilirken hata: {Id}", id);
            throw;
        }
    }

    public async Task<CagriDetayViewModel?> GetBySlugAsync(string slug)
    {
        try
        {
            var cagri = await _context.CagriBilgileri
                .Include(c => c.HedefIl)
                .Include(c => c.HedefIlce)
                .Include(c => c.EkDosyalar)
                .FirstOrDefaultAsync(c => c.Slug == slug && !c.SilindiMi);

            if (cagri == null)
                return null;

            return await MapToDetayViewModelAsync(cagri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çağrı detayı getirilirken hata: {Slug}", slug);
            throw;
        }
    }

    public async Task<(bool Success, int? Id, string Message)> CreateAsync(CagriOlusturViewModel model, string userId)
    {
        try
        {
            var slug = await GenerateUniqueSlugAsync(model.Baslik);

            var cagri = new CagriBilgisi
            {
                Baslik = model.Baslik,
                KisaAciklama = model.KisaAciklama,
                DetayliAciklama = model.Aciklama,
                CagriTuru = model.Turu,
                KurumAdi = model.Kurum,
                CagriBaslangicTarihi = model.BaslangicTarihi,
                CagriBitisTarihi = model.BitisTarihi,
                BasvuruLinki = model.BasvuruLinki,
                IletisimEmail = model.IletisimEmail,
                IletisimTelefon = model.IletisimTelefon,
                HedefKitle = model.HedefKitle,
                HedefIlId = model.HedefIlId,
                HedefIlceId = model.HedefIlceId,
                Slug = slug,
                YayindaMi = true,
                OlusturanKullaniciId = userId,
                OlusturulmaTarihi = DateTime.UtcNow
            };

            _context.CagriBilgileri.Add(cagri);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Yeni çağrı oluşturuldu: {Id} - {Baslik}", cagri.Id, cagri.Baslik);

            return (true, cagri.Id, "Çağrı başarıyla oluşturuldu");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çağrı oluşturulurken hata");
            return (false, null, "Çağrı oluşturulurken bir hata oluştu");
        }
    }

    public async Task<(bool Success, string Message)> UpdateAsync(int id, CagriGuncelleViewModel model, string userId)
    {
        try
        {
            var cagri = await _context.CagriBilgileri.FindAsync(id);
            if (cagri == null || cagri.SilindiMi)
                return (false, "Çağrı bulunamadı");

            // Slug güncelleme (başlık değiştiyse)
            if (cagri.Baslik != model.Baslik)
                cagri.Slug = await GenerateUniqueSlugAsync(model.Baslik);

            cagri.Baslik = model.Baslik;
            cagri.KisaAciklama = model.KisaAciklama;
            cagri.DetayliAciklama = model.Aciklama;
            cagri.CagriTuru = model.Turu;
            cagri.KurumAdi = model.Kurum;
            cagri.CagriBaslangicTarihi = model.BaslangicTarihi;
            cagri.CagriBitisTarihi = model.BitisTarihi;
            cagri.BasvuruLinki = model.BasvuruLinki;
            cagri.IletisimEmail = model.IletisimEmail;
            cagri.IletisimTelefon = model.IletisimTelefon;
            cagri.HedefKitle = model.HedefKitle;
            cagri.HedefIlId = model.HedefIlId;
            cagri.HedefIlceId = model.HedefIlceId;
            cagri.GuncelleyenKullaniciId = userId;
            cagri.GuncellenmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Çağrı güncellendi: {Id} - {Baslik}", cagri.Id, cagri.Baslik);

            return (true, "Çağrı başarıyla güncellendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çağrı güncellenirken hata: {Id}", id);
            return (false, "Çağrı güncellenirken bir hata oluştu");
        }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id, string userId)
    {
        try
        {
            var cagri = await _context.CagriBilgileri.FindAsync(id);
            if (cagri == null || cagri.SilindiMi)
                return (false, "Çağrı bulunamadı");

            // Soft delete
            cagri.SilindiMi = true;
            cagri.SilenKullaniciId = userId;
            cagri.SilinmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Çağrı silindi (soft): {Id} - {Baslik}", cagri.Id, cagri.Baslik);

            return (true, "Çağrı başarıyla silindi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çağrı silinirken hata: {Id}", id);
            return (false, "Çağrı silinirken bir hata oluştu");
        }
    }

    #endregion

    #region Takip İşlemleri

    public async Task<(bool Success, string Message)> TakipEtAsync(int cagriId, string userId)
    {
        try
        {
            var mevcutTakip = await _context.CagriTakipleri
                .FirstOrDefaultAsync(t => t.CagriBilgisiId == cagriId && t.KullaniciId == userId);

            if (mevcutTakip != null)
                return (false, "Bu çağrıyı zaten takip ediyorsunuz");

            var takip = new CagriTakip
            {
                CagriBilgisiId = cagriId,
                KullaniciId = userId,
                TakipTarihi = DateTime.UtcNow,
                EmailBildirimiAlsin = true,
                OlusturulmaTarihi = DateTime.UtcNow
            };

            _context.CagriTakipleri.Add(takip);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Çağrı takip edildi: {CagriId} - Kullanıcı: {UserId}", cagriId, userId);

            return (true, "Çağrı takip listesine eklendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çağrı takip edilirken hata: {CagriId}", cagriId);
            return (false, "Takip eklenirken bir hata oluştu");
        }
    }

    public async Task<(bool Success, string Message)> TakipBirakAsync(int cagriId, string userId)
    {
        try
        {
            var takip = await _context.CagriTakipleri
                .FirstOrDefaultAsync(t => t.CagriBilgisiId == cagriId && t.KullaniciId == userId);

            if (takip == null)
                return (false, "Bu çağrıyı takip etmiyorsunuz");

            _context.CagriTakipleri.Remove(takip);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Çağrı takip bırakıldı: {CagriId} - Kullanıcı: {UserId}", cagriId, userId);

            return (true, "Takip listesinden çıkarıldı");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çağrı takip bırakılırken hata: {CagriId}", cagriId);
            return (false, "Takip çıkarılırken bir hata oluştu");
        }
    }

    public async Task<bool> TakipEdiyorMuAsync(int cagriId, string userId)
    {
        return await _context.CagriTakipleri
            .AnyAsync(t => t.CagriBilgisiId == cagriId && t.KullaniciId == userId);
    }

    public async Task<int> GetTakipciSayisiAsync(int cagriId)
    {
        return await _context.CagriTakipleri
            .CountAsync(t => t.CagriBilgisiId == cagriId);
    }

    #endregion

    #region Arama ve Filtreleme

    public async Task<List<CagriListViewModel>> SearchAsync(string keyword, int maxResults = 10)
    {
        try
        {
            var searchTerm = keyword.ToLower();

            return await _context.CagriBilgileri
                .Where(c => !c.SilindiMi && c.YayindaMi)
                .Where(c =>
                    c.Baslik.ToLower().Contains(searchTerm) ||
                    c.KurumAdi.ToLower().Contains(searchTerm) ||
                    (c.KisaAciklama != null && c.KisaAciklama.ToLower().Contains(searchTerm)))
                .OrderByDescending(c => c.OlusturulmaTarihi)
                .Take(maxResults)
                .Select(c => new CagriListViewModel
                {
                    Id = c.Id,
                    Baslik = c.Baslik,
                    KisaAciklama = c.KisaAciklama,
                    Kurum = c.KurumAdi,
                    Turu = c.CagriTuru,
                    TuruText = c.CagriTuru.ToString(),
                    Slug = c.Slug
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Çağrı aranırken hata: {Keyword}", keyword);
            return new List<CagriListViewModel>();
        }
    }

    public async Task<List<CagriListViewModel>> GetPopulerCagrilarAsync(int adet = 5)
    {
        try
        {
            return await _context.CagriBilgileri
                .Where(c => !c.SilindiMi && c.YayindaMi)
                .OrderByDescending(c => c.Takipciler.Count)
                .ThenByDescending(c => c.GoruntulenmeSayisi)
                .Take(adet)
                .Select(c => new CagriListViewModel
                {
                    Id = c.Id,
                    Baslik = c.Baslik,
                    KisaAciklama = c.KisaAciklama,
                    KapakResmiUrl = c.KapakGorseliUrl,
                    Kurum = c.DuzenlenenKurum,
                    Turu = c.CagriTuru,
                    TuruText = c.CagriTuru.ToString(),
                    TakipciSayisi = c.Takipciler.Count,
                    Slug = c.Slug
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Popüler çağrılar getirilirken hata");
            return new List<CagriListViewModel>();
        }
    }

    public async Task<List<CagriListViewModel>> GetYaklasanCagrilarAsync(int adet = 5)
    {
        try
        {
            var bugun = DateTime.Now.Date;

            return await _context.CagriBilgileri
                .Where(c => !c.SilindiMi && c.YayindaMi && c.CagriBitisTarihi.HasValue && c.CagriBitisTarihi.Value >= bugun)
                .OrderBy(c => c.CagriBitisTarihi)
                .Take(adet)
                .Select(c => new CagriListViewModel
                {
                    Id = c.Id,
                    Baslik = c.Baslik,
                    KisaAciklama = c.KisaAciklama,
                    Kurum = c.KurumAdi,
                    BitisTarihi = c.CagriBitisTarihi,
                    Turu = c.CagriTuru,
                    TuruText = c.CagriTuru.ToString(),
                    Slug = c.Slug
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Yaklaşan çağrılar getirilirken hata");
            return new List<CagriListViewModel>();
        }
    }

    #endregion

    #region Yardımcı Metotlar

    public async Task IncrementViewCountAsync(int cagriId)
    {
        try
        {
            var cagri = await _context.CagriBilgileri.FindAsync(cagriId);
            if (cagri != null && !cagri.SilindiMi)
            {
                cagri.GoruntulenmeSayisi++;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Görüntülenme sayısı artırılırken hata: {CagriId}", cagriId);
        }
    }

    public async Task<string> GenerateUniqueSlugAsync(string baslik)
    {
        var slug = CreateSlug(baslik);
        var originalSlug = slug;
        var counter = 1;

        while (await _context.CagriBilgileri.AnyAsync(c => c.Slug == slug))
        {
            slug = $"{originalSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    #endregion

    #region Private Helper Methods

    private async Task<CagriDetayViewModel> MapToDetayViewModelAsync(CagriBilgisi cagri, string? userId = null)
    {
        var model = new CagriDetayViewModel
        {
            Id = cagri.Id,
            Baslik = cagri.Baslik,
            KapakResmiUrl = cagri.KapakGorseliUrl,
            Aciklama = cagri.DetayliAciklama,
            Turu = cagri.CagriTuru,
            TuruText = cagri.CagriTuru.ToString(),
            Kurum = cagri.KurumAdi,
            KurumLogoUrl = cagri.DuzenlenenKurumLogoUrl,
            BaslangicTarihi = cagri.CagriBaslangicTarihi,
            BitisTarihi = cagri.CagriBitisTarihi,
            BasvuruLinki = cagri.BasvuruLinki,
            IletisimEmail = cagri.IletisimEmail,
            IletisimTelefon = cagri.IletisimTelefon,
            HedefKitle = cagri.HedefKitle,
            HedefIlId = cagri.HedefIlId,
            HedefIlAdi = cagri.HedefIl?.Ad,
            HedefIlceId = cagri.HedefIlceId,
            HedefIlceAdi = cagri.HedefIlce?.Ad,
            TakipciSayisi = await GetTakipciSayisiAsync(cagri.Id),
            GoruntulenmeSayisi = cagri.GoruntulenmeSayisi,
            KullaniciTakipEdiyor = !string.IsNullOrEmpty(userId) && await TakipEdiyorMuAsync(cagri.Id, userId),
            OlusturmaTarihi = cagri.OlusturulmaTarihi,
            GuncellemeTarihi = cagri.GuncellenmeTarihi,
            Slug = cagri.Slug,
            EkDosyalar = cagri.EkDosyalar.Select(d => new CagriDosyaViewModel
            {
                Id = d.Id,
                DosyaAdi = d.DosyaUrl,
                DosyaUrl = d.DosyaUrl,
                DosyaTuru = d.DosyaTuru,
                DosyaTuruText = d.DosyaTuru.ToString(),
                DosyaBoyutu = d.DosyaBoyutu ?? 0,
                DosyaBoyutuText = FormatFileSize(d.DosyaBoyutu ?? 0)
            }).ToList()
        };

        return model;
    }

    private static string CreateSlug(string text)
    {
        var turkishMap = new Dictionary<char, char>
        {
            {'ç', 'c'}, {'Ç', 'c'}, {'ğ', 'g'}, {'Ğ', 'g'},
            {'ı', 'i'}, {'İ', 'i'}, {'ö', 'o'}, {'Ö', 'o'},
            {'ş', 's'}, {'Ş', 's'}, {'ü', 'u'}, {'Ü', 'u'}
        };

        var slug = text.ToLower();
        foreach (var pair in turkishMap)
        {
            slug = slug.Replace(pair.Key, pair.Value);
        }

        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return slug;
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    #endregion
}

/*
 * SERVIS AÇIKLAMASI:
 * ==================
 * CagriBilgisiService - Resmi kurum çağrılarının yönetimi
 * 
 * Özellikler:
 * - CRUD işlemleri (Create, Read, Update, Delete)
 * - Takip sistemi (Follow/Unfollow)
 * - Arama ve filtreleme
 * - SEO-friendly slug oluşturma
 * - Soft delete
 * - Görüntülenme sayacı
 * 
 * Mimari:
 * - Eventual consistency
 * - Async/Await
 * - Try/Catch + Logging
 * - Clean code principles
 * 
 * DÜZELTİLEN HATALAR (2026-01-27):
 * - BaseEntity property isimleri (SilindiMi, OlusturulmaTarihi, GuncellenmeTarihi)
 * - CagriBilgisi property isimleri (DetayliAciklama, CagriTuru, DuzenlenenKurum, YayindaMi)
 * - CagriEkDosya nullable DosyaBoyutu
 * - CagriTakip EmailBildirimiAlsin
 * - Dictionary<char, char> türü (string yerine)
 * - Navigation property isimleri
 */