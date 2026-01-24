using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YenilikciEgitimPlatformu.Data;
using YenilikciEgitimPlatformu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace YenilikciEgitimPlatformu.Pages;

/*
 * ════════════════════════════════════════════════════════════════════════════
 * IndexModel - Ana Sayfa PageModel (Enhanced v2.0)
 * ════════════════════════════════════════════════════════════════════════════
 * Mimar Notu:
 * - Clean Architecture için Servis katmanına geçiş öncesi hibrit yapı.
 * - Tüm listeler için 'AsNoTracking' performans iyileştirmesi.
 * - Null-Safety ve Exception Handling eklendi.
 */

public class IndexModel : PageModel
{
    #region Fields

    private readonly ApplicationDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    #endregion

    #region Constructor

    public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    #endregion

    #region Properties

    // --- Hero Stats (İstatistikler) ---
    public int ToplamCagri { get; set; }
    public int ToplamProje { get; set; }
    public int ToplamKullanici { get; set; }
    public int ToplamOkul { get; set; }

    // --- Vitrin Listeleri ---
    public List<CagriViewModel> OneCikanCagrilar { get; set; } = new();
    public List<ProjeViewModel> OneCikanProjeler { get; set; } = new();

    // --- Yan Bölümler ---
    public List<CagriViewModel> SonYarismalar { get; set; } = new();
    public List<IcerikViewModel> SonHaberler { get; set; } = new();

    #endregion

    #region ViewModels

    public class CagriViewModel
    {
        public int Id { get; set; }
        public string Baslik { get; set; }
        public string KisaAciklama { get; set; }
        public string KurumAdi { get; set; }
        public string CagriTuru { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int KalanGun => BitisTarihi.HasValue ? (BitisTarihi.Value - DateTime.UtcNow).Days : 0;
    }

    public class ProjeViewModel
    {
        public int Id { get; set; }
        public string ProjeAdi { get; set; }
        public string KisaAciklama { get; set; }
        public string KapakGorseliUrl { get; set; }
        public string Kategori { get; set; }
        public int BegeniSayisi { get; set; }
        public string OkulAdi { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
    }

    public class IcerikViewModel
    {
        public int Id { get; set; }
        public string Baslik { get; set; }
        public string Ozet { get; set; }
        public string ResimUrl { get; set; }
        public DateTime Tarih { get; set; }
        public string Tur { get; set; } // Haber, Duyuru
    }

    #endregion

    #region Handlers

    public async Task OnGetAsync()
    {
        try
        {
            // [Mimar Notu]: Veritabanı sorguları optimize edildi.

            // 1. İstatistikler
            ToplamCagri = await _context.CagriBilgileri.CountAsync(c => c.YayindaMi && c.CagriBitisTarihi > DateTime.UtcNow);
            ToplamProje = await _context.ProjeYonetimleri.CountAsync(p => p.YayindaMi);
            ToplamKullanici = await _context.Users.CountAsync();
            ToplamOkul = await _context.Okullar.CountAsync();

            // 2. Öne Çıkan Çağrılar
            OneCikanCagrilar = await _context.CagriBilgileri
                .AsNoTracking()
                .Where(c => c.YayindaMi && c.CagriBitisTarihi > DateTime.UtcNow)
                .OrderByDescending(c => c.OlusturulmaTarihi)
                .Take(3)
                .Select(c => new CagriViewModel
                {
                    Id = c.Id,
                    Baslik = c.Baslik,
                    KisaAciklama = c.KisaAciklama,
                    KurumAdi = c.KurumAdi,
                    CagriTuru = c.CagriTuru.ToString(),
                    BitisTarihi = c.CagriBitisTarihi
                })
                .ToListAsync();

            // 3. Son Projeler (Vitrin)
            OneCikanProjeler = await _context.ProjeYonetimleri
                .AsNoTracking()
                .Where(p => p.YayindaMi)
                .OrderByDescending(p => p.OlusturulmaTarihi)
                .Take(8) // Daha fazla veri çekiyoruz (Grid için)
                .Select(p => new ProjeViewModel
                {
                    Id = p.Id,
                    ProjeAdi = p.ProjeAdi,
                    KisaAciklama = p.KisaAciklama,
                    KapakGorseliUrl = !string.IsNullOrEmpty(p.KapakGorseliUrl)
                        ? p.KapakGorseliUrl
                        : "https://images.unsplash.com/photo-1531403009284-440f080d1e12?auto=format&fit=crop&q=80&w=800", // Fallback image
                    Kategori = "Genel", // Kategori implementasyonu sonrası güncellenecek
                    BegeniSayisi = 0,
                    OkulAdi = "Ankara Fen Lisesi", // Join sonrası güncellenecek
                    OlusturulmaTarihi = p.OlusturulmaTarihi
                })
                .ToListAsync();

            // 4. Son Yarışmalar (Mock/Simülasyon Verisi)
            // Gerçek DB'de: .Where(c => c.CagriTuru == CagriTuru.Yarisma)
            SonYarismalar = await _context.CagriBilgileri
                .AsNoTracking()
                .Where(c => c.YayindaMi && c.CagriBitisTarihi > DateTime.UtcNow)
                .OrderBy(c => c.CagriBitisTarihi)
                .Take(4)
                .Select(c => new CagriViewModel
                {
                    Id = c.Id,
                    Baslik = c.Baslik,
                    KisaAciklama = c.KisaAciklama,
                    KurumAdi = c.KurumAdi,
                    CagriTuru = "Yarışma",
                    BitisTarihi = c.CagriBitisTarihi
                })
                .ToListAsync();

            // 5. Haberler (Mock Data)
            SonHaberler = new List<IcerikViewModel>
            {
                new IcerikViewModel { Id=1, Baslik="YEP Platformu 81 İlde Yayında!", Ozet="Milli Eğitim Bakanlığı iş birliği ile platformumuz tüm okulların erişimine açıldı.", ResimUrl="https://images.unsplash.com/photo-1524178232363-1fb2b075b655?auto=format&fit=crop&q=80&w=300", Tarih=DateTime.Now, Tur="Haber" },
                new IcerikViewModel { Id=2, Baslik="2026 Eğitim Teknolojileri Zirvesi", Ozet="Bu yıl İstanbul'da düzenlenecek zirveye katılım rekor düzeyde.", ResimUrl="https://images.unsplash.com/photo-1544531586-fde5298cdd40?auto=format&fit=crop&q=80&w=300", Tarih=DateTime.Now.AddDays(-2), Tur="Duyuru" },
                new IcerikViewModel { Id=3, Baslik="Genç Yazılımcılar Yarışıyor", Ozet="Lise öğrencileri için düzenlenen hackathon başvuruları başladı.", ResimUrl="https://images.unsplash.com/photo-1504384308090-c54be3855833?auto=format&fit=crop&q=80&w=300", Tarih=DateTime.Now.AddDays(-5), Tur="Yarışma" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ana sayfa verileri yüklenirken kritik hata.");
        }
    }

    #endregion
}