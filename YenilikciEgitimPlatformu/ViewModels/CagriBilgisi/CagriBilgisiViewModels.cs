using System.ComponentModel.DataAnnotations;
using YenilikciEgitimPlatformu.Models;

namespace YenilikciEgitimPlatformu.ViewModels.CagriBilgisi;

#region Liste ve Filtreleme

/// <summary>
/// Çağrı listesi için ViewModel
/// </summary>
public class CagriListViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? KisaAciklama { get; set; }
    public string? KapakResmiUrl { get; set; }
    public CagriTuru Turu { get; set; }
    public string TuruText { get; set; } = string.Empty;
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public string Kurum { get; set; } = string.Empty;
    public int TakipciSayisi { get; set; }
    public int GoruntulenmeSayisi { get; set; }
    public bool Aktif { get; set; }
    public string Slug { get; set; } = string.Empty;
}

/// <summary>
/// Çağrı filtreleme ViewModel
/// </summary>
public class CagriFiltreleViewModel
{
    [Display(Name = "Arama")]
    public string? Arama { get; set; }

    [Display(Name = "Çağrı Türü")]
    public CagriTuru? Turu { get; set; }

    [Display(Name = "Kurum")]
    public string? Kurum { get; set; }

    [Display(Name = "İl")]
    public int? IlId { get; set; }

    [Display(Name = "İlçe")]
    public int? IlceId { get; set; }

    [Display(Name = "Başlangıç Tarihi (Min)")]
    public DateTime? BaslangicTarihiMin { get; set; }

    [Display(Name = "Başlangıç Tarihi (Max)")]
    public DateTime? BaslangicTarihiMax { get; set; }

    [Display(Name = "Sadece Aktif")]
    public bool SadeceAktif { get; set; } = true;

    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    // Sorting
    public string SortBy { get; set; } = "BaslangicTarihi";
    public bool SortDescending { get; set; } = true;
}

#endregion

#region Detay

/// <summary>
/// Çağrı detay ViewModel
/// </summary>
public class CagriDetayViewModel
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? KapakResmiUrl { get; set; }
    public string? Aciklama { get; set; }
    public CagriTuru Turu { get; set; }
    public string TuruText { get; set; } = string.Empty;
    public string Kurum { get; set; } = string.Empty;
    public string? KurumLogoUrl { get; set; }
    public DateTime? BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public string? BasvuruLinki { get; set; }
    public string? IletisimEmail { get; set; }
    public string? IletisimTelefon { get; set; }

    // Hedef Kitle
    public string? HedefKitle { get; set; }
    public int? HedefIlId { get; set; }
    public string? HedefIlAdi { get; set; }
    public int? HedefIlceId { get; set; }
    public string? HedefIlceAdi { get; set; }

    // İstatistikler
    public int TakipciSayisi { get; set; }
    public int GoruntulenmeSayisi { get; set; }
    public bool KullaniciTakipEdiyor { get; set; }

    // Ek Dosyalar
    public List<CagriDosyaViewModel> EkDosyalar { get; set; } = new();

    // Meta
    public DateTime OlusturmaTarihi { get; set; }
    public DateTime? GuncellemeTarihi { get; set; }
    public string Slug { get; set; } = string.Empty;
}

/// <summary>
/// Çağrı ek dosya ViewModel
/// </summary>
public class CagriDosyaViewModel
{
    public int Id { get; set; }
    public string DosyaAdi { get; set; } = string.Empty;
    public string DosyaUrl { get; set; } = string.Empty;
    public CagriDosyaTuru DosyaTuru { get; set; }
    public string DosyaTuruText { get; set; } = string.Empty;
    public long DosyaBoyutu { get; set; }
    public string DosyaBoyutuText { get; set; } = string.Empty;
}

#endregion

#region Oluşturma ve Güncelleme

/// <summary>
/// Çağrı oluşturma ViewModel (Admin)
/// </summary>
public class CagriOlusturViewModel
{
    [Required(ErrorMessage = "Başlık zorunludur")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Kısa Açıklama")]
    public string? KisaAciklama { get; set; }

    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [Required(ErrorMessage = "Çağrı türü zorunludur")]
    [Display(Name = "Çağrı Türü")]
    public CagriTuru Turu { get; set; }

    [Required(ErrorMessage = "Kurum adı zorunludur")]
    [StringLength(200)]
    [Display(Name = "Kurum")]
    public string Kurum { get; set; } = string.Empty;

    [Display(Name = "Kurum Logo")]
    public IFormFile? KurumLogoFile { get; set; }

    [Display(Name = "Kapak Resmi")]
    public IFormFile? KapakResmiFile { get; set; }

    [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
    [Display(Name = "Başlangıç Tarihi")]
    public DateTime? BaslangicTarihi { get; set; }

    [Display(Name = "Bitiş Tarihi")]
    public DateTime? BitisTarihi { get; set; }

    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    [Display(Name = "Başvuru Linki")]
    public string? BasvuruLinki { get; set; }

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    [Display(Name = "İletişim E-posta")]
    public string? IletisimEmail { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    [Display(Name = "İletişim Telefon")]
    public string? IletisimTelefon { get; set; }

    [StringLength(500)]
    [Display(Name = "Hedef Kitle")]
    public string? HedefKitle { get; set; }

    [Display(Name = "Hedef İl")]
    public int? HedefIlId { get; set; }

    [Display(Name = "Hedef İlçe")]
    public int? HedefIlceId { get; set; }
}

/// <summary>
/// Çağrı güncelleme ViewModel (Admin)
/// </summary>
public class CagriGuncelleViewModel : CagriOlusturViewModel
{
    public int Id { get; set; }
    public string? MevcutKapakResmiUrl { get; set; }
    public string? MevcutKurumLogoUrl { get; set; }
}

#endregion