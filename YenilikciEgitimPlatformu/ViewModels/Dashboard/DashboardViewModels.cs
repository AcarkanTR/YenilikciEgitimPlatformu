using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YenilikciEgitimPlatformu.ViewModels.Dashboard;

#region Admin Dashboard

/// <summary>
/// Admin Dashboard ana ViewModel - Modern UI için Güncellendi
/// </summary>
public class AdminDashboardViewModel
{
    // --- Temel Sayaçlar (Kartlar) ---
    public int ToplamKullaniciSayisi { get; set; }
    public int AktifKullaniciSayisi { get; set; }
    public int BuAyYeniKullanici { get; set; } // Trend göstergesi için

    public int ToplamOkulSayisi { get; set; } // Yeni eklendi
    public int AktifOkulSayisi { get; set; }  // Yeni eklendi

    public int ToplamProjeSayisi { get; set; }
    public int AktifProjeSayisi { get; set; } // Yayında olanlar
    public int BekleyenProjeSayisi { get; set; } // Onay/Yayın bekleyenler

    public int ToplamCagriSayisi { get; set; } // Sistem 1 (Çağrılar)
    public int BekleyenOnaySayisi { get; set; } // Toplam bekleyen işler

    // --- Grafik Verileri (Chart.js için Hazır Listeler) ---
    public List<string> GrafikAylar { get; set; } = new();
    public List<int> GrafikKullaniciVerileri { get; set; } = new();

    // --- Detaylı Listeler ---
    public List<SonAktiviteViewModel> SonAktiviteler { get; set; } = new(); // AuditLog'dan
    public List<DurumDagilimViewModel> ProjeDurumDagilimi { get; set; } = new();

    // Eski yapıyı bozmamak için tutulanlar (Opsiyonel kullanılabilir)
    public List<AktifKullaniciViewModel> EnAktifKullanicilar { get; set; } = new();
    public List<AylikIstatistikViewModel> AylikKayitlar { get; set; } = new();
}

/// <summary>
/// Dashboard üzerindeki "Son Aktiviteler" tablosu için
/// </summary>
public class SonAktiviteViewModel
{
    public string KullaniciAdi { get; set; } = string.Empty;
    public string IslemTuru { get; set; } = string.Empty; // Create, Update, Delete
    public string EntityTuru { get; set; } = string.Empty; // Proje, Okul...
    public string Ozet { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
    public string? ProfilResmiUrl { get; set; }
    public string BasHarf => !string.IsNullOrEmpty(KullaniciAdi) ? KullaniciAdi[0].ToString().ToUpper() : "?";
}

/// <summary>
/// Bekleyen onaylar özet ViewModel (Eski yapıdan referansla)
/// </summary>
public class BekleyenOnaylarViewModel
{
    public int BekleyenCagriSayisi { get; set; }
    public int BekleyenYorumSayisi { get; set; }
    public int BekleyenGonderiSayisi { get; set; }
    public int ToplamBekleyen => BekleyenCagriSayisi + BekleyenYorumSayisi + BekleyenGonderiSayisi;
}

#endregion

#region User Dashboard (Mevcut Yapı Korundu)

public class UserDashboardViewModel
{
    public string KullaniciAdi { get; set; } = string.Empty;
    public string? ProfilFotoUrl { get; set; }

    public int ToplamProjeSayisi { get; set; }
    public int AktifProjeSayisi { get; set; }
    public int TamamlananProjeSayisi { get; set; }
    public int ToplamGonderiSayisi { get; set; }
    public int ToplamYorumSayisi { get; set; }
    public int ToplamBegeniSayisi { get; set; }
    public int KazanilanRozetSayisi { get; set; }
    public int SeviPuani { get; set; }

    public List<ProjeOzetViewModel> SonProjeler { get; set; } = new();
    public List<AylikIstatistikViewModel> AylikAktivite { get; set; } = new();
    public List<AktiviteViewModel> SonAktiviteler { get; set; } = new();
    public List<DurumDagilimViewModel> ProjeDurumDagilimi { get; set; } = new();
}

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

public class AylikIstatistikViewModel
{
    [Display(Name = "Ay")]
    public string Ay { get; set; } = string.Empty;

    [Display(Name = "Değer")]
    public int Deger { get; set; }
}

public class AktifKullaniciViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string AdSoyad { get; set; } = string.Empty;
    public string? ProfilFotoUrl { get; set; }
    public int AktivitePuani { get; set; }
    public int ProjeSayisi { get; set; }
    public int GonderiSayisi { get; set; }
}

public class DurumDagilimViewModel
{
    public string Durum { get; set; } = string.Empty;
    public int Sayi { get; set; }
    public string Renk { get; set; } = string.Empty;
}

public class KategoriDagilimViewModel
{
    public string Kategori { get; set; } = string.Empty;
    public int Sayi { get; set; }
}

public class AktiviteViewModel
{
    public string Tip { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public DateTime Tarih { get; set; }
    public string Ikon { get; set; } = "fa-circle";
}

#endregion