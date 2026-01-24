using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YenilikciEgitimPlatformu.Models.Identity;

namespace YenilikciEgitimPlatformu.Models;

/*
 * Sosyal İçerik Entity Modelleri
 * 
 * Platform'un sosyal medya benzeri özelliklerini sağlar:
 * - Gonderi: Kullanıcı paylaşımları
 * - Yorum: Gönderi ve proje yorumları (nested comments destekli)
 * - Begeni: Beğeni sistemi (polymorphic)
 * - Dosya: Medya yükleme sistemi
 */

#region Gonderi Entity (Post)

/// <summary>
/// Gonderi - Kullanıcı paylaşımları
/// 
/// Özellikler:
/// - Metin içerik
/// - Medya ekleri (resim, video)
/// - Yorum sistemi
/// - Beğeni sistemi
/// - Hashtag desteği
/// </summary>
public class Gonderi : BaseEntity
{
    [Required(ErrorMessage = "Gönderi içeriği zorunludur")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "İçerik 1-5000 karakter arasında olmalıdır")]
    [Display(Name = "İçerik")]
    [DataType(DataType.MultilineText)]
    public string Icerik { get; set; } = string.Empty;

    /// <summary>
    /// Gönderi türü
    /// </summary>
    [Required]
    [Display(Name = "Gönderi Türü")]
    public GonderiTuru Turu { get; set; } = GonderiTuru.Metin;

    /// <summary>
    /// Medya URL'leri (Virgülle ayrılmış)
    /// Örnek: "image1.jpg,image2.jpg,video.mp4"
    /// </summary>
    [StringLength(2000)]
    [Display(Name = "Medya URL'leri")]
    public string? MedyaUrlleri { get; set; }

    /// <summary>
    /// Medya URL listesi
    /// </summary>
    [NotMapped]
    public List<string> MedyaListesi =>
        string.IsNullOrWhiteSpace(MedyaUrlleri)
            ? new List<string>()
            : MedyaUrlleri.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(m => m.Trim())
                         .ToList();

    /// <summary>
    /// Hashtag'ler (virgülle ayrılmış)
    /// Örnek: "#egitim,#teknoloji,#proje"
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Hashtag'ler")]
    public string? Hashtagler { get; set; }

    /// <summary>
    /// Hashtag listesi
    /// </summary>
    [NotMapped]
    public List<string> HashtagListesi =>
        string.IsNullOrWhiteSpace(Hashtagler)
            ? new List<string>()
            : Hashtagler.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(h => h.Trim())
                       .ToList();

    /// <summary>
    /// İlişkili proje yönetimi (varsa)
    /// Kullanıcı bir gönderiyi proje ile ilişkilendirebilir
    /// </summary>
    [Display(Name = "İlişkili Proje")]
    public int? ProjeYonetimId { get; set; }

    [ForeignKey(nameof(ProjeYonetimId))]
    public virtual ProjeYonetimi? ProjeYonetim { get; set; }

    #region İstatistikler

    [Display(Name = "Beğeni Sayısı")]
    public int BegeniSayisi { get; set; } = 0;

    [Display(Name = "Yorum Sayısı")]
    public int YorumSayisi { get; set; } = 0;

    [Display(Name = "Paylaşım Sayısı")]
    public int PaylasimSayisi { get; set; } = 0;

    [Display(Name = "Görüntülenme Sayısı")]
    public int GoruntulenmeSayisi { get; set; } = 0;

    #endregion

    #region Durum ve Yayın Kontrolü

    /// <summary>
    /// Gönderi yayında mı?
    /// </summary>
    [Display(Name = "Yayında")]
    public bool YayindaMi { get; set; } = true;

    /// <summary>
    /// Gönderi sabitlendirme tarihi (Öne çıkan gönderiler için)
    /// </summary>
    [Display(Name = "Sabitlenme Tarihi")]
    public DateTime? SabitlenmeTarihi { get; set; }

    /// <summary>
    /// Gönderi sabitli mi?
    /// </summary>
    [NotMapped]
    public bool SabitliMi => SabitlenmeTarihi.HasValue;

    #endregion

    #region Navigation Properties

    public virtual ICollection<Yorum> Yorumlar { get; set; } = new List<Yorum>();
    public virtual ICollection<Begeni> Begeniler { get; set; } = new List<Begeni>();

    #endregion
}

#endregion

#region Yorum Entity (Comment)

/// <summary>
/// Yorum - Gönderi ve proje yorumları
/// 
/// Özellikler:
/// - Nested comments (Alt yorumlar)
/// - Polymorphic (Farklı entity'lere yorum yapılabilir)
/// - Beğeni sistemi
/// - Moderasyon
/// </summary>
public class Yorum : BaseEntity
{
    [Required(ErrorMessage = "Yorum içeriği zorunludur")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Yorum 1-1000 karakter arasında olmalıdır")]
    [Display(Name = "Yorum")]
    [DataType(DataType.MultilineText)]
    public string Icerik { get; set; } = string.Empty;

    #region Polymorphic İlişki (Hangi Entity'ye Yorum Yapıldı?)

    /// <summary>
    /// Yorum yapılan entity türü
    /// Örnek: "Gonderi", "Proje"
    /// </summary>
    [Required]
    [StringLength(50)]
    [Display(Name = "Yorum Türü")]
    public string YorumTuru { get; set; } = string.Empty;

    /// <summary>
    /// Yorum yapılan entity'nin ID'si
    /// </summary>
    [Required]
    [Display(Name = "İlişkili Entity ID")]
    public int IliskiliEntityId { get; set; }

    #endregion

    #region Nested Comment (Alt Yorum)

    /// <summary>
    /// Üst yorum ID'si (Alt yorum ise)
    /// Null ise ana yorum
    /// </summary>
    [Display(Name = "Üst Yorum")]
    public int? UstYorumId { get; set; }

    [ForeignKey(nameof(UstYorumId))]
    public virtual Yorum? UstYorum { get; set; }

    /// <summary>
    /// Alt yorumlar
    /// </summary>
    public virtual ICollection<Yorum> AltYorumlar { get; set; } = new List<Yorum>();

    #endregion

    #region İstatistikler

    [Display(Name = "Beğeni Sayısı")]
    public int BegeniSayisi { get; set; } = 0;

    #endregion

    #region Moderasyon

    /// <summary>
    /// Yorum onaylandı mı? (Moderasyon için)
    /// </summary>
    [Display(Name = "Onaylandı")]
    public bool OnaylandiMi { get; set; } = true;

    /// <summary>
    /// Moderatör tarafından işaretlendi mi?
    /// </summary>
    [Display(Name = "İşaretli")]
    public bool IsaretliMi { get; set; } = false;

    #endregion

    #region Navigation Properties

    public virtual ICollection<Begeni> Begeniler { get; set; } = new List<Begeni>();

    #endregion

    #region Helper Properties

    /// <summary>
    /// Bu yorum ana yorum mu? (Alt yorum değil)
    /// </summary>
    [NotMapped]
    public bool AnaYorumMu => UstYorumId == null;

    /// <summary>
    /// Alt yorum sayısı
    /// </summary>
    [NotMapped]
    public int AltYorumSayisi => AltYorumlar?.Count ?? 0;

    #endregion
}

#endregion

#region Begeni Entity (Like)

/// <summary>
/// Begeni - Beğeni sistemi
/// 
/// Polymorphic yapı: Farklı entity'ler beğenilebilir
/// - Gonderi
/// - Yorum
/// - Proje
/// 
/// NOT: AuditOnlyBaseEntity kullanılmıyor (Cascade path çakışması önleme)
/// </summary>
public class Begeni
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    #region Polymorphic İlişki

    /// <summary>
    /// Beğenilen entity türü
    /// Örnek: "Gonderi", "Yorum", "Proje"
    /// </summary>
    [Required]
    [StringLength(50)]
    [Display(Name = "Beğeni Türü")]
    public string BegeniTuru { get; set; } = string.Empty;

    /// <summary>
    /// Beğenilen entity'nin ID'si
    /// </summary>
    [Required]
    [Display(Name = "İlişkili Entity ID")]
    public int IliskiliEntityId { get; set; }

    #endregion

    #region Beğeni Kullanıcısı

    [Required]
    [StringLength(450)]
    public string KullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(KullaniciId))]
    public virtual ApplicationUser Kullanici { get; set; } = null!;

    #endregion

    [Required]
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

    #region Helper Properties

    /// <summary>
    /// Beğeni tarihi (OlusturulmaTarihi)
    /// </summary>
    [NotMapped]
    public DateTime BegeniTarihi => OlusturulmaTarihi;

    #endregion
}

#endregion

#region Dosya Entity (File)

/// <summary>
/// Dosya - Medya ve doküman yükleme sistemi
/// 
/// Kullanım alanları:
/// - Proje dokümanları
/// - Gönderi medyaları
/// - Profil fotoğrafları
/// - Duyuru ekleri
/// </summary>
public class Dosya : BaseEntity
{
    [Required]
    [StringLength(255)]
    [Display(Name = "Dosya Adı")]
    public string DosyaAdi { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Display(Name = "Orijinal Dosya Adı")]
    public string OrijinalDosyaAdi { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "Dosya Yolu")]
    public string DosyaYolu { get; set; } = string.Empty;

    /// <summary>
    /// Dosya URL'si (Public erişim için)
    /// </summary>
    [Required]
    [StringLength(500)]
    [Display(Name = "Dosya URL")]
    public string DosyaUrl { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "MIME Type")]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Dosya boyutu (byte cinsinden)
    /// </summary>
    [Required]
    [Display(Name = "Dosya Boyutu (Byte)")]
    public long DosyaBoyutu { get; set; }

    /// <summary>
    /// Dosya türü
    /// </summary>
    [Required]
    [Display(Name = "Dosya Türü")]
    public DosyaTuru Turu { get; set; }

    #region Polymorphic İlişki (Hangi Entity'ye Ait?)

    /// <summary>
    /// İlişkili entity türü
    /// Örnek: "Proje", "Gonderi", "Kullanici"
    /// </summary>
    [StringLength(50)]
    [Display(Name = "İlişkili Entity Türü")]
    public string? IliskiliEntityTuru { get; set; }

    /// <summary>
    /// İlişkili entity ID'si
    /// </summary>
    [Display(Name = "İlişkili Entity ID")]
    public int? IliskiliEntityId { get; set; }

    #endregion

    #region Resim Özel Alanlar

    /// <summary>
    /// Resim genişliği (pixel)
    /// </summary>
    [Display(Name = "Genişlik")]
    public int? Genislik { get; set; }

    /// <summary>
    /// Resim yüksekliği (pixel)
    /// </summary>
    [Display(Name = "Yükseklik")]
    public int? Yukseklik { get; set; }

    /// <summary>
    /// Thumbnail URL (Küçük boyut)
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Thumbnail URL")]
    public string? ThumbnailUrl { get; set; }

    #endregion

    #region İstatistikler

    [Display(Name = "İndirilme Sayısı")]
    public int IndiriliSayisi { get; set; } = 0;

    #endregion

    #region Helper Properties

    /// <summary>
    /// Dosya boyutu human-readable format
    /// Örnek: "2.5 MB"
    /// </summary>
    [NotMapped]
    public string DosyaBoyutuFormatli
    {
        get
        {
            string[] boyutlar = { "B", "KB", "MB", "GB", "TB" };
            double boyut = DosyaBoyutu;
            int sira = 0;

            while (boyut >= 1024 && sira < boyutlar.Length - 1)
            {
                boyut /= 1024;
                sira++;
            }

            return $"{boyut:0.##} {boyutlar[sira]}";
        }
    }

    /// <summary>
    /// Dosya uzantısı
    /// </summary>
    [NotMapped]
    public string Uzanti => Path.GetExtension(DosyaAdi).ToLowerInvariant();

    /// <summary>
    /// Dosya resim mi?
    /// </summary>
    [NotMapped]
    public bool ResimMi => Turu == DosyaTuru.Resim;

    /// <summary>
    /// Dosya video mu?
    /// </summary>
    [NotMapped]
    public bool VideoMu => Turu == DosyaTuru.Video;

    #endregion
}

#endregion

#region Enums

public enum GonderiTuru
{
    [Display(Name = "Metin")]
    Metin = 0,

    [Display(Name = "Resim")]
    Resim = 1,

    [Display(Name = "Video")]
    Video = 2,

    [Display(Name = "Link")]
    Link = 3,

    [Display(Name = "Karma (Metin + Medya)")]
    Karma = 4
}

public enum DosyaTuru
{
    [Display(Name = "Resim")]
    Resim = 0,

    [Display(Name = "Video")]
    Video = 1,

    [Display(Name = "Doküman")]
    Dokuman = 2,

    [Display(Name = "Arşiv")]
    Arsiv = 3,

    [Display(Name = "Diğer")]
    Diger = 99
}

#endregion