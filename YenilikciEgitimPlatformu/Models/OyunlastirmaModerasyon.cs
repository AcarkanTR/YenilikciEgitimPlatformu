using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YenilikciEgitimPlatformu.Models.Identity;

namespace YenilikciEgitimPlatformu.Models;

/*
 * Oyunlaştırma ve Moderasyon Entity Modelleri
 * 
 * Bu dosya platformun oyunlaştırma ve moderasyon sistemlerini içerir:
 * - Rozet: Başarı rozetleri
 * - KullaniciRozet: Kullanıcı-Rozet ilişkisi
 * - ModerasyonKaydi: İçerik moderasyon kayıtları
 * - AuditLog: Sistem audit trail
 */

#region Rozet Entity (Badge/Achievement)

/// <summary>
/// Rozet - Oyunlaştırma başarı rozetleri
/// 
/// Özellikler:
/// - Event-based kazanım
/// - Seviyelendirme (Bronze, Silver, Gold)
/// - XP ödülü
/// - Görsel rozet tasarımı
/// 
/// Örnekler:
/// - "İlk Adım": İlk gönderi paylaşma
/// - "Proje Ustası": 10 proje tamamlama
/// - "Yorum Kralı": 100 yorum yapma
/// - "Takım Oyuncusu": İlk proje ekibine katılma
/// </summary>
public class Rozet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Rozet Adı")]
    public string Ad { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string Aciklama { get; set; } = string.Empty;

    /// <summary>
    /// Rozet görseli URL
    /// </summary>
    [Required]
    [StringLength(255)]
    [Display(Name = "Rozet Görseli")]
    public string GorselUrl { get; set; } = string.Empty;

    /// <summary>
    /// Rozet ikonu (Font Awesome)
    /// Örnek: "fa-solid fa-trophy"
    /// </summary>
    [StringLength(50)]
    [Display(Name = "İkon")]
    public string? Ikon { get; set; }

    /// <summary>
    /// Rozet seviyesi
    /// </summary>
    [Required]
    [Display(Name = "Seviye")]
    public RozetSeviye Seviye { get; set; }

    /// <summary>
    /// Rozet rengi (Tailwind CSS)
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Renk")]
    public string? Renk { get; set; }

    /// <summary>
    /// Rozet kazanıldığında verilecek XP
    /// </summary>
    [Required]
    [Range(0, 1000)]
    [Display(Name = "XP Ödülü")]
    public int XPOdulu { get; set; }

    /// <summary>
    /// Rozet kategorisi
    /// </summary>
    [Required]
    [Display(Name = "Kategori")]
    public RozetKategori Kategori { get; set; }

    /// <summary>
    /// Rozet kazanım koşulu (Event adı)
    /// Örnek: "IlkGonderi", "IlkProje", "ProfilTamamlama"
    /// 
    /// Bu alan Service Layer'da event trigger için kullanılır
    /// </summary>
    [Required]
    [StringLength(100)]
    [Display(Name = "Kazanım Koşulu")]
    public string KosulEventAdi { get; set; } = string.Empty;

    /// <summary>
    /// Koşul değeri (Örn: 10 proje tamamlanması için 10)
    /// </summary>
    [Display(Name = "Koşul Değeri")]
    public int? KosulDegeri { get; set; }

    /// <summary>
    /// Rozet nadir mi? (Özel rozetler)
    /// </summary>
    [Display(Name = "Nadir Rozet")]
    public bool NadirMi { get; set; } = false;

    /// <summary>
    /// Rozet aktif mi?
    /// </summary>
    [Display(Name = "Aktif")]
    public bool AktifMi { get; set; } = true;

    /// <summary>
    /// Sıralama (Gösterim sırası)
    /// </summary>
    [Display(Name = "Sıra")]
    public int Sira { get; set; } = 0;

    #region Navigation Properties

    public virtual ICollection<KullaniciRozet> KullaniciRozetleri { get; set; } = new List<KullaniciRozet>();

    #endregion

    #region Helper Properties

    /// <summary>
    /// Rozet kaç kullanıcı tarafından kazanıldı?
    /// </summary>
    [NotMapped]
    public int KazananSayisi => KullaniciRozetleri?.Count ?? 0;

    #endregion
}

#endregion

#region Kullanıcı Rozet Entity (Junction Table)

/// <summary>
/// KullaniciRozet - Kullanıcı ve Rozet ilişkisi
/// 
/// Bir kullanıcı birden fazla rozet kazanabilir
/// Bir rozet birden fazla kullanıcı tarafından kazanılabilir
/// </summary>
public class KullaniciRozet : AuditOnlyBaseEntity
{
    [Required]
    [Display(Name = "Kullanıcı")]
    public string KullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(KullaniciId))]
    public virtual ApplicationUser Kullanici { get; set; } = null!;

    [Required]
    [Display(Name = "Rozet")]
    public int RozetId { get; set; }

    [ForeignKey(nameof(RozetId))]
    public virtual Rozet Rozet { get; set; } = null!;

    /// <summary>
    /// Rozet kazanılma tarihi
    /// </summary>
    [Required]
    [Display(Name = "Kazanılma Tarihi")]
    public DateTime KazanilmaTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Rozet nasıl kazanıldı? (Açıklama)
    /// Örnek: "İlk projesini tamamladı"
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Kazanım Açıklaması")]
    public string? KazanimAciklamasi { get; set; }

    /// <summary>
    /// Rozet görünür mü? (Kullanıcı profilde gizleyebilir)
    /// </summary>
    [Display(Name = "Görünür")]
    public bool GorunurMu { get; set; } = true;
}

#endregion

#region Moderasyon Kaydı Entity

/// <summary>
/// ModerasyonKaydi - İçerik moderasyon kayıtları
/// 
/// README'de belirtildiği gibi:
/// - Audit Log'dan AYRI sorumluluk
/// - Hukuki iz için kullanılır
/// - Geri alınamaz kararlar
/// - Hangi içerik, hangi aksiyon, hangi gerekçe
/// 
/// Soft Delete YOKTUR (Yasal kayıt)
/// </summary>
public class ModerasyonKaydi : AuditOnlyBaseEntity
{
    #region İçerik Bilgileri (Polymorphic)

    /// <summary>
    /// Modere edilen içerik türü
    /// Örnek: "Gonderi", "Yorum", "Proje"
    /// </summary>
    [Required]
    [StringLength(50)]
    [Display(Name = "İçerik Türü")]
    public string IcerikTuru { get; set; } = string.Empty;

    /// <summary>
    /// Modere edilen içerik ID'si
    /// </summary>
    [Required]
    [Display(Name = "İçerik ID")]
    public int IcerikId { get; set; }

    /// <summary>
    /// İçerik sahibi
    /// </summary>
    [Required]
    [StringLength(450)]
    [Display(Name = "İçerik Sahibi")]
    public string IcerikSahibiId { get; set; } = string.Empty;

    [ForeignKey(nameof(IcerikSahibiId))]
    public virtual ApplicationUser IcerikSahibi { get; set; } = null!;

    #endregion

    #region Moderasyon Bilgileri

    /// <summary>
    /// Moderatör (OlusturanKullaniciId'den gelir)
    /// </summary>

    /// <summary>
    /// Yapılan aksiyon
    /// </summary>
    [Required]
    [Display(Name = "Aksiyon")]
    public ModerasyonAksiyon Aksiyon { get; set; }

    /// <summary>
    /// Moderasyon gerekçesi (ZORUNLU)
    /// </summary>
    [Required]
    [StringLength(1000)]
    [Display(Name = "Gerekçe")]
    public string Gerekce { get; set; } = string.Empty;

    /// <summary>
    /// İçeriğin orijinal hali (Yedek)
    /// Silinen içeriklerin yedeği
    /// </summary>
    [Display(Name = "Orijinal İçerik")]
    public string? OrijinalIcerik { get; set; }

    /// <summary>
    /// Moderasyon tarihi
    /// </summary>
    [Required]
    [Display(Name = "Moderasyon Tarihi")]
    public DateTime ModerasyonTarihi { get; set; } = DateTime.UtcNow;

    #endregion

    #region İhlal Bilgileri

    /// <summary>
    /// İhlal türü
    /// </summary>
    [Display(Name = "İhlal Türü")]
    public IhlalTuru? IhlalTuru { get; set; }

    /// <summary>
    /// İhlal ciddiyeti
    /// </summary>
    [Display(Name = "Ciddiyet Seviyesi")]
    public CiddiyetSeviyesi CiddiyetSeviyesi { get; set; } = CiddiyetSeviyesi.Dusuk;

    #endregion

    #region İstatistikler

    /// <summary>
    /// İçerik sahibine kaçıncı moderasyon?
    /// </summary>
    [Display(Name = "Kullanıcının N. Moderasyonu")]
    public int KullaniciModerasyonSayisi { get; set; } = 1;

    #endregion

    #region Notlar

    /// <summary>
    /// Moderatör notu (İç kullanım)
    /// </summary>
    [StringLength(1000)]
    [Display(Name = "Moderatör Notu")]
    public string? ModeratorNotu { get; set; }

    #endregion
}

#endregion

#region Audit Log Entity

/// <summary>
/// AuditLog - Sistem audit trail (Teknik iz)
/// 
/// README'de belirtildiği gibi:
/// - Moderasyon'dan AYRI sorumluluk
/// - Teknik iz için kullanılır
/// - Kim, ne zaman, neyi yaptı
/// - Gerekçe tutmaz, sadece kayıt
/// 
/// Örnekler:
/// - Kullanıcı girişi
/// - Rol değişimi
/// - İçerik oluşturma/güncelleme/silme
/// - Sistem ayarları değişikliği
/// </summary>
public class AuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// İşlemi yapan kullanıcı
    /// </summary>
    [Required]
    [StringLength(450)]
    [Display(Name = "Kullanıcı")]
    public string KullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(KullaniciId))]
    public virtual ApplicationUser Kullanici { get; set; } = null!;

    /// <summary>
    /// İşlem türü
    /// Örnek: "Create", "Update", "Delete", "Login", "Logout"
    /// </summary>
    [Required]
    [StringLength(50)]
    [Display(Name = "İşlem Türü")]
    public string IslemTuru { get; set; } = string.Empty;

    /// <summary>
    /// Etkilenen entity türü
    /// Örnek: "Proje", "Kullanici", "Gonderi"
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Entity Türü")]
    public string? EntityTuru { get; set; }

    /// <summary>
    /// Etkilenen entity ID'si
    /// </summary>
    [Display(Name = "Entity ID")]
    public int? EntityId { get; set; }

    /// <summary>
    /// İşlem açıklaması
    /// Örnek: "Proje başlığı güncellendi"
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    /// <summary>
    /// Eski değer (JSON format)
    /// </summary>
    [Display(Name = "Eski Değer")]
    public string? EskiDeger { get; set; }

    /// <summary>
    /// Yeni değer (JSON format)
    /// </summary>
    [Display(Name = "Yeni Değer")]
    public string? YeniDeger { get; set; }

    /// <summary>
    /// IP Adresi
    /// </summary>
    [StringLength(50)]
    [Display(Name = "IP Adresi")]
    public string? IpAdresi { get; set; }

    /// <summary>
    /// User Agent (Tarayıcı bilgisi)
    /// </summary>
    [StringLength(500)]
    [Display(Name = "User Agent")]
    public string? UserAgent { get; set; }

    /// <summary>
    /// İşlem tarihi
    /// </summary>
    [Required]
    [Display(Name = "İşlem Tarihi")]
    public DateTime IslemTarihi { get; set; } = DateTime.UtcNow;
}

#endregion

#region Enums

/// <summary>
/// Rozet seviyesi
/// </summary>
public enum RozetSeviye
{
    [Display(Name = "Bronz")]
    Bronz = 1,

    [Display(Name = "Gümüş")]
    Gumus = 2,

    [Display(Name = "Altın")]
    Altin = 3,

    [Display(Name = "Platin")]
    Platin = 4,

    [Display(Name = "Elmas")]
    Elmas = 5
}

/// <summary>
/// Rozet kategorisi
/// </summary>
public enum RozetKategori
{
    [Display(Name = "Başlangıç")]
    Baslangic = 0,

    [Display(Name = "Proje")]
    Proje = 1,

    [Display(Name = "Sosyal")]
    Sosyal = 2,

    [Display(Name = "Öğrenme")]
    Ogrenme = 3,

    [Display(Name = "Liderlik")]
    Liderlik = 4,

    [Display(Name = "Yaratıcılık")]
    Yaraticilik = 5,

    [Display(Name = "Özel")]
    Ozel = 99
}

/// <summary>
/// Moderasyon aksiyonu
/// </summary>
public enum ModerasyonAksiyon
{
    [Display(Name = "Uyarı")]
    Uyari = 0,

    [Display(Name = "Gizleme")]
    Gizleme = 1,

    [Display(Name = "Silme")]
    Silme = 2,

    [Display(Name = "Düzenleme")]
    Duzenleme = 3,

    [Display(Name = "Hesap Askıya Alma")]
    HesapAskiyaAlma = 4,

    [Display(Name = "Hesap Kapama")]
    HesapKapama = 5
}

/// <summary>
/// İhlal türü
/// </summary>
public enum IhlalTuru
{
    [Display(Name = "Spam")]
    Spam = 0,

    [Display(Name = "Küfür/Hakaret")]
    KufurHakaret = 1,

    [Display(Name = "Nefret Söylemi")]
    NefretSoylemi = 2,

    [Display(Name = "Dolandırıcılık")]
    Dolandiricilik = 3,

    [Display(Name = "Telif İhlali")]
    TelifIhlali = 4,

    [Display(Name = "Yanıltıcı Bilgi")]
    YaniltıcıBilgi = 5,

    [Display(Name = "Şiddet")]
    Siddet = 6,

    [Display(Name = "Uygunsuz İçerik")]
    UygunsuzIcerik = 7,

    [Display(Name = "Diğer")]
    Diger = 99
}

/// <summary>
/// Ciddiyet seviyesi
/// </summary>
public enum CiddiyetSeviyesi
{
    [Display(Name = "Düşük")]
    Dusuk = 0,

    [Display(Name = "Orta")]
    Orta = 1,

    [Display(Name = "Yüksek")]
    Yuksek = 2,

    [Display(Name = "Kritik")]
    Kritik = 3
}

#endregion