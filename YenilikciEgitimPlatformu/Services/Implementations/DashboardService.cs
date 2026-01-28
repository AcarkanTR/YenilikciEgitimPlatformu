using Microsoft.EntityFrameworkCore;
using YenilikciEgitimPlatformu.Data;
using YenilikciEgitimPlatformu.Models;
using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.ViewModels.Dashboard;
using System.Globalization;

namespace YenilikciEgitimPlatformu.Services.Implementations;

/// <summary>
/// Dashboard servisi implementasyonu
/// Admin ve User dashboard'ları için istatistik ve özet bilgiler sağlar
/// 
/// [Mimari Notu]
/// Bu servis eventual consistency kabul eder.
/// DB atomic, cache ve SignalR side-effect olarak ele alınır.
/// </summary>
public class DashboardService : IDashboardService
{
    #region Fields & Constructor

    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        ApplicationDbContext context,
        ILogger<DashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #endregion

    #region Admin Dashboard

    public async Task<AdminDashboardViewModel> GetAdminDashboardDataAsync()
    {
        try
        {
            var model = new AdminDashboardViewModel();
            var bugun = DateTime.Now;
            var ayBaslangic = new DateTime(bugun.Year, bugun.Month, 1);

            // Kullanıcı istatistikleri
            model.ToplamKullaniciSayisi = await _context.Users.CountAsync();
            model.AktifKullaniciSayisi = await _context.Users.CountAsync(u => !u.SilindiMi);
            model.BuAyYeniKullanici = await _context.Users
                .CountAsync(u => u.KayitTarihi >= ayBaslangic);

            // Çağrı istatistikleri
            model.ToplamCagriSayisi = await _context.CagriBilgileri.CountAsync();
            model.AktifCagriSayisi = await _context.CagriBilgileri
                .CountAsync(c => c.YayindaMi && !c.SilindiMi);

            // Proje istatistikleri
            model.ToplamProjeSayisi = await _context.ProjeYonetimleri.CountAsync();
            model.AktifProjeSayisi = await _context.ProjeYonetimleri
                .CountAsync(p => p.YayindaMi && !p.SilindiMi);
            model.BuAyYeniProje = await _context.ProjeYonetimleri
                .CountAsync(p => p.OlusturulmaTarihi >= ayBaslangic);

            // Listeler
            model.EnAktifKullanicilar = await GetEnAktifKullanicilarAsync(10);
            model.AylikKayitlar = await GetAylikKullaniciKayitlariAsync(6);
            model.ProjeDurumDagilimi = await GetProjeDurumDagilimAsync();
            model.BekleyenOnaylar = await GetBekleyenOnaylarAsync();

            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Admin dashboard verileri getirilirken hata oluştu");
            throw;
        }
    }

    public async Task<List<AylikIstatistikViewModel>> GetAylikKullaniciKayitlariAsync(int sonKacAy = 6)
    {
        try
        {
            var baslangicTarihi = DateTime.Now.AddMonths(-sonKacAy);
            var kultur = new CultureInfo("tr-TR");

            var kayitlar = await _context.Users
                .Where(u => u.KayitTarihi >= baslangicTarihi)
                .GroupBy(u => new { u.KayitTarihi.Year, u.KayitTarihi.Month })
                .Select(g => new
                {
                    Yil = g.Key.Year,
                    Ay = g.Key.Month,
                    Sayi = g.Count()
                })
                .OrderBy(x => x.Yil).ThenBy(x => x.Ay)
                .ToListAsync();

            return kayitlar.Select(k => new AylikIstatistikViewModel
            {
                Ay = new DateTime(k.Yil, k.Ay, 1).ToString("MMMM yyyy", kultur),
                Deger = k.Sayi
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aylık kullanıcı kayıtları getirilirken hata oluştu");
            return new List<AylikIstatistikViewModel>();
        }
    }

    public async Task<List<AktifKullaniciViewModel>> GetEnAktifKullanicilarAsync(int adet = 10)
    {
        try
        {
            var kullanicilar = await _context.Users
                .Where(u => !u.SilindiMi)
                .Select(u => new AktifKullaniciViewModel
                {
                    UserId = u.Id,
                    AdSoyad = u.Ad + " " + u.Soyad,
                    ProfilFotoUrl = u.ProfilFotografiUrl,
                    ProjeSayisi = u.KurulanProjeler.Count(p => !p.SilindiMi),
                    GonderiSayisi = u.Gonderiler.Count(g => !g.SilindiMi),
                    AktivitePuani = u.KurulanProjeler.Count(p => !p.SilindiMi) * 10 +
                                    u.Gonderiler.Count(g => !g.SilindiMi) * 5 +
                                    u.Yorumlar.Count(y => !y.SilindiMi) * 2
                })
                .OrderByDescending(u => u.AktivitePuani)
                .Take(adet)
                .ToListAsync();

            return kullanicilar;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "En aktif kullanıcılar getirilirken hata oluştu");
            return new List<AktifKullaniciViewModel>();
        }
    }

    public async Task<BekleyenOnaylarViewModel> GetBekleyenOnaylarAsync()
    {
        try
        {
            return new BekleyenOnaylarViewModel
            {
                BekleyenCagriSayisi = await _context.CagriBilgileri
                    .CountAsync(c => !c.YayindaMi && !c.SilindiMi),
                BekleyenYorumSayisi = await _context.Yorumlar
                    .CountAsync(y => !y.OnaylandiMi && !y.SilindiMi)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bekleyen onaylar getirilirken hata oluştu");
            return new BekleyenOnaylarViewModel();
        }
    }

    #endregion

    #region User Dashboard

    public async Task<UserDashboardViewModel> GetUserDashboardDataAsync(string userId)
    {
        try
        {
            var kullanici = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (kullanici == null)
                throw new Exception("Kullanıcı bulunamadı");

            var model = new UserDashboardViewModel
            {
                KullaniciAdi = kullanici.Ad + " " + kullanici.Soyad,
                ProfilFotoUrl = kullanici.ProfilFotografiUrl
            };

            // Proje istatistikleri
            var projeler = await _context.ProjeYonetimleri
                .Where(p => p.KurucuKullaniciId == userId && !p.SilindiMi)
                .ToListAsync();

            model.ToplamProjeSayisi = projeler.Count;
            model.AktifProjeSayisi = projeler.Count(p => p.YayindaMi);
            model.TamamlananProjeSayisi = projeler.Count(p => p.Durum == ProjeDurumu.Tamamlandi);

            // Sosyal istatistikler
            model.ToplamGonderiSayisi = await _context.Gonderiler
                .CountAsync(g => g.OlusturanKullaniciId == userId && !g.SilindiMi);
            model.ToplamYorumSayisi = await _context.Yorumlar
                .CountAsync(y => y.OlusturanKullaniciId == userId && !y.SilindiMi);
            model.ToplamBegeniSayisi = await _context.Begeniler
                .CountAsync(b => b.KullaniciId == userId);

            // Oyunlaştırma
            model.KazanilanRozetSayisi = await _context.KullaniciRozetleri
                .CountAsync(kr => kr.KullaniciId == userId);
            model.SeviPuani = kullanici.DeneyimPuani;

            // Listeler
            model.SonProjeler = await GetKullaniciSonProjelerAsync(userId, 5);
            model.AylikAktivite = await GetKullaniciAylikAktiviteAsync(userId, 6);
            model.SonAktiviteler = await GetKullaniciSonAktivitelerAsync(userId, 10);
            model.ProjeDurumDagilimi = await GetProjeDurumDagilimAsync(userId);

            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User dashboard verileri getirilirken hata: {UserId}", userId);
            throw;
        }
    }

    public async Task<List<AylikIstatistikViewModel>> GetKullaniciAylikAktiviteAsync(string userId, int sonKacAy = 6)
    {
        try
        {
            var baslangicTarihi = DateTime.Now.AddMonths(-sonKacAy);
            var kultur = new CultureInfo("tr-TR");

            // Gönderi + Yorum + Proje oluşturma
            var gonderiler = await _context.Gonderiler
                .Where(g => g.OlusturanKullaniciId == userId && g.OlusturulmaTarihi >= baslangicTarihi)
                .GroupBy(g => new { g.OlusturulmaTarihi.Year, g.OlusturulmaTarihi.Month })
                .Select(g => new { Yil = g.Key.Year, Ay = g.Key.Month, Sayi = g.Count() })
                .ToListAsync();

            return gonderiler.Select(g => new AylikIstatistikViewModel
            {
                Ay = new DateTime(g.Yil, g.Ay, 1).ToString("MMMM yyyy", kultur),
                Deger = g.Sayi
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı aylık aktivite getirilirken hata: {UserId}", userId);
            return new List<AylikIstatistikViewModel>();
        }
    }

    public async Task<List<AktiviteViewModel>> GetKullaniciSonAktivitelerAsync(string userId, int adet = 10)
    {
        try
        {
            var aktiviteler = new List<AktiviteViewModel>();

            // Son projeler
            var projeler = await _context.ProjeYonetimleri
                .Where(p => p.KurucuKullaniciId == userId && !p.SilindiMi)
                .OrderByDescending(p => p.OlusturulmaTarihi)
                .Take(3)
                .Select(p => new AktiviteViewModel
                {
                    Tip = "Proje Oluşturdu",
                    Aciklama = p.ProjeAciklamasi,
                    LinkUrl = $"/Projeler/Detay/{p.Slug}",
                    Tarih = p.OlusturulmaTarihi,
                    Ikon = "fa-rocket"
                })
                .ToListAsync();

            aktiviteler.AddRange(projeler);

            // Son gönderiler
            var gonderiler = await _context.Gonderiler
                .Where(g => g.OlusturanKullaniciId == userId && !g.SilindiMi)
                .OrderByDescending(g => g.OlusturulmaTarihi)
                .Take(3)
                .Select(g => new AktiviteViewModel
                {
                    Tip = "Gönderi Paylaştı",
                    Aciklama = g.Icerik.Substring(0, Math.Min(50, g.Icerik.Length)) + "...",
                    LinkUrl = $"/Sosyal/Detay/{g.Id}",
                    Tarih = g.OlusturulmaTarihi,
                    Ikon = "fa-comment"
                })
                .ToListAsync();

            aktiviteler.AddRange(gonderiler);

            return aktiviteler
                .OrderByDescending(a => a.Tarih)
                .Take(adet)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı son aktiviteleri getirilirken hata: {UserId}", userId);
            return new List<AktiviteViewModel>();
        }
    }

    #endregion

    #region Ortak Metotlar

    public async Task<List<DurumDagilimViewModel>> GetProjeDurumDagilimAsync(string? userId = null)
    {
        try
        {
            var query = _context.ProjeYonetimleri.Where(p => !p.SilindiMi);

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(p => p.KurucuKullaniciId == userId);

            var dagilim = await query
                .GroupBy(p => p.Durum)
                .Select(g => new DurumDagilimViewModel
                {
                    Durum = g.Key.ToString(),
                    Sayi = g.Count(),
                    Renk = GetDurumRengi(g.Key)
                })
                .ToListAsync();

            return dagilim;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje durum dağılımı getirilirken hata");
            return new List<DurumDagilimViewModel>();
        }
    }

    public async Task<List<KategoriDagilimViewModel>> GetKategoriDagilimAsync()
    {
        try
        {
            var dagilim = await _context.ProjeYonetimleri
                .Where(p => !p.SilindiMi && p.KategoriId != null)
                .GroupBy(p => p.Kategori!.Ad)
                .Select(g => new KategoriDagilimViewModel
                {
                    Kategori = g.Key,
                    Sayi = g.Count()
                })
                .OrderByDescending(k => k.Sayi)
                .Take(10)
                .ToListAsync();

            return dagilim;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kategori dağılımı getirilirken hata");
            return new List<KategoriDagilimViewModel>();
        }
    }

    #endregion

    #region Private Helper Methods

    private async Task<List<ProjeOzetViewModel>> GetKullaniciSonProjelerAsync(string userId, int adet)
    {
        return await _context.ProjeYonetimleri
            .Where(p => p.KurucuKullaniciId == userId && !p.SilindiMi)
            .OrderByDescending(p => p.OlusturulmaTarihi)
            .Take(adet)
            .Select(p => new ProjeOzetViewModel
            {
                Id = p.Id,
                Baslik = p.ProjeAciklamasi,
                KapakResmiUrl = p.KapakGorseliUrl,
                Durum = p.Durum.ToString(),
                IlerlemeYuzdesi = p.IlerlemeYuzdesi,
                OlusturmaTarihi = p.OlusturulmaTarihi
            })
            .ToListAsync();
    }

    private static string GetDurumRengi(ProjeDurumu durum)
    {
        return durum switch
        {
            ProjeDurumu.FikirAsamasi => "#3b82f6", // Blue
            ProjeDurumu.Planlama => "#8b5cf6", // Purple
            ProjeDurumu.DevamEdiyor => "#f59e0b", // Amber
            ProjeDurumu.Askida => "#6b7280", // Gray
            ProjeDurumu.Tamamlandi => "#10b981", // Green
            ProjeDurumu.IptalEdildi => "#ef4444", // Red
            _ => "#6b7280"
        };
    }

    #endregion
}