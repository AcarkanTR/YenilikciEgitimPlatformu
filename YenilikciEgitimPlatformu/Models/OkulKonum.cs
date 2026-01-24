using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YenilikciEgitimPlatformu.Models.Identity;

namespace YenilikciEgitimPlatformu.Models;

/*
 * Okul ve Konum Entity Modelleri
 * 
 * Bu dosya eğitim sisteminin coğrafi ve kurumsal yapısını içerir:
 * - İl (81 il)
 * - İlçe (İllere bağlı)
 * - Okul (İlçelere bağlı)
 * 
 * İlişki Yapısı:
 * İl (1) -> (N) İlçe (1) -> (N) Okul
 * 
 * Kullanım Alanları:
 * - Kullanıcı kaydı (Okul seçimi)
 * - Filtreleme (İl/İlçe bazlı arama)
 * - Raporlama (Bölgesel istatistikler)
 * - Duyuru/Etkinlik hedefleme
 */

#region İl (Province) Entity

/// <summary>
/// İl (Province) - Türkiye'nin 81 ili
/// 
/// Bu tablo seed data ile doldurulur (DbSeeder.cs)
/// </summary>
public class Il
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "İl adı zorunludur")]
    [StringLength(50, ErrorMessage = "İl adı en fazla 50 karakter olabilir")]
    [Display(Name = "İl Adı")]
    public string Ad { get; set; } = string.Empty;

    /// <summary>
    /// İl plaka kodu (1-81)
    /// </summary>
    [Required]
    [Range(1, 81, ErrorMessage = "Plaka kodu 1-81 arasında olmalıdır")]
    [Display(Name = "Plaka Kodu")]
    public int PlakaKodu { get; set; }

    /// <summary>
    /// İlin coğrafi bölgesi
    /// Örnek: Marmara, Ege, Karadeniz, İç Anadolu, Akdeniz, Doğu Anadolur, Güneydoğu Anadolu
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Bölge")]
    public string? Bolge { get; set; }

    #region Navigation Properties

    /// <summary>
    /// İle bağlı ilçeler
    /// </summary>
    public virtual ICollection<Ilce> Ilceler { get; set; } = new List<Ilce>();

    /// <summary>
    /// İldeki kullanıcılar
    /// </summary>
    public virtual ICollection<ApplicationUser> Kullanicilar { get; set; } = new List<ApplicationUser>();

    /// <summary>
    /// İldeki okullar (doğrudan ilişki)
    /// </summary>
    public virtual ICollection<Okul> Okullar { get; set; } = new List<Okul>();

    #endregion
}

#endregion

#region İlçe (District) Entity

/// <summary>
/// İlçe (District) - İllere bağlı ilçeler
/// 
/// Her ilçe bir ile bağlıdır.
/// Bu tablo seed data ile doldurulur.
/// </summary>
public class Ilce
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "İlçe adı zorunludur")]
    [StringLength(100, ErrorMessage = "İlçe adı en fazla 100 karakter olabilir")]
    [Display(Name = "İlçe Adı")]
    public string Ad { get; set; } = string.Empty;

    #region Foreign Keys

    [Required(ErrorMessage = "İl seçimi zorunludur")]
    [Display(Name = "İl")]
    public int IlId { get; set; }

    [ForeignKey(nameof(IlId))]
    public virtual Il Il { get; set; } = null!;

    #endregion

    #region Navigation Properties

    /// <summary>
    /// İlçedeki kullanıcılar
    /// </summary>
    public virtual ICollection<ApplicationUser> Kullanicilar { get; set; } = new List<ApplicationUser>();

    /// <summary>
    /// İlçedeki okullar
    /// </summary>
    public virtual ICollection<Okul> Okullar { get; set; } = new List<Okul>();

    #endregion
}

#endregion

#region Okul (School) Entity

/// <summary>
/// Okul (School) - Eğitim kurumları
/// 
/// Öğretmen ve öğrenciler bir okula bağlıdır.
/// Okullar ilçelere bağlıdır.
/// </summary>
public class Okul : BaseEntity
{
    [Required(ErrorMessage = "Okul adı zorunludur")]
    [StringLength(200, ErrorMessage = "Okul adı en fazla 200 karakter olabilir")]
    [Display(Name = "Okul Adı")]
    public string Ad { get; set; } = string.Empty;

    /// <summary>
    /// Okul türü
    /// Örnek: İlkokul, Ortaokul, Lise, Özel Okul, Meslek Lisesi
    /// </summary>
    [Required(ErrorMessage = "Okul türü zorunludur")]
    [Display(Name = "Okul Türü")]
    public OkulTuru Turu { get; set; }

    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [StringLength(500)]
    [Display(Name = "Adres")]
    public string? Adres { get; set; }

    [StringLength(20)]
    [Display(Name = "Telefon")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string? Telefon { get; set; }

    [StringLength(100)]
    [Display(Name = "E-posta")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string? Email { get; set; }

    [StringLength(255)]
    [Display(Name = "Website")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string? WebsiteUrl { get; set; }

    /// <summary>
    /// Okul logosu
    /// </summary>
    [StringLength(255)]
    [Display(Name = "Logo URL")]
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Okul aktif mi? (Kapatılan okullar pasif yapılır)
    /// </summary>
    [Display(Name = "Aktif Mi?")]
    public new bool AktifMi { get; set; } = true;

    /// <summary>
    /// MEB okul kodu (varsa)
    /// </summary>
    [StringLength(50)]
    [Display(Name = "MEB Okul Kodu")]
    public string? MEBOkulKodu { get; set; }

    #region Foreign Keys

    [Required(ErrorMessage = "İl seçimi zorunludur")]
    [Display(Name = "İl")]
    public int IlId { get; set; }

    [ForeignKey(nameof(IlId))]
    public virtual Il Il { get; set; } = null!;

    [Required(ErrorMessage = "İlçe seçimi zorunludur")]
    [Display(Name = "İlçe")]
    public int IlceId { get; set; }

    [ForeignKey(nameof(IlceId))]
    public virtual Ilce Ilce { get; set; } = null!;

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Okuldaki kullanıcılar (Öğretmen ve Öğrenciler)
    /// </summary>
    public virtual ICollection<ApplicationUser> Kullanicilar { get; set; } = new List<ApplicationUser>();

    /// <summary>
    /// Okuldan oluşturulan kullanıcı projeleri (Sistem 2)
    /// </summary>
    public virtual ICollection<ProjeYonetimi> ProjeYonetimleri { get; set; } = new List<ProjeYonetimi>();

    #endregion

    #region Helper Properties

    /// <summary>
    /// Okulun tam adı (İl, İlçe ile birlikte)
    /// Örnek: "Ankara - Çankaya - Atatürk Anadolu Lisesi"
    /// </summary>
    [NotMapped]
    public string TamAd => $"{Il?.Ad} - {Ilce?.Ad} - {Ad}";

    #endregion
}

#endregion

#region Okul Türü Enum

/// <summary>
/// Okul Türleri
/// 
/// Türkiye Milli Eğitim Sistemi'ndeki okul türleri
/// </summary>
public enum OkulTuru
{
    [Display(Name = "İlkokul")]
    Ilkokul = 1,

    [Display(Name = "Ortaokul")]
    Ortaokul = 2,

    [Display(Name = "Lise (Genel)")]
    Lise = 3,

    [Display(Name = "Anadolu Lisesi")]
    AnadoluLisesi = 4,

    [Display(Name = "Meslek Lisesi")]
    MeslekLisesi = 5,

    [Display(Name = "Fen Lisesi")]
    FenLisesi = 6,

    [Display(Name = "Sosyal Bilimler Lisesi")]
    SosyalBilimlerLisesi = 7,

    [Display(Name = "Güzel Sanatlar Lisesi")]
    GuzelSanatlarLisesi = 8,

    [Display(Name = "Spor Lisesi")]
    SporLisesi = 9,

    [Display(Name = "İmam Hatip Lisesi")]
    ImamHatipLisesi = 10,

    [Display(Name = "Özel Okul")]
    OzelOkul = 11,

    [Display(Name = "Özel Eğitim Okulu")]
    OzelEgitimOkulu = 12,

    [Display(Name = "Anaokulu")]
        Anaokulu = 13,

    [Display(Name = "Diğer")]
    Diger = 99
}

#endregion