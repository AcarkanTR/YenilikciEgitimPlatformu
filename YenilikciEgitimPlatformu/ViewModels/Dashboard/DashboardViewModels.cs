using System.ComponentModel.DataAnnotations;

namespace YenilikciEgitimPlatformu.ViewModels.Dashboard;

#region Admin Dashboard

/// <summary>
/// Admin Dashboard ana ViewModel
/// </summary>
public class AdminDashboardViewModel
{
    public int ToplamKullaniciSayisi { get; set; }
    public int AktifKullaniciSayisi { get; set; }
    public int ToplamCagriSayisi { get; set; }
    public int AktifCagriSayisi { get; set; }
    public int ToplamProjeSayisi { get; set; }
    public int AktifProjeSayisi { get; set; }
    public int BuAyYeniKullanici { get; set; }
    public int BuAyYeniProje { get; set; }

    // Listeler
    public List<AktifKullaniciViewModel> EnAktifKullanicilar { get; set; } = new();
    public List<AylikIstatistikViewModel> AylikKayitlar { get; set; } = new();
    public List<DurumDagilimViewModel> ProjeDurumDagilimi { get; set; } = new();
    public BekleyenOnaylarViewModel BekleyenOnaylar { get; set; } = new();
}

/// <summary>
/// Bekleyen onaylar özet ViewModel
/// </summary>
public class BekleyenOnaylarViewModel
{
    public int BekleyenCagriSayisi { get; set; }
    public int BekleyenYorumSayisi { get; set; }
    public int BekleyenGonderiSayisi { get; set; }
    public int ToplamBekleyen => BekleyenCagriSayisi + BekleyenYorumSayisi + BekleyenGonderiSayisi;
}

#endregion

#region User Dashboard

/// <summary>
/// User Dashboard ana ViewModel
/// </summary>
public class UserDashboardViewModel
{
    public string KullaniciAdi { get; set; } = string.Empty;
    public string? ProfilFotoUrl { get; set; }

    // İstatistikler
    public int ToplamProjeSayisi { get; set; }
    public int AktifProjeSayisi { get; set; }
    public int TamamlananProjeSayisi { get; set; }
    public int ToplamGonderiSayisi { get; set; }
    public int ToplamYorumSayisi { get; set; }
    public int ToplamBegeniSayisi { get; set; }
    public int KazanilanRozetSayisi { get; set; }
    public int SeviPuani { get; set; }

    // Listeler
    public List<ProjeOzetViewModel> SonProjeler { get; set; } = new();
    public List<AylikIstatistikViewModel> AylikAktivite { get; set; } = new();
    public List<AktiviteViewModel> SonAktiviteler { get; set; } = new();
    public List<DurumDagilimViewModel> ProjeDurumDagilimi { get; set; } = new();
}

/// <summary>
/// Proje özet bilgisi
/// </summary>
public class ProjeOzetViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? KapakResmiUrl { get; set; }
    public string Durum { get; set; } = string.Empty;
    public int IlerlemeYuzdesi { get; set; }
    public DateTime OlusturmaTarihi { get; set; }
}

#endregion

#region Ortak ViewModel'ler

/// <summary>
/// Aylık istatistik (Chart.js için)
/// </summary>
public class AylikIstatistikViewModel
{
    [Display(Name = "Ay")]
    public string Ay { get; set; } = string.Empty; // Örn: "Ocak 2026"

    [Display(Name = "Değer")]
    public int Deger { get; set; }
}

/// <summary>
/// Aktif kullanıcı bilgisi
/// </summary>
public class AktifKullaniciViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string AdSoyad { get; set; } = string.Empty;
    public string? ProfilFotoUrl { get; set; }
    public int AktivitePuani { get; set; }
    public int ProjeSayisi { get; set; }
    public int GonderiSayisi { get; set; }
}

/// <summary>
/// Durum dağılımı (Pie Chart için)
/// </summary>
public class DurumDagilimViewModel
{
    [Display(Name = "Durum")]
    public string Durum { get; set; } = string.Empty;

    [Display(Name = "Sayı")]
    public int Sayi { get; set; }

    [Display(Name = "Renk")]
    public string Renk { get; set; } = string.Empty; // Hex color
}

/// <summary>
/// Kategori dağılımı (Bar Chart için)
/// </summary>
public class KategoriDagilimViewModel
{
    [Display(Name = "Kategori")]
    public string Kategori { get; set; } = string.Empty;

    [Display(Name = "Sayı")]
    public int Sayi { get; set; }
}

/// <summary>
/// Kullanıcı aktivitesi
/// </summary>
public class AktiviteViewModel
{
    public string Tip { get; set; } = string.Empty; // "Proje Oluşturdu", "Gönderi Paylaştı", vb.
    public string Aciklama { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public DateTime Tarih { get; set; }
    public string Ikon { get; set; } = "fa-circle"; // FontAwesome icon
}

#endregion