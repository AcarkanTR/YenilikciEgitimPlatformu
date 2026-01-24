using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YenilikciEgitimPlatformu.Models.Identity;

/*
 * ApplicationUser - Genişletilmiş Identity Kullanıcı Modeli
 * 
 * Bu sınıf ASP.NET Core Identity'nin varsayılan IdentityUser sınıfını genişletir.
 * 
 * Identity'den Gelen Özellikler (Inherit):
 * - Id: Kullanıcı benzersiz kimliği (GUID)
 * - UserName: Kullanıcı adı (unique)
 * - Email: E-posta adresi (unique)
 * - EmailConfirmed: E-posta doğrulandı mı?
 * - PasswordHash: Hash'lenmiş şifre
 * - PhoneNumber: Telefon numarası
 * - PhoneNumberConfirmed: Telefon doğrulandı mı?
 * - TwoFactorEnabled: 2FA aktif mi?
 * - LockoutEnd: Hesap ne zamana kadar kilitli?
 * - LockoutEnabled: Kilitleme aktif mi?
 * - AccessFailedCount: Başarısız giriş denemesi sayısı
 * 
 * Eklenen Özellikler:
 * - Profil bilgileri (Ad, Soyad, Bio, Avatar vs.)
 * - Okul ve konum bilgileri
 * - Kullanıcı tipi (Öğretmen/Öğrenci/Standart)
 * - Oyunlaştırma (XP, Seviye, Rozetler)
 * - İlişkiler (Projeler, Gönderiler, Yorumlar vs.)
 * 
 * ÖNEMLİ: "Kullanıcı Tipi" bir ROL değildir!
 * - Roller: Admin, Moderator, User (Yetkilendirme için)
 * - Kullanıcı Tipi: Öğretmen, Öğrenci, Standart (Profil özelliği)
 */

#region Ana Kullanıcı Modeli

public class ApplicationUser : IdentityUser
{
    #region Temel Profil Bilgileri
    /*
     * Kullanıcının temel kimlik bilgileri
     * Bu alanlar profile tamamlama oranını etkiler
     */

    [Required(ErrorMessage = "Ad zorunludur")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
    [Display(Name = "Ad")]
    public string Ad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
    [Display(Name = "Soyad")]
    public string Soyad { get; set; } = string.Empty;

    /// <summary>
    /// Tam ad - Read-only property (Ad + Soyad)
    /// </summary>
    [NotMapped]
    [Display(Name = "Ad Soyad")]
    public string TamAd => $"{Ad} {Soyad}";

    [StringLength(500, ErrorMessage = "Biyografi en fazla 500 karakter olabilir")]
    [Display(Name = "Biyografi")]
    public string? Bio { get; set; }

    [StringLength(255)]
    [Display(Name = "Profil Fotoğrafı")]
    public string? ProfilFotografiUrl { get; set; }

    [StringLength(255)]
    [Display(Name = "Kapak Fotoğrafı")]
    public string? KapakFotografiUrl { get; set; }

    #endregion

    #region Kullanıcı Tipi ve Okul Bilgileri
    /*
     * Kullanıcı tipi bir ENUM'dur ve ROL değildir!
     * 
     * Roller (Identity): Yetkilendirme için kullanılır
     * Kullanıcı Tipi: Profil özelliği ve filtreleme için kullanılır
     */

    [Required]
    [Display(Name = "Kullanıcı Tipi")]
    public KullaniciTipi KullaniciTipi { get; set; } = KullaniciTipi.Standart;

    // Okul bilgisi (Öğretmen ve Öğrenci için)
    [Display(Name = "Okul")]
    public int? OkulId { get; set; }

    [ForeignKey(nameof(OkulId))]
    public virtual Okul? Okul { get; set; }

    // Branş bilgisi (Öğretmen için)
    [StringLength(100)]
    [Display(Name = "Branş")]
    public string? Brans { get; set; }

    // Sınıf bilgisi (Öğrenci için)
    [StringLength(50)]
    [Display(Name = "Sınıf")]
    public string? Sinif { get; set; }

    // Öğrenci numarası (Öğrenci için)
    [StringLength(50)]
    [Display(Name = "Öğrenci No")]
    public string? OgrenciNo { get; set; }

    #endregion

    #region İletişim ve Konum Bilgileri

    [Display(Name = "İl")]
    public int? IlId { get; set; }

    [ForeignKey(nameof(IlId))]
    public virtual Il? Il { get; set; }

    [Display(Name = "İlçe")]
    public int? IlceId { get; set; }

    [ForeignKey(nameof(IlceId))]
    public virtual Ilce? Ilce { get; set; }

    [StringLength(500)]
    [Display(Name = "Adres")]
    public string? Adres { get; set; }

    [StringLength(255)]
    [Display(Name = "Website")]
    public string? WebsiteUrl { get; set; }

    [StringLength(100)]
    [Display(Name = "Twitter")]
    public string? TwitterHandle { get; set; }

    [StringLength(100)]
    [Display(Name = "LinkedIn")]
    public string? LinkedInUrl { get; set; }

    #endregion

    #region Oyunlaştırma (Gamification)
    /*
     * Kullanıcı XP ve seviye sistemi
     * 
     * XP Kazanım Mekanizması (Event-Based):
     * - İlk gönderi: +10 XP
     * - İlk proje: +50 XP
     * - Proje tamamlanması: +100 XP
     * - Profil tamamlama: +20 XP
     * - İlk yorum: +5 XP
     * 
     * Seviye Hesaplama:
     * Seviye 1: 0-100 XP
     * Seviye 2: 101-250 XP
     * Seviye 3: 251-500 XP
     * Seviye N: Formül = (N-1) * 150 + 100
     */

    [Display(Name = "Deneyim Puanı (XP)")]
    public int DeneyimPuani { get; set; } = 0;

    [Display(Name = "Seviye")]
    public int Seviye { get; set; } = 1;

    /// <summary>
    /// Bir sonraki seviye için gereken XP
    /// </summary>
    [NotMapped]
    public int SonrakiSeviyeIcinGerekenXP => (Seviye * 150) + 100;

    /// <summary>
    /// Mevcut seviyede kazanılan ilerleme yüzdesi
    /// </summary>
    [NotMapped]
    public int SeviyeIlerlemesiYuzdesi
    {
        get
        {
            int mevcutSeviyeBaslangicXP = ((Seviye - 1) * 150) + 100;
            int mevcutSeviyedeKazanilan = DeneyimPuani - mevcutSeviyeBaslangicXP;
            int seviyeIcinGereken = SonrakiSeviyeIcinGerekenXP - mevcutSeviyeBaslangicXP;
            return seviyeIcinGereken > 0 ? (mevcutSeviyedeKazanilan * 100) / seviyeIcinGereken : 0;
        }
    }

    #endregion

    #region Profil Tamamlama ve İstatistikler

    /// <summary>
    /// Profil tamamlanma yüzdesi (0-100)
    /// Hesaplama: Doldurulmuş alan sayısı / Toplam alan sayısı
    /// </summary>
    [NotMapped]
    public int ProfilTamamlanmaYuzdesi
    {
        get
        {
            int toplamAlan = 10; // Temel profil alanları
            int doluAlan = 0;

            if (!string.IsNullOrEmpty(Ad)) doluAlan++;
            if (!string.IsNullOrEmpty(Soyad)) doluAlan++;
            if (!string.IsNullOrEmpty(Bio)) doluAlan++;
            if (!string.IsNullOrEmpty(ProfilFotografiUrl)) doluAlan++;
            if (OkulId.HasValue) doluAlan++;
            if (IlId.HasValue) doluAlan++;
            if (!string.IsNullOrEmpty(PhoneNumber)) doluAlan++;
            if (EmailConfirmed) doluAlan++;

            // Kullanıcı tipine göre ek alanlar
            if (KullaniciTipi == KullaniciTipi.Ogretmen && !string.IsNullOrEmpty(Brans)) doluAlan++;
            if (KullaniciTipi == KullaniciTipi.Ogrenci && !string.IsNullOrEmpty(Sinif)) doluAlan++;

            return (doluAlan * 100) / toplamAlan;
        }
    }

    /// <summary>
    /// Kullanıcının paylaşılabilir profil URL'si
    /// Örnek: /Profil/kullanici-adi-123
    /// </summary>
    [NotMapped]
    public string ProfilUrl => $"/Profil/{UserName}";

    #endregion

    #region Hesap Durumu ve Meta Bilgiler

    [Display(Name = "Hesap Aktif")]
    public bool AktifMi { get; set; } = true;

    [Display(Name = "Email Bildirimler")]
    public bool EmailBildirimleri { get; set; } = true;

    [Display(Name = "Push Bildirimler")]
    public bool PushBildirimleri { get; set; } = true;

    [Display(Name = "Kayıt Tarihi")]
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;

    [Display(Name = "Son Giriş Tarihi")]
    public DateTime? SonGirisTarihi { get; set; }

    [Display(Name = "Son Aktivite Tarihi")]
    public DateTime? SonAktiviteTarihi { get; set; }

    /// <summary>
    /// Soft Delete için - Hesap silindiğinde işaretlenir
    /// </summary>
    public bool SilindiMi { get; set; } = false;

    public DateTime? SilinmeTarihi { get; set; }

    #endregion

    #region Navigation Properties (İlişkiler)
    /*
     * Entity Framework Core için navigation property'ler
     * 
     * Virtual keyword: Lazy Loading için (gerekirse yüklenir)
     * ICollection: One-to-Many ilişkiler için
     * 
     * İKİ SİSTEM AYRIMI:
     * - Sistem 1: Çağrı Bilgi (CagriBilgisi)
     * - Sistem 2: Proje Yönetimi (ProjeYonetimi)
     */

    // ════════════════════════════════════════════════════════════════════════════
    // SİSTEM 1: ÇAĞRI BİLGİ SİSTEMİ
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kullanıcının takip ettiği çağrılar (MEB, TÜBİTAK vb.)
    /// </summary>
    public virtual ICollection<CagriTakip> CagriTakipleri { get; set; } = new List<CagriTakip>();

    // ════════════════════════════════════════════════════════════════════════════
    // SİSTEM 2: PROJE YÖNETİMİ
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kullanıcının kurduğu projeler (Kurucu)
    /// </summary>
    public virtual ICollection<ProjeYonetimi> KurulanProjeler { get; set; } = new List<ProjeYonetimi>();

    /// <summary>
    /// Kullanıcının katıldığı proje ekipleri
    /// </summary>
    public virtual ICollection<ProjeEkipUyesi> ProjeEkipUyelikleri { get; set; } = new List<ProjeEkipUyesi>();

    // ════════════════════════════════════════════════════════════════════════════
    // SOSYAL İÇERİK
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kullanıcının gönderileri
    /// </summary>
    public virtual ICollection<Gonderi> Gonderiler { get; set; } = new List<Gonderi>();

    /// <summary>
    /// Kullanıcının yorumları
    /// </summary>
    public virtual ICollection<Yorum> Yorumlar { get; set; } = new List<Yorum>();

    /// <summary>
    /// Kullanıcının beğenileri
    /// </summary>
    public virtual ICollection<Begeni> Begeniler { get; set; } = new List<Begeni>();

    // ════════════════════════════════════════════════════════════════════════════
    // OYUNLAŞTIRMA
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kullanıcının rozetleri
    /// </summary>
    public virtual ICollection<KullaniciRozet> Rozetler { get; set; } = new List<KullaniciRozet>();

    // ════════════════════════════════════════════════════════════════════════════
    // BİLDİRİM
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kullanıcının bildirimleri
    /// </summary>
    public virtual ICollection<Bildirim> Bildirimler { get; set; } = new List<Bildirim>();

    // ════════════════════════════════════════════════════════════════════════════
    // DUYURU VE ETKİNLİK
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kullanıcının takip ettiği duyurular
    /// </summary>
    public virtual ICollection<DuyuruTakip> DuyuruTakipleri { get; set; } = new List<DuyuruTakip>();

    /// <summary>
    /// Kullanıcının katıldığı etkinlikler
    /// </summary>
    public virtual ICollection<EtkinlikKatilimci> EtkinlikKatilimlari { get; set; } = new List<EtkinlikKatilimci>();

    // ════════════════════════════════════════════════════════════════════════════
    // DOSYALAR
    // ════════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Kullanıcı tarafından yüklenen dosyalar
    /// </summary>
    public virtual ICollection<Dosya> Dosyalar { get; set; } = new List<Dosya>();

    #endregion

    #region Yardımcı Metodlar

    /// <summary>
    /// XP ekler ve seviye kontrolü yapar
    /// </summary>
    public void XPEkle(int miktar)
    {
        DeneyimPuani += miktar;
        SeviyeKontrolEt();
    }

    /// <summary>
    /// Seviye atlama kontrolü yapar
    /// </summary>
    private void SeviyeKontrolEt()
    {
        while (DeneyimPuani >= SonrakiSeviyeIcinGerekenXP)
        {
            Seviye++;
            // Seviye atlama bildirimi (SignalR ile gönderilir - Service layer'da)
        }
    }

    /// <summary>
    /// Son aktivite tarihini günceller
    /// </summary>
    public void AktiviteGuncelle()
    {
        SonAktiviteTarihi = DateTime.UtcNow;
    }

    #endregion
}

#endregion

#region Kullanıcı Tipi Enum
/*
 * Kullanıcı Tipi Enum
 * 
 * Bu bir ROL değildir!
 * - Roller (Identity): Admin, Moderator, User
 * - Kullanıcı Tipi: Profil özelliği ve filtreleme için
 * 
 * Kullanım:
 * - Kayıt sırasında seçilir
 * - Profile göre özel özellikler açılır (Branş, Sınıf vs.)
 * - Filtreleme ve raporlamada kullanılır
 */

public enum KullaniciTipi
{
    [Display(Name = "Standart Kullanıcı")]
    Standart = 0,

    [Display(Name = "Öğrenci")]
    Ogrenci = 1,

    [Display(Name = "Öğretmen")]
    Ogretmen = 2,

    [Display(Name = "Veli")]
    Veli = 3,

    [Display(Name = "Kurum Yetkilisi")]
    KurumYetkilisi = 4
}

#endregion