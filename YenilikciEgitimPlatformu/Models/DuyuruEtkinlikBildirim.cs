using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YenilikciEgitimPlatformu.Models.Identity;

namespace YenilikciEgitimPlatformu.Models;

/*
 * Duyuru, Etkinlik ve Bildirim Entity Modelleri
 * 
 * Platform'un içerik yönetim ve bildirim sistemini içerir:
 * - Duyuru: Sistem duyuruları (haberler, duyurular)
 * - Etkinlik: Eğitim etkinlikleri (webinar, workshop vs.)
 * - Bildirim: Kullanıcı bildirimleri (SignalR entegrasyonu)
 */

#region Duyuru Entity (Announcement)

/// <summary>
/// Duyuru - Sistem duyuruları ve haberler
/// 
/// Özellikler:
/// - Başlangıç/Bitiş tarihleri
/// - Hedef kitle filtreleme (İl, İlçe, Okul)
/// - Dosya ekleri
/// - Takip sistemi
/// - Görüntülenme istatistikleri
/// </summary>
public class Duyuru : BaseEntity
{
    [Required(ErrorMessage = "Duyuru başlığı zorunludur")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duyuru içeriği zorunludur")]
    [Display(Name = "İçerik")]
    [DataType(DataType.Html)]
    public string Icerik { get; set; } = string.Empty;

    /// <summary>
    /// Özet metin (Liste görünümü için)
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Özet")]
    public string? Ozet { get; set; }

    /// <summary>
    /// SEO uyumlu slug
    /// </summary>
    [Required]
    [StringLength(255)]
    [Display(Name = "URL Slug")]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Kapak görseli
    /// </summary>
    [StringLength(255)]
    [Display(Name = "Kapak Görseli")]
    public string? KapakGorseliUrl { get; set; }

    #region Tarih Bilgileri

    [Required]
    [Display(Name = "Yayın Başlangıç Tarihi")]
    [DataType(DataType.DateTime)]
    public DateTime BaslangicTarihi { get; set; } = DateTime.UtcNow;

    [Display(Name = "Yayın Bitiş Tarihi")]
    [DataType(DataType.DateTime)]
    public DateTime? BitisTarihi { get; set; }

    #endregion

    #region Kategori ve Öncelik

    [Required]
    [Display(Name = "Kategori")]
    public int KategoriId { get; set; }

    [ForeignKey(nameof(KategoriId))]
    public virtual DuyuruKategori Kategori { get; set; } = null!;

    [Required]
    [Display(Name = "Öncelik")]
    public DuyuruOncelik Oncelik { get; set; } = DuyuruOncelik.Normal;

    #endregion

    #region Hedef Kitle Filtreleme

    /// <summary>
    /// Duyuru sadece belirli illere mi gösterilsin?
    /// Null ise tüm Türkiye
    /// </summary>
    [Display(Name = "Hedef İl")]
    public int? HedefIlId { get; set; }

    [ForeignKey(nameof(HedefIlId))]
    public virtual Il? HedefIl { get; set; }

    /// <summary>
    /// Duyuru sadece belirli ilçelere mi gösterilsin?
    /// </summary>
    [Display(Name = "Hedef İlçe")]
    public int? HedefIlceId { get; set; }

    [ForeignKey(nameof(HedefIlceId))]
    public virtual Ilce? HedefIlce { get; set; }

    /// <summary>
    /// Duyuru sadece belirli okullara mı gösterilsin?
    /// </summary>
    [Display(Name = "Hedef Okul")]
    public int? HedefOkulId { get; set; }

    [ForeignKey(nameof(HedefOkulId))]
    public virtual Okul? HedefOkul { get; set; }

    #endregion

    #region Durum ve Yayın Kontrolü

    [Display(Name = "Yayında")]
    public bool YayindaMi { get; set; } = false;

    [Display(Name = "Öne Çıkan")]
    public bool OneCikanMi { get; set; } = false;

    /// <summary>
    /// Duyuru onaylandı mı? (Moderasyon)
    /// </summary>
    [Display(Name = "Onaylandı")]
    public new bool OnaylandiMi { get; set; } = false;

    #endregion

    #region Ek Bilgiler

    /// <summary>
    /// Dış link (Detay için)
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Dış Link")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string? DisLink { get; set; }

    /// <summary>
    /// PDF dosyası URL
    /// </summary>
    [StringLength(500)]
    [Display(Name = "PDF URL")]
    public string? PdfUrl { get; set; }

    #endregion

    #region İstatistikler

    [Display(Name = "Görüntülenme Sayısı")]
    public int GoruntulenmeSayisi { get; set; } = 0;

    [Display(Name = "Takip Eden Sayısı")]
    public int TakipEdenSayisi { get; set; } = 0;

    #endregion

    #region Navigation Properties

    public virtual ICollection<DuyuruTakip> Takipciler { get; set; } = new List<DuyuruTakip>();
    public virtual ICollection<Dosya> Dosyalar { get; set; } = new List<Dosya>();

    #endregion

    #region Helper Properties

    /// <summary>
    /// Duyuru aktif mi? (Tarih aralığı kontrolü)
    /// </summary>
    [NotMapped]
    public bool AktifMi
    {
        get
        {
            var now = DateTime.UtcNow;
            return YayindaMi &&
                   now >= BaslangicTarihi &&
                   (!BitisTarihi.HasValue || now <= BitisTarihi.Value);
        }
    }

    #endregion
}

#endregion

#region Duyuru Kategori Entity

public class DuyuruKategori
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Kategori Adı")]
    public string Ad { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "İkon")]
    public string? Ikon { get; set; }

    [StringLength(50)]
    [Display(Name = "Renk")]
    public string? Renk { get; set; }

    public virtual ICollection<Duyuru> Duyurular { get; set; } = new List<Duyuru>();
}

#endregion

#region Duyuru Takip Entity (Junction Table)

/// <summary>
/// DuyuruTakip - Kullanıcı duyuru takip ilişkisi
/// 
/// NOT: AuditOnlyBaseEntity kullanılmıyor (Cascade path çakışması önleme)
/// </summary>
public class DuyuruTakip
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int DuyuruId { get; set; }

    [ForeignKey(nameof(DuyuruId))]
    public virtual Duyuru Duyuru { get; set; } = null!;

    [Required]
    [StringLength(450)]
    public string KullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(KullaniciId))]
    public virtual ApplicationUser Kullanici { get; set; } = null!;

    [Display(Name = "Takip Tarihi")]
    public DateTime TakipTarihi { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
}

#endregion

#region Etkinlik Entity (Event)

/// <summary>
/// Etkinlik - Eğitim etkinlikleri
/// 
/// Özellikler:
/// - Tarih/saat bilgileri
/// - Konum (Fiziksel veya Online)
/// - Katılımcı yönetimi
/// - Kontenjan kontrolü
/// </summary>
public class Etkinlik : BaseEntity
{
    [Required(ErrorMessage = "Etkinlik başlığı zorunludur")]
    [StringLength(200)]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    [Required(ErrorMessage = "Etkinlik açıklaması zorunludur")]
    [Display(Name = "Açıklama")]
    [DataType(DataType.Html)]
    public string Aciklama { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Özet")]
    public string? Ozet { get; set; }

    [StringLength(255)]
    [Display(Name = "Kapak Görseli")]
    public string? KapakGorseliUrl { get; set; }

    #region Tarih ve Saat

    [Required]
    [Display(Name = "Başlangıç Tarihi")]
    [DataType(DataType.DateTime)]
    public DateTime BaslangicTarihi { get; set; }

    [Required]
    [Display(Name = "Bitiş Tarihi")]
    [DataType(DataType.DateTime)]
    public DateTime BitisTarihi { get; set; }

    #endregion

    #region Konum Bilgileri

    [Required]
    [Display(Name = "Etkinlik Türü")]
    public EtkinlikTuru Turu { get; set; }

    /// <summary>
    /// Fiziksel konum (Etkinlik fiziksel ise)
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Konum")]
    public string? Konum { get; set; }

    /// <summary>
    /// Online link (Etkinlik online ise)
    /// Örnek: Zoom, Teams, Google Meet linki
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Online Link")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string? OnlineLink { get; set; }

    #endregion

    #region Katılımcı Yönetimi

    /// <summary>
    /// Maksimum katılımcı sayısı (Null ise sınırsız)
    /// </summary>
    [Display(Name = "Maksimum Katılımcı")]
    public int? MaksimumKatilimci { get; set; }

    /// <summary>
    /// Mevcut katılımcı sayısı
    /// </summary>
    [Display(Name = "Mevcut Katılımcı")]
    public int MevcutKatilimci { get; set; } = 0;

    /// <summary>
    /// Kayıt açık mı?
    /// </summary>
    [Display(Name = "Kayıt Açık")]
    public bool KayitAcik { get; set; } = true;

    #endregion

    #region Durum

    [Display(Name = "Yayında")]
    public bool YayindaMi { get; set; } = false;

    [Display(Name = "Öne Çıkan")]
    public bool OneCikanMi { get; set; } = false;

    #endregion

    #region Navigation Properties

    public virtual ICollection<EtkinlikKatilimci> Katilimcilar { get; set; } = new List<EtkinlikKatilimci>();
    public virtual ICollection<Dosya> Dosyalar { get; set; } = new List<Dosya>();

    #endregion

    #region Helper Properties

    [NotMapped]
    public bool KontenjanDoluMu =>
        MaksimumKatilimci.HasValue && MevcutKatilimci >= MaksimumKatilimci.Value;

    [NotMapped]
    public bool BasladiMi => DateTime.UtcNow >= BaslangicTarihi;

    [NotMapped]
    public bool BittiMi => DateTime.UtcNow > BitisTarihi;

    [NotMapped]
    public EtkinlikDurum Durum
    {
        get
        {
            if (BittiMi) return EtkinlikDurum.Bitti;
            if (BasladiMi) return EtkinlikDurum.DevamEdiyor;
            return EtkinlikDurum.Beklemede;
        }
    }

    #endregion
}

#endregion

#region Etkinlik Katılımcı Entity (Junction Table)

public class EtkinlikKatilimci
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int EtkinlikId { get; set; }

    [ForeignKey(nameof(EtkinlikId))]
    public virtual Etkinlik Etkinlik { get; set; } = null!;

    [Required]
    [StringLength(450)]
    public string KullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(KullaniciId))]
    public virtual ApplicationUser Kullanici { get; set; } = null!;

    [Display(Name = "Kayıt Tarihi")]
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;

    [Display(Name = "Katıldı Mı?")]
    public bool KatildiMi { get; set; } = false;

    [Display(Name = "Katılım Tarihi")]
    public DateTime? KatilimTarihi { get; set; }

    [Display(Name = "İptal Edildi Mi?")]
    public bool IptalEdildiMi { get; set; } = false;

    [Required]
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
}

#endregion

#region Bildirim Entity (Notification)

/// <summary>
/// Bildirim - Kullanıcı bildirimleri
/// 
/// Özellikler:
/// - SignalR entegrasyonu
/// - Okunma takibi
/// - Çoklu bildirim türü
/// - Deep linking (Bildirimi tıklayınca ilgili sayfaya git)
/// 
/// NOT: AuditOnlyBaseEntity kullanılmıyor (Cascade path çakışması önleme)
/// </summary>
public class Bildirim
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Bildirimin gönderildiği kullanıcı
    /// </summary>
    [Required]
    [StringLength(450)]
    [Display(Name = "Alıcı Kullanıcı")]
    public string AliciKullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(AliciKullaniciId))]
    public virtual ApplicationUser AliciKullanici { get; set; } = null!;

    /// <summary>
    /// Bildirimi gönderen kullanıcı (Opsiyonel)
    /// </summary>
    [StringLength(450)]
    [Display(Name = "Gönderen Kullanıcı")]
    public string? GonderenKullaniciId { get; set; }

    [ForeignKey(nameof(GonderenKullaniciId))]
    public virtual ApplicationUser? GonderenKullanici { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "Mesaj")]
    public string Mesaj { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Bildirim Türü")]
    public BildirimTuru Turu { get; set; }

    #region Deep Linking (İlgili Sayfa)

    /// <summary>
    /// Bildirime tıklayınca gidilecek URL
    /// Örnek: "/Projeler/Detay/123"
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Hedef URL")]
    public string? HedefUrl { get; set; }

    /// <summary>
    /// İlgili entity türü
    /// Örnek: "Proje", "Gonderi"
    /// </summary>
    [StringLength(50)]
    [Display(Name = "İlişkili Entity Türü")]
    public string? IliskiliEntityTuru { get; set; }

    /// <summary>
    /// İlgili entity ID'si
    /// </summary>
    [Display(Name = "İlişkili Entity ID")]
    public int? IliskiliEntityId { get; set; }

    #endregion

    #region Okunma Durumu

    [Display(Name = "Okundu Mu?")]
    public bool OkunduMu { get; set; } = false;

    [Display(Name = "Okunma Tarihi")]
    public DateTime? OkunmaTarihi { get; set; }

    #endregion

    #region İkon ve Stil

    /// <summary>
    /// Font Awesome ikon class
    /// Örnek: "fa-solid fa-bell"
    /// </summary>
    [StringLength(50)]
    [Display(Name = "İkon")]
    public string? Ikon { get; set; }

    /// <summary>
    /// Bildirim rengi (Tailwind CSS class)
    /// Örnek: "blue", "green", "red"
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Renk")]
    public string? Renk { get; set; }

    #endregion

    [Required]
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
}

#endregion

#region Enums

public enum DuyuruOncelik
{
    [Display(Name = "Düşük")]
    Dusuk = 0,

    [Display(Name = "Normal")]
    Normal = 1,

    [Display(Name = "Yüksek")]
    Yuksek = 2,

    [Display(Name = "Acil")]
    Acil = 3
}

public enum EtkinlikTuru
{
    [Display(Name = "Fiziksel")]
    Fiziksel = 0,

    [Display(Name = "Online")]
    Online = 1,

    [Display(Name = "Hibrit")]
    Hibrit = 2
}

public enum EtkinlikDurum
{
    [Display(Name = "Beklemede")]
    Beklemede = 0,

    [Display(Name = "Devam Ediyor")]
    DevamEdiyor = 1,

    [Display(Name = "Bitti")]
    Bitti = 2
}

public enum BildirimTuru
{
    [Display(Name = "Sistem")]
    Sistem = 0,

    [Display(Name = "Proje")]
    Proje = 1,

    [Display(Name = "Gönderi")]
    Gonderi = 2,

    [Display(Name = "Yorum")]
    Yorum = 3,

    [Display(Name = "Beğeni")]
    Begeni = 4,

    [Display(Name = "Görev")]
    Gorev = 5,

    [Display(Name = "Etkinlik")]
    Etkinlik = 6,

    [Display(Name = "Duyuru")]
    Duyuru = 7,

    [Display(Name = "Rozet")]
    Rozet = 8,

    [Display(Name = "Seviye")]
    Seviye = 9
}

#endregion