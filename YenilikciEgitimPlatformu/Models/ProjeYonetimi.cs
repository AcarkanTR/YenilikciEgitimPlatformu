using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YenilikciEgitimPlatformu.Models.Identity;

namespace YenilikciEgitimPlatformu.Models;

/*
 * ════════════════════════════════════════════════════════════════════════════
 * SİSTEM 2: PROJE YÖNETİMİ SİSTEMİ
 * ════════════════════════════════════════════════════════════════════════════
 * 
 * Amaç: Kullanıcıların kendi projelerini oluşturması ve yönetmesi
 * Sahip: Kullanıcılar
 * Özellikler: Ekip kurma, görev atama, aktivite paylaşımı, ilerleme takibi
 * 
 * Örnekler:
 * - "Okulumuzda Geri Dönüşüm Projesi"
 * - "Mobil Uygulama Geliştirme Ekibi"
 * - "Bilim Fuarı Hazırlığı"
 */

#region Ana Entity

/// <summary>
/// ProjeYonetimi - Kullanıcı projeleri
/// 
/// Bu model kullanıcıların oluşturduğu, yönettiği projeleri temsil eder.
/// CagriBilgisi'nden TAMAMEN BAĞIMSIZDIR (opsiyonel bağlantı hariç).
/// </summary>
public class ProjeYonetimi : BaseEntity
{
    #region Temel Bilgiler

    /// <summary>
    /// Proje adı
    /// </summary>
    [Required]
    [StringLength(200)]
    [Display(Name = "Proje Adı")]
    public string ProjeAdi { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly başlık
    /// </summary>
    [StringLength(250)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Kısa açıklama
    /// </summary>
    [Required]
    [StringLength(500)]
    [Display(Name = "Kısa Açıklama")]
    public string KisaAciklama { get; set; } = string.Empty;

    /// <summary>
    /// Detaylı proje açıklaması
    /// </summary>
    [Display(Name = "Proje Açıklaması")]
    public string ProjeAciklamasi { get; set; } = string.Empty;

    /// <summary>
    /// Proje kapak görseli
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Kapak Görseli")]
    public string? KapakGorseliUrl { get; set; }

    #endregion

    #region Sahiplik

    /// <summary>
    /// Projeyi oluşturan kullanıcı (Kurucu)
    /// 
    /// NOT: OlusturanKullaniciId BaseEntity'den gelir ama
    /// KurucuKullaniciId daha açık bir ifade
    /// </summary>
    [Required]
    [StringLength(450)]
    [Display(Name = "Kurucu")]
    public string KurucuKullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(KurucuKullaniciId))]
    public virtual ApplicationUser Kurucu { get; set; } = null!;

    #endregion

    #region Proje Döngüsü (Project Lifecycle)

    /// <summary>
    /// Projenin amacı
    /// Örnek: "Okulda atık miktarını %50 azaltmak"
    /// </summary>
    [StringLength(1000)]
    [Display(Name = "Amaç")]
    public string? Amac { get; set; }

    /// <summary>
    /// Hedefler (Detaylı)
    /// </summary>
    [Display(Name = "Hedefler")]
    public string? Hedefler { get; set; }

    /// <summary>
    /// Yöntem / Metodoloji
    /// </summary>
    [Display(Name = "Yöntem")]
    public string? Yontem { get; set; }

    /// <summary>
    /// Beklenen çıktı
    /// </summary>
    [Display(Name = "Beklenen Çıktı")]
    public string? BeklenenCikti { get; set; }

    #endregion

    #region Kategori ve Etiketler

    /// <summary>
    /// Proje kategorisi
    /// </summary>
    [Display(Name = "Kategori")]
    public int? KategoriId { get; set; }

    [ForeignKey(nameof(KategoriId))]
    public virtual ProjeKategori? Kategori { get; set; }

    /// <summary>
    /// Etiketler (JSON array veya comma-separated)
    /// Örnek: ["robotik", "yapay-zeka", "eğitim"]
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Etiketler")]
    public string? Etiketler { get; set; }

    #endregion

    #region Ekip Yönetimi

    /// <summary>
    /// Maksimum katılımcı sayısı
    /// </summary>
    [Display(Name = "Maksimum Ekip Üyesi")]
    public int MaxKatilimciSayisi { get; set; } = 10;

    /// <summary>
    /// Mevcut katılımcı sayısı (Computed)
    /// </summary>
    [NotMapped]
    public int MevcutKatilimciSayisi => EkipUyeleri?.Count(e => e.AktifMi) ?? 0;

    /// <summary>
    /// Ekip dolu mu?
    /// </summary>
    [NotMapped]
    public bool EkipDoluMu => MevcutKatilimciSayisi >= MaxKatilimciSayisi;

    #endregion

    #region Proje Durumu ve Tarihler

    /// <summary>
    /// Proje durumu
    /// </summary>
    [Required]
    [Display(Name = "Durum")]
    public ProjeDurumu Durum { get; set; } = ProjeDurumu.FikirAsamasi;

    /// <summary>
    /// Proje başlangıç tarihi
    /// </summary>
    [Display(Name = "Başlangıç Tarihi")]
    public DateTime? BaslangicTarihi { get; set; }

    /// <summary>
    /// Hedef bitiş tarihi
    /// </summary>
    [Display(Name = "Hedef Bitiş Tarihi")]
    public DateTime? HedefBitisTarihi { get; set; }

    /// <summary>
    /// Gerçek bitiş tarihi
    /// </summary>
    [Display(Name = "Tamamlanma Tarihi")]
    public DateTime? TamamlanmaTarihi { get; set; }

    /// <summary>
    /// İlerleme yüzdesi (0-100)
    /// </summary>
    [Range(0, 100)]
    [Display(Name = "İlerleme (%)")]
    public int IlerlemeYuzdesi { get; set; } = 0;

    #endregion

    #region Görünürlük ve Ayarlar

    /// <summary>
    /// Proje herkese açık mı?
    /// </summary>
    [Display(Name = "Herkese Açık")]
    public bool HerkeseAcikMi { get; set; } = true;

    /// <summary>
    /// Yeni üye kabulü açık mı?
    /// </summary>
    [Display(Name = "Yeni Üye Kabulü Açık")]
    public bool YeniUyeKabuluAcikMi { get; set; } = true;

    /// <summary>
    /// Proje yayında mı?
    /// </summary>
    [Display(Name = "Yayında")]
    public bool YayindaMi { get; set; } = true;

    #endregion

    #region Kaynak Çağrı Bağlantısı (Opsiyonel)

    /// <summary>
    /// Bu proje bir CagriBilgisi için mi oluşturuldu?
    /// 
    /// Örnek: "Bu proje TÜBİTAK 2204-A yarışması için oluşturuldu"
    /// 
    /// NULL ise: Bağımsız proje
    /// DOLU ise: İlgili çağrı için hazırlanan proje
    /// </summary>
    [Display(Name = "Kaynak Çağrı")]
    public int? KaynakCagriBilgisiId { get; set; }

    [ForeignKey(nameof(KaynakCagriBilgisiId))]
    public virtual CagriBilgisi? KaynakCagriBilgisi { get; set; }

    #endregion

    #region Okul Bağlantısı (Opsiyonel)

    /// <summary>
    /// Proje bir okula bağlı mı?
    /// </summary>
    [Display(Name = "Okul")]
    public int? OkulId { get; set; }

    [ForeignKey(nameof(OkulId))]
    public virtual Okul? Okul { get; set; }

    #endregion

    #region İstatistikler

    [Display(Name = "Görüntülenme Sayısı")]
    public int GoruntulenmeSayisi { get; set; } = 0;

    [Display(Name = "Beğeni Sayısı")]
    public int BegeniSayisi { get; set; } = 0;

    [Display(Name = "Yorum Sayısı")]
    public int YorumSayisi { get; set; } = 0;

    [Display(Name = "Paylaşım Sayısı")]
    public int PaylasimSayisi { get; set; } = 0;

    #endregion

    #region İlişkiler

    /// <summary>
    /// Ekip üyeleri
    /// </summary>
    public virtual ICollection<ProjeEkipUyesi> EkipUyeleri { get; set; } = new List<ProjeEkipUyesi>();

    /// <summary>
    /// Proje görevleri
    /// </summary>
    public virtual ICollection<ProjeGorevi> Gorevler { get; set; } = new List<ProjeGorevi>();

    /// <summary>
    /// Proje aktiviteleri (Sprint, toplantı, demo vb.)
    /// </summary>
    public virtual ICollection<ProjeAktivitesi> Aktiviteler { get; set; } = new List<ProjeAktivitesi>();

    /// <summary>
    /// Proje dosyaları
    /// </summary>
    public virtual ICollection<ProjeDosya> Dosyalar { get; set; } = new List<ProjeDosya>();

    #endregion

    #region Helper Properties

    /// <summary>
    /// Proje süresi (Gün)
    /// </summary>
    [NotMapped]
    public int? ProjeSuresiGun
    {
        get
        {
            if (!BaslangicTarihi.HasValue || !HedefBitisTarihi.HasValue)
                return null;

            return (HedefBitisTarihi.Value - BaslangicTarihi.Value).Days;
        }
    }

    /// <summary>
    /// Proje gecikmede mi?
    /// </summary>
    [NotMapped]
    public bool GecikmedeMi
    {
        get
        {
            if (!HedefBitisTarihi.HasValue || Durum == ProjeDurumu.Tamamlandi)
                return false;

            return DateTime.UtcNow > HedefBitisTarihi.Value;
        }
    }

    /// <summary>
    /// Aktif görev sayısı
    /// </summary>
    [NotMapped]
    public int AktifGorevSayisi => Gorevler?.Count(g => g.Durum != GorevDurumu.Tamamlandi && g.Durum != GorevDurumu.IptalEdildi) ?? 0;

    #endregion
}

#endregion

#region Ekip Üyeleri

/// <summary>
/// ProjeEkipUyesi - Proje ve Kullanıcı arasında Many-to-Many ilişki
/// 
/// NOT: AuditOnlyBaseEntity kullanılmıyor (Cascade path çakışması önleme)
/// </summary>
public class ProjeEkipUyesi
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Proje")]
    public int ProjeYonetimId { get; set; }

    [ForeignKey(nameof(ProjeYonetimId))]
    public virtual ProjeYonetimi ProjeYonetim { get; set; } = null!;

    [Required]
    [StringLength(450)]
    public string KullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(KullaniciId))]
    public virtual ApplicationUser Kullanici { get; set; } = null!;

    /// <summary>
    /// Ekip üyesinin rolü
    /// </summary>
    [Required]
    [Display(Name = "Rol")]
    public ProjeRol Rol { get; set; } = ProjeRol.Uye;

    /// <summary>
    /// Üyenin özel rol başlığı
    /// Örnek: "Backend Developer", "UI Designer", "Araştırmacı"
    /// </summary>
    [StringLength(100)]
    [Display(Name = "Özel Rol")]
    public string? OzelRol { get; set; }

    [Display(Name = "Katılma Tarihi")]
    public DateTime KatilmaTarihi { get; set; } = DateTime.UtcNow;

    [Display(Name = "Ayrılma Tarihi")]
    public DateTime? AyrilmaTarihi { get; set; }

    [Display(Name = "Aktif Mi?")]
    public bool AktifMi { get; set; } = true;

    /// <summary>
    /// Kurucu mu? (Projeyi oluşturan)
    /// </summary>
    [Display(Name = "Kurucu")]
    public bool KurucuMu { get; set; } = false;

    [Required]
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
}

#endregion

#region Görevler

/// <summary>
/// ProjeGorevi - Proje görevleri (Task management)
/// </summary>
public class ProjeGorevi : BaseEntity
{
    [Required]
    public int ProjeYonetimId { get; set; }

    [ForeignKey(nameof(ProjeYonetimId))]
    public virtual ProjeYonetimi ProjeYonetim { get; set; } = null!;

    [Required]
    [StringLength(200)]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    /// <summary>
    /// Görevi atayan kullanıcı (OlusturanKullaniciId BaseEntity'den gelir)
    /// </summary>

    /// <summary>
    /// Görevin atandığı kullanıcı
    /// </summary>
    [StringLength(450)]
    [Display(Name = "Atanan Kullanıcı")]
    public string? AtananKullaniciId { get; set; }

    [ForeignKey(nameof(AtananKullaniciId))]
    public virtual ApplicationUser? AtananKullanici { get; set; }

    [Required]
    [Display(Name = "Durum")]
    public GorevDurumu Durum { get; set; } = GorevDurumu.Yapilacak;

    [Required]
    [Display(Name = "Öncelik")]
    public GorevOncelik Oncelik { get; set; } = GorevOncelik.Normal;

    [Display(Name = "Başlangıç Tarihi")]
    public DateTime? BaslangicTarihi { get; set; }

    [Display(Name = "Bitiş Tarihi")]
    public DateTime? BitisTarihi { get; set; }

    [Display(Name = "Son Teslim Tarihi")]
    public DateTime? SonTeslimTarihi { get; set; }

    [Range(0, 100)]
    [Display(Name = "İlerleme (%)")]
    public int IlerlemeYuzdesi { get; set; } = 0;

    /// <summary>
    /// Tahmini süre (Saat)
    /// </summary>
    [Display(Name = "Tahmini Süre (Saat)")]
    public int? TahminiSure { get; set; }

    /// <summary>
    /// Gerçekleşen süre (Saat)
    /// </summary>
    [Display(Name = "Gerçekleşen Süre (Saat)")]
    public int? GerceklesenSure { get; set; }

    #region Helper Properties

    [NotMapped]
    public bool GeciktiMi
    {
        get
        {
            if (!SonTeslimTarihi.HasValue || Durum == GorevDurumu.Tamamlandi)
                return false;

            return DateTime.UtcNow > SonTeslimTarihi.Value;
        }
    }

    #endregion
}

#endregion

#region Aktiviteler

/// <summary>
/// ProjeAktivitesi - Proje aktiviteleri
/// (Sprint, toplantı, demo, paylaşım vb.)
/// </summary>
public class ProjeAktivitesi : BaseEntity
{
    [Required]
    public int ProjeYonetimId { get; set; }

    [ForeignKey(nameof(ProjeYonetimId))]
    public virtual ProjeYonetimi ProjeYonetim { get; set; } = null!;

    [Required]
    [Display(Name = "Aktivite Türü")]
    public ProjeAktiviteTuru Tur { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    /// <summary>
    /// Aktivite tarihi
    /// </summary>
    [Display(Name = "Aktivite Tarihi")]
    public DateTime? AktiviteTarihi { get; set; }

    /// <summary>
    /// Aktivite yeri (Toplantı için)
    /// </summary>
    [StringLength(200)]
    [Display(Name = "Yer")]
    public string? Yer { get; set; }

    /// <summary>
    /// Online link (Online toplantı için)
    /// </summary>
    [StringLength(500)]
    [Display(Name = "Online Link")]
    public string? OnlineLink { get; set; }

    /// <summary>
    /// Katılımcı ID'leri (JSON array)
    /// </summary>
    [Display(Name = "Katılımcılar")]
    public string? KatilimciIds { get; set; }
}

#endregion

#region Dosyalar

/// <summary>
/// ProjeDosya - Proje dosyaları
/// </summary>
public class ProjeDosya : BaseEntity
{
    [Required]
    public int ProjeYonetimId { get; set; }

    [ForeignKey(nameof(ProjeYonetimId))]
    public virtual ProjeYonetimi ProjeYonetim { get; set; } = null!;

    [Required]
    [StringLength(200)]
    [Display(Name = "Dosya Adı")]
    public string DosyaAdi { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "Dosya Yolu")]
    public string DosyaYolu { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Dosya Türü")]
    public string? DosyaTuru { get; set; }

    [Display(Name = "Dosya Boyutu (Byte)")]
    public long DosyaBoyutu { get; set; }

    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    /// <summary>
    /// Dosya kategorisi
    /// Örnek: "Sunum", "Rapor", "Kod", "Tasarım"
    /// </summary>
    [StringLength(100)]
    [Display(Name = "Kategori")]
    public string? Kategori { get; set; }

    /// <summary>
    /// Versiyon numarası
    /// </summary>
    [StringLength(50)]
    [Display(Name = "Versiyon")]
    public string? Versiyon { get; set; }
}

#endregion

#region Proje Kategori (Sistem 2 için)

/// <summary>
/// ProjeKategori - Proje kategorileri
/// 
/// NOT: Bu CagriBilgisi kategorilerinden AYRI!
/// Kullanıcı projelerine özgü kategoriler
/// </summary>
public class ProjeKategori : BaseEntity
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Kategori Adı")]
    public string Ad { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [StringLength(50)]
    [Display(Name = "İkon")]
    public string? Ikon { get; set; }

    [StringLength(50)]
    [Display(Name = "Renk")]
    public string? Renk { get; set; }

    [Display(Name = "Sıra")]
    public int Sira { get; set; } = 0;

    [Display(Name = "Aktif")]
    public bool AktifMi { get; set; } = true;

    public virtual ICollection<ProjeYonetimi> Projeler { get; set; } = new List<ProjeYonetimi>();
}

#endregion

#region Enum'lar

/// <summary>
/// Proje durumu
/// </summary>
public enum ProjeDurumu
{
    [Display(Name = "Fikir Aşaması")]
    FikirAsamasi = 1,

    [Display(Name = "Planlama")]
    Planlama = 2,

    [Display(Name = "Devam Ediyor")]
    DevamEdiyor = 3,

    [Display(Name = "Askıda")]
    Askida = 4,

    [Display(Name = "Tamamlandı")]
    Tamamlandi = 5,

    [Display(Name = "İptal Edildi")]
    IptalEdildi = 6
}

/// <summary>
/// Proje rol
/// </summary>
public enum ProjeRol
{
    [Display(Name = "Kurucu")]
    Kurucu = 1,

    [Display(Name = "Yönetici")]
    Yonetici = 2,

    [Display(Name = "Üye")]
    Uye = 3,

    [Display(Name = "Gözlemci")]
    Gozlemci = 4
}

/// <summary>
/// Görev durumu
/// </summary>
public enum GorevDurumu
{
    [Display(Name = "Yapılacak")]
    Yapilacak = 1,

    [Display(Name = "Devam Ediyor")]
    DevamEdiyor = 2,

    [Display(Name = "Beklemede")]
    Beklemede = 3,

    [Display(Name = "Tamamlandı")]
    Tamamlandi = 4,

    [Display(Name = "İptal Edildi")]
    IptalEdildi = 5
}

/// <summary>
/// Görev öncelik
/// </summary>
public enum GorevOncelik
{
    [Display(Name = "Düşük")]
    Dusuk = 1,

    [Display(Name = "Normal")]
    Normal = 2,

    [Display(Name = "Yüksek")]
    Yuksek = 3,

    [Display(Name = "Acil")]
    Acil = 4
}

/// <summary>
/// Proje aktivite türü
/// </summary>
public enum ProjeAktiviteTuru
{
    [Display(Name = "Sprint")]
    Sprint = 1,

    [Display(Name = "Toplantı")]
    Toplanti = 2,

    [Display(Name = "Demo Günü")]
    DemoGunu = 3,

    [Display(Name = "Paylaşım")]
    Paylasim = 4,

    [Display(Name = "Milestone")]
    Milestone = 5,

    [Display(Name = "Review")]
    Review = 6
}

#endregion