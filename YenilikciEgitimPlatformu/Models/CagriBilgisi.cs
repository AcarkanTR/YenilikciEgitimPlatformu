using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YenilikciEgitimPlatformu.Models.Identity;

namespace YenilikciEgitimPlatformu.Models;

/*
 * ════════════════════════════════════════════════════════════════════════════
 * SİSTEM 1: ÇAĞRI BİLGİ SİSTEMİ
 * ════════════════════════════════════════════════════════════════════════════
 * 
 * Amaç: Resmi kurum çağrılarının (proje/yarışma/etkinlik) tek merkezden yayınlanması
 * Yönetici: Admin/Moderatör
 * Kullanıcı Etkileşimi: Takip, Katılım Bildirimi, Bildirim
 * 
 * Örnekler:
 * - TÜBİTAK 2204-A Ortaöğretim Öğrencileri Araştırma Projeleri Yarışması
 * - MEB Fen Bilimleri Proje Yarışması
 * - TEKNOFEST Roket Yarışması
 * - Yerel Bilim Festivali
 */

#region Ana Entity

/// <summary>
/// CagriBilgisi - Resmi kurum çağrıları
/// 
/// Bu model MEB, TÜBİTAK, TEKNOFEST gibi kurumların
/// proje/yarışma/etkinlik çağrılarını temsil eder.
/// 
/// Tek model, esnek yapı - tüm çağrı türleri için kullanılır
/// </summary>
public class CagriBilgisi : BaseEntity
{
    #region Genel Bilgiler

    /// <summary>
    /// Çağrı başlığı
    /// Örnek: "TÜBİTAK 2204-A Ortaöğretim Öğrencileri Araştırma Projeleri Yarışması"
    /// </summary>
    [Required]
    [StringLength(300)]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly başlık
    /// </summary>
    [StringLength(350)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Kısa açıklama (Liste sayfasında gösterilir)
    /// </summary>
    [Required]
    [StringLength(500)]
    [Display(Name = "Kısa Açıklama")]
    public string KisaAciklama { get; set; } = string.Empty;

    /// <summary>
    /// Detaylı açıklama (Rich text)
    /// </summary>
    [Display(Name = "Detaylı Açıklama")]
    public string DetayliAciklama { get; set; } = string.Empty;

    /// <summary>
    /// Kapak görseli URL
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Kapak Görseli")]
    public string? KapakGorseliUrl { get; set; }

    #endregion

    #region Çağrı Türü ve Kategori

    /// <summary>
    /// Çağrı türü (Proje, Yarışma, Etkinlik, Fon)
    /// </summary>
    [Required]
    [Display(Name = "Çağrı Türü")]
    public CagriTuru CagriTuru { get; set; }

    /// <summary>
    /// Çağrı kategorisi (Opsiyonel)
    /// Örnek: "Fen Bilimleri", "Sosyal Bilimler", "Teknoloji"
    /// </summary>
    [StringLength(100)]
    [Display(Name = "Kategori")]
    public string? Kategori { get; set; }

    #endregion

    #region Kurumsal Bilgiler

    /// <summary>
    /// Çağrıyı yapan kurum
    /// Örnek: "TÜBİTAK", "MEB", "TEKNOFEST"
    /// </summary>
    [Required]
    [StringLength(200)]
    [Display(Name = "Kurum Adı")]
    public string KurumAdi { get; set; } = string.Empty;

    /// <summary>
    /// Kurumun resmi web sitesi linki
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Resmi Link")]
    public string? ResmiLink { get; set; }

    /// <summary>
    /// İletişim e-posta
    /// </summary>
    [StringLength(200)]
    [EmailAddress]
    [Display(Name = "İletişim E-posta")]
    public string? IletisimEmail { get; set; }

    /// <summary>
    /// İletişim telefon
    /// </summary>
    [StringLength(50)]
    [Phone]
    [Display(Name = "İletişim Telefon")]
    public string? IletisimTelefon { get; set; }

    #endregion

    #region Tarihler

    /// <summary>
    /// Çağrı yayınlanma tarihi (Site üzerinde)
    /// </summary>
    [Required]
    [Display(Name = "Yayınlanma Tarihi")]
    public DateTime YayinlanmaTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Çağrı başlangıç tarihi (Resmi çağrı açılma)
    /// </summary>
    [Display(Name = "Çağrı Başlangıç Tarihi")]
    public DateTime? CagriBaslangicTarihi { get; set; }

    /// <summary>
    /// Çağrı bitiş tarihi (Çağrı kapanma)
    /// </summary>
    [Display(Name = "Çağrı Bitiş Tarihi")]
    public DateTime? CagriBitisTarihi { get; set; }

    /// <summary>
    /// Başvuru başlangıç tarihi
    /// </summary>
    [Display(Name = "Başvuru Başlangıç")]
    public DateTime? BasvuruBaslangicTarihi { get; set; }

    /// <summary>
    /// Başvuru bitiş tarihi
    /// </summary>
    [Display(Name = "Başvuru Bitiş")]
    public DateTime? BasvuruBitisTarihi { get; set; }

    #endregion

    #region Katılım Şartları

    /// <summary>
    /// Katılım şartları (Rich text)
    /// Örnek: "9-12. sınıf öğrencileri", "Yaş sınırı: 14-18"
    /// </summary>
    [Display(Name = "Katılım Şartları")]
    public string? KatilimSartlari { get; set; }

    /// <summary>
    /// Hedef kitle
    /// Örnek: "Ortaöğretim Öğrencileri", "Lise Öğrencileri"
    /// </summary>
    [StringLength(200)]
    [Display(Name = "Hedef Kitle")]
    public string? HedefKitle { get; set; }

    #endregion

    #region Etkinlik Özel Alanları (Sadece CagriTuru.Etkinlik için)

    /// <summary>
    /// Etkinlik başlangıç saati
    /// </summary>
    [Display(Name = "Başlangıç Saati")]
    public TimeSpan? EtkinlikBaslangicSaati { get; set; }

    /// <summary>
    /// Etkinlik bitiş saati
    /// </summary>
    [Display(Name = "Bitiş Saati")]
    public TimeSpan? EtkinlikBitisSaati { get; set; }

    /// <summary>
    /// Etkinlik yeri (Fiziksel konum)
    /// Örnek: "İstanbul Kongre Merkezi"
    /// </summary>
    [StringLength(300)]
    [Display(Name = "Etkinlik Yeri")]
    public string? EtkinlikYeri { get; set; }

    /// <summary>
    /// Online link (Online etkinlikler için)
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Online Link")]
    public string? OnlineLink { get; set; }

    /// <summary>
    /// Maksimum katılımcı sayısı
    /// </summary>
    [Display(Name = "Maksimum Katılımcı")]
    public int? MaksimumKatilimci { get; set; }

    #endregion

    #region Yarışma/Proje Özel Alanları

    /// <summary>
    /// Şartname dosya URL
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Şartname Dosyası")]
    public string? SartnameDosyaUrl { get; set; }

    /// <summary>
    /// Ödül bilgisi
    /// Örnek: "Birinci: 10.000 TL, İkinci: 5.000 TL"
    /// </summary>
    [StringLength(1000)]
    [Display(Name = "Ödül Bilgisi")]
    public string? OdulBilgisi { get; set; }

    /// <summary>
    /// Fon/Bütçe miktarı
    /// </summary>
    [Display(Name = "Bütçe")]
    public decimal? Butce { get; set; }

    #endregion

    #region Durum ve Görünürlük

    /// <summary>
    /// Çağrı aktif mi? (Pasif yapılırsa görünmez)
    /// </summary>
    [Display(Name = "Aktif")]
    public bool AktifMi { get; set; } = true;

    /// <summary>
    /// Yayında mı? (Admin onayı)
    /// </summary>
    [Display(Name = "Yayında")]
    public bool YayindaMi { get; set; } = false;

    /// <summary>
    /// Öne çıkarılsın mı? (Ana sayfada gösterilir)
    /// </summary>
    [Display(Name = "Öne Çıkan")]
    public bool OneCikanMi { get; set; } = false;

    #endregion

    #region Hedef Filtreleme (Okul/İl/İlçe)

    /// <summary>
    /// Hedef il (Null ise tüm iller)
    /// </summary>
    [Display(Name = "Hedef İl")]
    public int? HedefIlId { get; set; }

    [ForeignKey(nameof(HedefIlId))]
    public virtual Il? HedefIl { get; set; }

    /// <summary>
    /// Hedef ilçe (Null ise tüm ilçeler)
    /// </summary>
    [Display(Name = "Hedef İlçe")]
    public int? HedefIlceId { get; set; }

    [ForeignKey(nameof(HedefIlceId))]
    public virtual Ilce? HedefIlce { get; set; }

    /// <summary>
    /// Hedef okul (Null ise tüm okullar)
    /// </summary>
    [Display(Name = "Hedef Okul")]
    public int? HedefOkulId { get; set; }

    [ForeignKey(nameof(HedefOkulId))]
    public virtual Okul? HedefOkul { get; set; }

    #endregion

    #region İlişkiler

    /// <summary>
    /// Ek dosyalar (PDF, video, görsel)
    /// </summary>
    public virtual ICollection<CagriEkDosya> EkDosyalar { get; set; } = new List<CagriEkDosya>();

    /// <summary>
    /// Çağrıyı takip eden kullanıcılar
    /// </summary>
    public virtual ICollection<CagriTakip> Takipciler { get; set; } = new List<CagriTakip>();

    #endregion

    #region Helper Properties

    /// <summary>
    /// Çağrı devam ediyor mu?
    /// </summary>
    [NotMapped]
    public bool DevamEdiyorMu
    {
        get
        {
            if (!CagriBaslangicTarihi.HasValue || !CagriBitisTarihi.HasValue)
                return true;

            var now = DateTime.UtcNow;
            return now >= CagriBaslangicTarihi.Value && now <= CagriBitisTarihi.Value;
        }
    }

    /// <summary>
    /// Başvurular açık mı?
    /// </summary>
    [NotMapped]
    public bool BasvuruAcikMi
    {
        get
        {
            if (!BasvuruBaslangicTarihi.HasValue || !BasvuruBitisTarihi.HasValue)
                return false;

            var now = DateTime.UtcNow;
            return now >= BasvuruBaslangicTarihi.Value && now <= BasvuruBitisTarihi.Value;
        }
    }

    /// <summary>
    /// Çağrı sona erdi mi?
    /// </summary>
    [NotMapped]
    public bool SonaErdiMi
    {
        get
        {
            if (!CagriBitisTarihi.HasValue)
                return false;

            return DateTime.UtcNow > CagriBitisTarihi.Value;
        }
    }

    #endregion
}

#endregion

#region Ek Dosyalar

/// <summary>
/// CagriEkDosya - Çağrıya ait ek dosyalar
/// (PDF, Video, Görsel, Link)
/// </summary>
public class CagriEkDosya : BaseEntity
{
    [Required]
    [Display(Name = "Çağrı")]
    public int CagriBilgisiId { get; set; }

    [ForeignKey(nameof(CagriBilgisiId))]
    public virtual CagriBilgisi CagriBilgisi { get; set; } = null!;

    /// <summary>
    /// Dosya URL veya Link
    /// </summary>
    [Required]
    [StringLength(1000)]
    [Display(Name = "Dosya URL")]
    public string DosyaUrl { get; set; } = string.Empty;

    /// <summary>
    /// Dosya türü
    /// </summary>
    [Required]
    [Display(Name = "Dosya Türü")]
    public CagriDosyaTuru DosyaTuru { get; set; }

    /// <summary>
    /// Dosya başlığı
    /// Örnek: "Başvuru Formu", "Tanıtım Videosu"
    /// </summary>
    [Required]
    [StringLength(200)]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    /// <summary>
    /// Dosya açıklaması
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    /// <summary>
    /// Dosya boyutu (byte)
    /// </summary>
    [Display(Name = "Dosya Boyutu")]
    public long? DosyaBoyutu { get; set; }

    /// <summary>
    /// Sıra numarası (Gösterim sırası)
    /// </summary>
    [Display(Name = "Sıra")]
    public int Sira { get; set; } = 0;
}

#endregion

#region Takip

/// <summary>
/// CagriTakip - Kullanıcı çağrı takip ilişkisi
/// 
/// Kullanıcı bir çağrıyı takip eder, değişiklikler hakkında bildirim alır
/// </summary>
public class CagriTakip
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CagriBilgisiId { get; set; }

    [ForeignKey(nameof(CagriBilgisiId))]
    public virtual CagriBilgisi CagriBilgisi { get; set; } = null!;

    [Required]
    [StringLength(450)]
    public string KullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(KullaniciId))]
    public virtual ApplicationUser Kullanici { get; set; } = null!;

    [Display(Name = "Takip Tarihi")]
    public DateTime TakipTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// E-posta bildirimi alsın mı?
    /// </summary>
    [Display(Name = "E-posta Bildirimi")]
    public bool EmailBildirimiAlsin { get; set; } = true;

    [Required]
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
}

#endregion

#region Enum'lar

/// <summary>
/// Çağrı türü
/// </summary>
public enum CagriTuru
{
    [Display(Name = "Proje Çağrısı")]
    Proje = 1,

    [Display(Name = "Yarışma")]
    Yarisma = 2,

    [Display(Name = "Etkinlik")]
    Etkinlik = 3,

    [Display(Name = "Fon/Destek Programı")]
    FonProgrami = 4,

    [Display(Name = "Eğitim/Workshop")]
    Egitim = 5,

    [Display(Name = "Seminer")]
    Seminer = 7,

    [Display(Name = "Diğer")]
    Diger = 6
}

/// <summary>
/// Çağrı ek dosya türü
/// </summary>
public enum CagriDosyaTuru
{
    [Display(Name = "PDF Döküman")]
    PDF = 1,

    [Display(Name = "Video")]
    Video = 2,

    [Display(Name = "Görsel/Resim")]
    Gorsel = 3,

    [Display(Name = "Link/URL")]
    Link = 4,

    [Display(Name = "Şartname")]
    Sartname = 5,

    [Display(Name = "Başvuru Formu")]
    BasvuruFormu = 6
}

#endregion