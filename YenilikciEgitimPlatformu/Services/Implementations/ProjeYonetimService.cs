using Microsoft.EntityFrameworkCore;
using YenilikciEgitimPlatformu.Data;
using YenilikciEgitimPlatformu.Models;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.ProjeYonetim;
using System.Text.RegularExpressions;

namespace YenilikciEgitimPlatformu.Services.Implementations;

/// <summary>
/// Proje Yönetimi (Sistem 2) servisi implementasyonu
/// Kullanıcı projelerinin CRUD, ekip yönetimi ve yetkilendirme işlemleri
/// 
/// [Mimari Notu]
/// Bu servis eventual consistency kabul eder.
/// DB atomic, cache ve SignalR side-effect olarak ele alınır.
/// 
/// [DÜZELTME NOTU - 2026-01-27]
/// Tüm property isimleri mevcut model yapısıyla uyumlu hale getirildi.
/// </summary>
public class ProjeYonetimService : IProjeYonetimService
{
    #region Fields & Constructor

    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProjeYonetimService> _logger;

    public ProjeYonetimService(
        ApplicationDbContext context,
        ILogger<ProjeYonetimService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #endregion

    #region CRUD İşlemleri

    public async Task<(List<ProjeListViewModel> Data, int TotalCount)> GetAllAsync(ProjeFiltreleViewModel filtre)
    {
        try
        {
            var query = _context.ProjeYonetimleri
                .Include(p => p.Kategori)
                .Include(p => p.Kurucu)
                .Include(p => p.EkipUyeleri)
                .Where(p => !p.SilindiMi);

            // Filtreleme
            if (filtre.SadeceHerkeseAcik)
                query = query.Where(p => p.HerkeseAcikMi);

            if (filtre.SadeceKendiProjelerim && !string.IsNullOrEmpty(filtre.UserId))
                query = query.Where(p => p.KurucuKullaniciId == filtre.UserId);

            if (!string.IsNullOrWhiteSpace(filtre.Arama))
            {
                var searchTerm = filtre.Arama.ToLower();
                query = query.Where(p =>
                    p.ProjeAdi.ToLower().Contains(searchTerm) ||
                    (p.KisaAciklama != null && p.KisaAciklama.ToLower().Contains(searchTerm)));
            }

            if (filtre.KategoriId.HasValue)
                query = query.Where(p => p.KategoriId == filtre.KategoriId.Value);

            if (filtre.Durum.HasValue)
                query = query.Where(p => p.Durum == filtre.Durum.Value);

            if (filtre.IlId.HasValue)
                query = query.Where(p => p.Okul != null && p.Okul.IlId == filtre.IlId.Value);

            // Toplam sayı
            var totalCount = await query.CountAsync();

            // Sıralama
            query = filtre.SortBy switch
            {
                "Baslik" => filtre.SortDescending ? query.OrderByDescending(p => p.ProjeAdi) : query.OrderBy(p => p.ProjeAdi),
                "GoruntulenmeSayisi" => filtre.SortDescending ? query.OrderByDescending(p => p.GoruntulenmeSayisi) : query.OrderBy(p => p.GoruntulenmeSayisi),
                "BegeniSayisi" => filtre.SortDescending ? query.OrderByDescending(p => p.BegeniSayisi) : query.OrderBy(p => p.BegeniSayisi),
                _ => filtre.SortDescending ? query.OrderByDescending(p => p.OlusturulmaTarihi) : query.OrderBy(p => p.OlusturulmaTarihi)
            };

            // Sayfalama
            var data = await query
                .Skip((filtre.Page - 1) * filtre.PageSize)
                .Take(filtre.PageSize)
                .Select(p => new ProjeListViewModel
                {
                    Id = p.Id,
                    Baslik = p.ProjeAdi,
                    KisaAciklama = p.KisaAciklama,
                    KapakResmiUrl = p.KapakGorseliUrl,
                    Durum = p.Durum,
                    DurumText = p.Durum.ToString(),
                    KategoriAdi = p.Kategori != null ? p.Kategori.Ad : null,
                    KurucuAdSoyad = p.Kurucu.Ad + " " + p.Kurucu.Soyad,
                    EkipUyeSayisi = p.EkipUyeleri.Count,
                    IlerlemeYuzdesi = p.IlerlemeYuzdesi,
                    GoruntulenmeSayisi = p.GoruntulenmeSayisi,
                    BegeniSayisi = p.BegeniSayisi,
                    OlusturmaTarihi = p.OlusturulmaTarihi,
                    Slug = p.Slug
                })
                .ToListAsync();

            return (data, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Projeler listelenirken hata oluştu");
            throw;
        }
    }

    public async Task<ProjeDetayViewModel?> GetByIdAsync(int id)
    {
        try
        {
            var proje = await _context.ProjeYonetimleri
                .Include(p => p.Kategori)
                .Include(p => p.Kurucu)
                .Include(p => p.KaynakCagriBilgisi)
                .Include(p => p.EkipUyeleri).ThenInclude(e => e.Kullanici)
                .Include(p => p.Gorevler)
                .FirstOrDefaultAsync(p => p.Id == id && !p.SilindiMi);

            if (proje == null)
                return null;

            return MapToDetayViewModel(proje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje detayı getirilirken hata: {Id}", id);
            throw;
        }
    }

    public async Task<ProjeDetayViewModel?> GetBySlugAsync(string slug)
    {
        try
        {
            var proje = await _context.ProjeYonetimleri
                .Include(p => p.Kategori)
                .Include(p => p.Kurucu)
                .Include(p => p.KaynakCagriBilgisi)
                .Include(p => p.EkipUyeleri).ThenInclude(e => e.Kullanici)
                .Include(p => p.Gorevler)
                .FirstOrDefaultAsync(p => p.Slug == slug && !p.SilindiMi);

            if (proje == null)
                return null;

            return MapToDetayViewModel(proje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje detayı getirilirken hata: {Slug}", slug);
            throw;
        }
    }

    public async Task<(bool Success, int? Id, string Message)> CreateAsync(ProjeOlusturViewModel model, string userId)
    {
        try
        {
            var slug = await GenerateUniqueSlugAsync(model.Baslik);

            var proje = new ProjeYonetimi
            {
                ProjeAdi = model.Baslik,
                KisaAciklama = model.KisaAciklama,
                ProjeAciklamasi = model.Aciklama,
                KategoriId = model.KategoriId,
                BaslangicTarihi = model.BaslangicTarihi,
                TamamlanmaTarihi = model.TamamlanmaTarihi,
                HerkeseAcikMi = model.HerkeseAcikMi,
                YeniUyeKabuluAcikMi = model.YeniUyeKabuluAcikMi,
                KaynakCagriBilgisiId = model.KaynakCagriBilgisiId,
                Slug = slug,
                Durum = ProjeDurumu.FikirAsamasi,
                IlerlemeYuzdesi = 0,
                YayindaMi = true,
                KurucuKullaniciId = userId,
                OlusturanKullaniciId = userId,
                OlusturulmaTarihi = DateTime.UtcNow
            };

            _context.ProjeYonetimleri.Add(proje);
            await _context.SaveChangesAsync();

            // Kurucu otomatik ekip üyesi olsun
            var ekipUyesi = new ProjeEkipUyesi
            {
                ProjeYonetimId = proje.Id,
                KullaniciId = userId,
                Rol = ProjeRol.Kurucu,
                KatilmaTarihi = DateTime.UtcNow,
                OlusturulmaTarihi = DateTime.UtcNow
            };
            _context.ProjeEkipUyeleri.Add(ekipUyesi);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Yeni proje oluşturuldu: {Id} - {Baslik}", proje.Id, proje.ProjeAdi);

            return (true, proje.Id, "Proje başarıyla oluşturuldu");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje oluşturulurken hata");
            return (false, null, "Proje oluşturulurken bir hata oluştu");
        }
    }

    public async Task<(bool Success, string Message)> UpdateAsync(int id, ProjeGuncelleViewModel model, string userId)
    {
        try
        {
            var proje = await _context.ProjeYonetimleri.FindAsync(id);
            if (proje == null || proje.SilindiMi)
                return (false, "Proje bulunamadı");

            // Yetki kontrolü
            if (!await CanUserEditAsync(id, userId))
                return (false, "Bu projeyi düzenleme yetkiniz yok");

            // Slug güncelleme (başlık değiştiyse)
            if (proje.ProjeAdi != model.Baslik)
                proje.Slug = await GenerateUniqueSlugAsync(model.Baslik);

            proje.ProjeAdi = model.Baslik;
            proje.KisaAciklama = model.KisaAciklama;
            proje.ProjeAciklamasi = model.Aciklama;
            proje.KategoriId = model.KategoriId;
            proje.BaslangicTarihi = model.BaslangicTarihi;
            proje.TamamlanmaTarihi = model.TamamlanmaTarihi;
            proje.IlerlemeYuzdesi = model.IlerlemeYuzdesi;
            proje.Durum = model.Durum;
            proje.HerkeseAcikMi = model.HerkeseAcikMi;
            proje.YeniUyeKabuluAcikMi = model.YeniUyeKabuluAcikMi;
            proje.GuncelleyenKullaniciId = userId;
            proje.GuncellenmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Proje güncellendi: {Id} - {Baslik}", proje.Id, proje.ProjeAdi);

            return (true, "Proje başarıyla güncellendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje güncellenirken hata: {Id}", id);
            return (false, "Proje güncellenirken bir hata oluştu");
        }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id, string userId)
    {
        try
        {
            var proje = await _context.ProjeYonetimleri.FindAsync(id);
            if (proje == null || proje.SilindiMi)
                return (false, "Proje bulunamadı");

            // Yetki kontrolü
            if (!await CanUserDeleteAsync(id, userId))
                return (false, "Bu projeyi silme yetkiniz yok");

            // Soft delete
            proje.SilindiMi = true;
            proje.SilenKullaniciId = userId;
            proje.SilinmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Proje silindi (soft): {Id} - {Baslik}", proje.Id, proje.ProjeAdi);

            return (true, "Proje başarıyla silindi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje silinirken hata: {Id}", id);
            return (false, "Proje silinirken bir hata oluştu");
        }
    }

    #endregion

    #region Yetkilendirme Kontrolleri

    public async Task<bool> CanUserEditAsync(int projeId, string userId)
    {
        var ekipUyesi = await _context.ProjeEkipUyeleri
            .FirstOrDefaultAsync(e => e.ProjeYonetimId == projeId && e.KullaniciId == userId);

        return ekipUyesi != null && (ekipUyesi.Rol == ProjeRol.Kurucu || ekipUyesi.Rol == ProjeRol.Yonetici);
    }

    public async Task<bool> CanUserDeleteAsync(int projeId, string userId)
    {
        var ekipUyesi = await _context.ProjeEkipUyeleri
            .FirstOrDefaultAsync(e => e.ProjeYonetimId == projeId && e.KullaniciId == userId);

        return ekipUyesi != null && ekipUyesi.Rol == ProjeRol.Kurucu;
    }

    public async Task<bool> IsUserFounderAsync(int projeId, string userId)
    {
        var proje = await _context.ProjeYonetimleri.FindAsync(projeId);
        return proje != null && proje.KurucuKullaniciId == userId;
    }

    #endregion

    #region Ekip Yönetimi

    public async Task<(bool Success, string Message)> AddTeamMemberAsync(int projeId, string uyeUserId, ProjeRol rol, string userId)
    {
        try
        {
            if (!await CanUserEditAsync(projeId, userId))
                return (false, "Ekip üyesi ekleme yetkiniz yok");

            var mevcutUye = await _context.ProjeEkipUyeleri
                .FirstOrDefaultAsync(e => e.ProjeYonetimId == projeId && e.KullaniciId == uyeUserId);

            if (mevcutUye != null)
                return (false, "Bu kullanıcı zaten ekip üyesi");

            if (rol == ProjeRol.Kurucu)
                return (false, "Kurucu rolü değiştirilemez");

            var ekipUyesi = new ProjeEkipUyesi
            {
                ProjeYonetimId = projeId,
                KullaniciId = uyeUserId,
                Rol = rol,
                KatilmaTarihi = DateTime.UtcNow,
                OlusturulmaTarihi = DateTime.UtcNow
            };

            _context.ProjeEkipUyeleri.Add(ekipUyesi);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Projeye ekip üyesi eklendi: {ProjeId} - Kullanıcı: {UserId} - Rol: {Rol}",
                projeId, uyeUserId, rol);

            return (true, "Ekip üyesi başarıyla eklendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ekip üyesi eklenirken hata: {ProjeId}", projeId);
            return (false, "Ekip üyesi eklenirken bir hata oluştu");
        }
    }

    public async Task<(bool Success, string Message)> RemoveTeamMemberAsync(int projeId, string uyeUserId, string userId)
    {
        try
        {
            if (!await CanUserEditAsync(projeId, userId))
                return (false, "Ekip üyesi çıkarma yetkiniz yok");

            var ekipUyesi = await _context.ProjeEkipUyeleri
                .FirstOrDefaultAsync(e => e.ProjeYonetimId == projeId && e.KullaniciId == uyeUserId);

            if (ekipUyesi == null)
                return (false, "Ekip üyesi bulunamadı");

            if (ekipUyesi.Rol == ProjeRol.Kurucu)
                return (false, "Proje kurucusu çıkarılamaz");

            _context.ProjeEkipUyeleri.Remove(ekipUyesi);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Projeden ekip üyesi çıkarıldı: {ProjeId} - Kullanıcı: {UserId}", projeId, uyeUserId);

            return (true, "Ekip üyesi projeden çıkarıldı");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ekip üyesi çıkarılırken hata: {ProjeId}", projeId);
            return (false, "Ekip üyesi çıkarılırken bir hata oluştu");
        }
    }

    public async Task<(bool Success, string Message)> ChangeTeamMemberRoleAsync(int projeId, string uyeUserId, ProjeRol yeniRol, string userId)
    {
        try
        {
            if (!await CanUserEditAsync(projeId, userId))
                return (false, "Rol değiştirme yetkiniz yok");

            var ekipUyesi = await _context.ProjeEkipUyeleri
                .FirstOrDefaultAsync(e => e.ProjeYonetimId == projeId && e.KullaniciId == uyeUserId);

            if (ekipUyesi == null)
                return (false, "Ekip üyesi bulunamadı");

            if (ekipUyesi.Rol == ProjeRol.Kurucu || yeniRol == ProjeRol.Kurucu)
                return (false, "Kurucu rolü değiştirilemez");

            ekipUyesi.Rol = yeniRol;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ekip üyesi rolü değiştirildi: {ProjeId} - Kullanıcı: {UserId} - Yeni Rol: {Rol}",
                projeId, uyeUserId, yeniRol);

            return (true, "Ekip üyesi rolü güncellendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ekip üyesi rolü değiştirilirken hata: {ProjeId}", projeId);
            return (false, "Rol değiştirilirken bir hata oluştu");
        }
    }

    public async Task<List<EkipUyesiViewModel>> GetTeamMembersAsync(int projeId)
    {
        try
        {
            return await _context.ProjeEkipUyeleri
                .Where(e => e.ProjeYonetimId == projeId)
                .Include(e => e.Kullanici)
                .OrderBy(e => e.Rol)
                .ThenBy(e => e.KatilmaTarihi)
                .Select(e => new EkipUyesiViewModel
                {
                    Id = e.Id,
                    UserId = e.KullaniciId,
                    AdSoyad = e.Kullanici.Ad + " " + e.Kullanici.Soyad,
                    ProfilFotoUrl = e.Kullanici.ProfilFotografiUrl,
                    Rol = e.Rol,
                    RolText = e.Rol.ToString(),
                    KatilmaTarihi = e.KatilmaTarihi
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ekip üyeleri getirilirken hata: {ProjeId}", projeId);
            return new List<EkipUyesiViewModel>();
        }
    }

    #endregion

    #region İstatistikler

    public async Task<int> GetUserProjectCountAsync(string userId)
    {
        return await _context.ProjeYonetimleri
            .CountAsync(p => p.KurucuKullaniciId == userId && !p.SilindiMi);
    }

    public async Task IncrementViewCountAsync(int projeId)
    {
        try
        {
            var proje = await _context.ProjeYonetimleri.FindAsync(projeId);
            if (proje != null && !proje.SilindiMi)
            {
                proje.GoruntulenmeSayisi++;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Görüntülenme sayısı artırılırken hata: {ProjeId}", projeId);
        }
    }

    public async Task<List<ProjeListViewModel>> GetPopularProjectsAsync(int adet = 5)
    {
        try
        {
            return await _context.ProjeYonetimleri
                .Where(p => !p.SilindiMi && p.YayindaMi && p.HerkeseAcikMi)
                .OrderByDescending(p => p.BegeniSayisi)
                .ThenByDescending(p => p.GoruntulenmeSayisi)
                .Take(adet)
                .Select(p => new ProjeListViewModel
                {
                    Id = p.Id,
                    Baslik = p.ProjeAdi,
                    KapakResmiUrl = p.KapakGorseliUrl,
                    Durum = p.Durum,
                    DurumText = p.Durum.ToString(),
                    BegeniSayisi = p.BegeniSayisi,
                    GoruntulenmeSayisi = p.GoruntulenmeSayisi,
                    Slug = p.Slug
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Popüler projeler getirilirken hata");
            return new List<ProjeListViewModel>();
        }
    }

    public async Task<List<ProjeListViewModel>> GetRecentProjectsAsync(int adet = 5)
    {
        try
        {
            return await _context.ProjeYonetimleri
                .Where(p => !p.SilindiMi && p.YayindaMi && p.HerkeseAcikMi)
                .OrderByDescending(p => p.OlusturulmaTarihi)
                .Take(adet)
                .Select(p => new ProjeListViewModel
                {
                    Id = p.Id,
                    Baslik = p.ProjeAdi,
                    KapakResmiUrl = p.KapakGorseliUrl,
                    Durum = p.Durum,
                    DurumText = p.Durum.ToString(),
                    OlusturmaTarihi = p.OlusturulmaTarihi,
                    Slug = p.Slug
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Son projeler getirilirken hata");
            return new List<ProjeListViewModel>();
        }
    }

    #endregion

    #region Yardımcı Metotlar

    public async Task<string> GenerateUniqueSlugAsync(string baslik)
    {
        var slug = CreateSlug(baslik);
        var originalSlug = slug;
        var counter = 1;

        while (await _context.ProjeYonetimleri.AnyAsync(p => p.Slug == slug))
        {
            slug = $"{originalSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    public async Task<(bool Success, string Message)> UpdateStatusAsync(int projeId, ProjeDurumu yeniDurum, string userId)
    {
        try
        {
            var proje = await _context.ProjeYonetimleri.FindAsync(projeId);
            if (proje == null || proje.SilindiMi)
                return (false, "Proje bulunamadı");

            if (!await CanUserEditAsync(projeId, userId))
                return (false, "Bu projenin durumunu güncelleme yetkiniz yok");

            proje.Durum = yeniDurum;
            proje.GuncelleyenKullaniciId = userId;
            proje.GuncellenmeTarihi = DateTime.UtcNow;

            if (yeniDurum == ProjeDurumu.Tamamlandi && proje.TamamlanmaTarihi == null)
                proje.TamamlanmaTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (true, "Proje durumu güncellendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje durumu güncellenirken hata: {ProjeId}", projeId);
            return (false, "Durum güncellenirken bir hata oluştu");
        }
    }

    #endregion

    #region Private Helper Methods

    private static ProjeDetayViewModel MapToDetayViewModel(ProjeYonetimi proje)
    {
        return new ProjeDetayViewModel
        {
            Id = proje.Id,
            Baslik = proje.ProjeAdi,
            KapakResmiUrl = proje.KapakGorseliUrl,
            Aciklama = proje.ProjeAciklamasi,
            Durum = proje.Durum,
            DurumText = proje.Durum.ToString(),
            KategoriId = proje.KategoriId,
            KategoriAdi = proje.Kategori?.Ad,
            KurucuUserId = proje.KurucuKullaniciId,
            KurucuAdSoyad = proje.Kurucu.Ad + " " + proje.Kurucu.Soyad,
            KurucuProfilFotoUrl = proje.Kurucu.ProfilFotografiUrl,
            BaslangicTarihi = proje.BaslangicTarihi,
            TamamlanmaTarihi = proje.TamamlanmaTarihi,
            IlerlemeYuzdesi = proje.IlerlemeYuzdesi,
            HerkeseAcikMi = proje.HerkeseAcikMi,
            YeniUyeKabuluAcikMi = proje.YeniUyeKabuluAcikMi,
            KaynakCagriBilgisiId = proje.KaynakCagriBilgisiId,
            KaynakCagriBaslik = proje.KaynakCagriBilgisi?.Baslik,
            GoruntulenmeSayisi = proje.GoruntulenmeSayisi,
            BegeniSayisi = proje.BegeniSayisi,
            YorumSayisi = proje.YorumSayisi,
            OlusturmaTarihi = proje.OlusturulmaTarihi,
            Slug = proje.Slug,
            EkipUyeleri = proje.EkipUyeleri.Select(e => new EkipUyesiViewModel
            {
                Id = e.Id,
                UserId = e.KullaniciId,
                AdSoyad = e.Kullanici.Ad + " " + e.Kullanici.Soyad,
                ProfilFotoUrl = e.Kullanici.ProfilFotografiUrl,
                Rol = e.Rol,
                RolText = e.Rol.ToString(),
                KatilmaTarihi = e.KatilmaTarihi
            }).ToList(),
            Gorevler = proje.Gorevler
                .Where(g => !g.SilindiMi)
                .OrderBy(g => g.Durum)
                .ThenByDescending(g => g.Oncelik)
                .Take(10)
                .Select(g => new GorevOzetViewModel
                {
                    Id = g.Id,
                    Baslik = g.Baslik,
                    Durum = g.Durum,
                    DurumText = g.Durum.ToString(),
                    Oncelik = g.Oncelik,
                    OncelikText = g.Oncelik.ToString(),
                    AtananKisiAdSoyad = g.AtananKullanici != null ? g.AtananKullanici.Ad + " " + g.AtananKullanici.Soyad : null,
                    BitisTarihi = g.BitisTarihi
                }).ToList()
        };
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

    #endregion
}

/*
 * SERVIS AÇIKLAMASI:
 * ==================
 * ProjeYonetimService - Kullanıcı projelerinin yönetimi
 * 
 * Özellikler:
 * - CRUD işlemleri
 * - Ekip yönetimi (Add, Remove, Change Role)
 * - Yetkilendirme kontrolleri
 * - İstatistikler
 * - SEO-friendly slug
 * - Soft delete
 * 
 * Mimari:
 * - Eventual consistency
 * - Async/Await
 * - Try/Catch + Logging
 * - Clean code principles
 * 
 * DÜZELTİLEN HATALAR (2026-01-27):
 * - BaseEntity property isimleri
 * - ProjeYonetimi property isimleri (ProjeAdi, ProjeAciklamasi, KurucuKullaniciId, Kurucu)
 * - ProjeEkipUyesi ProjeYonetimId
 * - Dictionary<char, char> türü
 * - Navigation property isimleri
 * - Nullable DateTime kontrolleri
 */