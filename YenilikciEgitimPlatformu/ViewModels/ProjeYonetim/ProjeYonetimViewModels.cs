using System.ComponentModel.DataAnnotations;
using YenilikciEgitimPlatformu.Models;

namespace YenilikciEgitimPlatformu.ViewModels.ProjeYonetim;

#region Liste ve Filtreleme

/// <summary>
/// Proje listesi için ViewModel
/// </summary>
public class ProjeListViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? KisaAciklama { get; set; }
    public string? KapakResmiUrl { get; set; }
    public ProjeDurumu Durum { get; set; }
    public string DurumText { get; set; } = string.Empty;
    public string? KategoriAdi { get; set; }
    public string KurucuAdSoyad { get; set; } = string.Empty;
    public int EkipUyeSayisi { get; set; }
    public int IlerlemeYuzdesi { get; set; }
    public int GoruntulenmeSayisi { get; set; }
    public int BegeniSayisi { get; set; }
    public DateTime OlusturmaTarihi { get; set; }
    public string Slug { get; set; } = string.Empty;
}

/// <summary>
/// Proje filtreleme ViewModel
/// </summary>
public class ProjeFiltreleViewModel
{
    [Display(Name = "Arama")]
    public string? Arama { get; set; }

    [Display(Name = "Kategori")]
    public int? KategoriId { get; set; }

    [Display(Name = "Durum")]
    public ProjeDurumu? Durum { get; set; }

    [Display(Name = "İl")]
    public int? IlId { get; set; }

    [Display(Name = "Sadece Herkese Açık")]
    public bool SadeceHerkeseAcik { get; set; } = true;

    [Display(Name = "Sadece Kendi Projelerim")]
    public bool SadeceKendiProjelerim { get; set; } = false;

    public string? UserId { get; set; } // Filtre için

    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    // Sorting
    public string SortBy { get; set; } = "OlusturmaTarihi";
    public bool SortDescending { get; set; } = true;
}

#endregion

#region Detay

/// <summary>
/// Proje detay ViewModel
/// </summary>
public class ProjeDetayViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? KapakResmiUrl { get; set; }
    public string? Aciklama { get; set; }
    public ProjeDurumu Durum { get; set; }
    public string DurumText { get; set; } = string.Empty;
    public int? KategoriId { get; set; }
    public string? KategoriAdi { get; set; }
    public string KurucuUserId { get; set; } = string.Empty;
    public string KurucuAdSoyad { get; set; } = string.Empty;
    public string? KurucuProfilFotoUrl { get; set; }

    // Tarihler
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? TamamlanmaTarihi { get; set; }
    public int IlerlemeYuzdesi { get; set; }

    // Görünürlük
    public bool HerkeseAcikMi { get; set; }
    public bool YeniUyeKabuluAcikMi { get; set; }

    // Kaynak Çağrı
    public int? KaynakCagriBilgisiId { get; set; }
    public string? KaynakCagriBaslik { get; set; }

    // İstatistikler
    public int GoruntulenmeSayisi { get; set; }
    public int BegeniSayisi { get; set; }
    public int YorumSayisi { get; set; }

    // Ekip ve Görevler
    public List<EkipUyesiViewModel> EkipUyeleri { get; set; } = new();
    public List<GorevOzetViewModel> Gorevler { get; set; } = new();

    // Meta
    public DateTime OlusturmaTarihi { get; set; }
    public string Slug { get; set; } = string.Empty;

    // Kullanıcı Yetkileri
    public bool KullaniciDuzenlemeYetkisiVar { get; set; }
    public bool KullaniciSilmeYetkisiVar { get; set; }
}

/// <summary>
/// Ekip üyesi ViewModel
/// </summary>
public class EkipUyesiViewModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AdSoyad { get; set; } = string.Empty;
    public string? ProfilFotoUrl { get; set; }
    public ProjeRol Rol { get; set; }
    public string RolText { get; set; } = string.Empty;
    public DateTime KatilmaTarihi { get; set; }
}

/// <summary>
/// Görev özet ViewModel
/// </summary>
public class GorevOzetViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public GorevDurumu Durum { get; set; }
    public string DurumText { get; set; } = string.Empty;
    public GorevOncelik Oncelik { get; set; }
    public string OncelikText { get; set; } = string.Empty;
    public string? AtananKisiAdSoyad { get; set; }
    public DateTime? BitisTarihi { get; set; }
}

#endregion

#region Oluşturma ve Güncelleme

/// <summary>
/// Proje oluşturma ViewModel
/// </summary>
public class ProjeOlusturViewModel
{
    [Required(ErrorMessage = "Proje başlığı zorunludur")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
    [Display(Name = "Proje Başlığı")]
    public string Baslik { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Kısa Açıklama")]
    public string? KisaAciklama { get; set; }

    [Display(Name = "Detaylı Açıklama")]
    public string? Aciklama { get; set; }

    [Display(Name = "Kategori")]
    public int? KategoriId { get; set; }

    [Display(Name = "Kapak Resmi")]
    public IFormFile? KapakResmiFile { get; set; }

    [Display(Name = "Başlangıç Tarihi")]
    public DateTime? BaslangicTarihi { get; set; } = DateTime.Now;

    [Display(Name = "Hedef Tamamlanma Tarihi")]
    public DateTime? TamamlanmaTarihi { get; set; }

    [Display(Name = "Herkese Açık")]
    public bool HerkeseAcikMi { get; set; } = true;

    [Display(Name = "Yeni Üye Kabulü Açık")]
    public bool YeniUyeKabuluAcikMi { get; set; } = true;

    [Display(Name = "Hangi Çağrı İçin? (Opsiyonel)")]
    public int? KaynakCagriBilgisiId { get; set; }
}

/// <summary>
/// Proje güncelleme ViewModel
/// </summary>
public class ProjeGuncelleViewModel : ProjeOlusturViewModel
{
    public int Id { get; set; }
    public string? MevcutKapakResmiUrl { get; set; }

    [Display(Name = "İlerleme Yüzdesi")]
    [Range(0, 100)]
    public int IlerlemeYuzdesi { get; set; }

    [Display(Name = "Proje Durumu")]
    public ProjeDurumu Durum { get; set; }
}

#endregion