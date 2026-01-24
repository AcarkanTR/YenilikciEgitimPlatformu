using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YenilikciEgitimPlatformu.Models.Identity;

namespace YenilikciEgitimPlatformu.Models;

/*
 * BaseEntity - Temel Entity Sınıfı
 * 
 * Bu sınıf tüm entity'lerin miras aldığı temel özellikleri içerir.
 * 
 * Sağladığı Özellikler:
 * 1. Primary Key (Id)
 * 2. Audit Trail (Kim, ne zaman oluşturdu/güncelledi)
 * 3. Soft Delete (Fiziksel silme yerine işaretleme)
 * 4. Timestamp (Concurrency kontrolü)
 * 
 * Kullanım:
 * public class Proje : BaseEntity { ... }
 * 
 * Avantajlar:
 * - Kod tekrarı önlenir
 * - Tutarlı veri yönetimi
 * - Audit trail otomatik
 * - Soft delete standart
 * - Concurrency koruması
 */

#region Base Entity Sınıfı

public abstract class BaseEntity
{
    #region Primary Key
    /*
     * Tüm entity'lerin benzersiz kimliği
     * 
     * - int tipinde (performans için)
     * - Identity(1,1) - Otomatik artan
     * - Primary Key
     */

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    #endregion

    #region Audit Trail (İz Sürme)
    /*
     * Kim, ne zaman oluşturdu/güncelledi bilgisi
     * 
     * Bu alanlar ApplicationDbContext.SaveChangesAsync() metodunda
     * otomatik olarak doldurulur.
     * 
     * Kullanım:
     * - Yasal gereklilikler
     * - Veri güvenliği
     * - Sorun giderme
     * - Kullanıcı aktivite takibi
     */

    [Required]
    [Display(Name = "Oluşturulma Tarihi")]
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(450)] // Identity User.Id max length
    [Display(Name = "Oluşturan Kullanıcı")]
    public string OlusturanKullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(OlusturanKullaniciId))]
    public virtual ApplicationUser? OlusturanKullanici { get; set; }

    [Display(Name = "Güncellenme Tarihi")]
    public DateTime? GuncellenmeTarihi { get; set; }

    [StringLength(450)]
    [Display(Name = "Güncelleyen Kullanıcı")]
    public string? GuncelleyenKullaniciId { get; set; }

    [ForeignKey(nameof(GuncelleyenKullaniciId))]
    public virtual ApplicationUser? GuncelleyenKullanici { get; set; }

    #endregion

    #region Soft Delete (Yumuşak Silme)
    /*
     * Fiziksel silme yerine kayıt işaretlenir
     * 
     * Avantajlar:
     * - Veri kaybı olmaz
     * - Geri getirilebilir
     * - Audit trail korunur
     * - Yasal gereklilikler
     * - İlişkisel bütünlük korunur
     * 
     * Kullanım:
     * context.Projeler.Where(p => !p.SilindiMi) // Aktif kayıtlar
     * 
     * DbContext'te Query Filter ile otomatikleştirilebilir.
     */

    [Display(Name = "Silindi Mi?")]
    public bool SilindiMi { get; set; } = false;

    [Display(Name = "Silinme Tarihi")]
    public DateTime? SilinmeTarihi { get; set; }

    [StringLength(450)]
    [Display(Name = "Silen Kullanıcı")]
    public string? SilenKullaniciId { get; set; }

    [ForeignKey(nameof(SilenKullaniciId))]
    public virtual ApplicationUser? SilenKullanici { get; set; }

    #endregion

    #region Concurrency Control (Eşzamanlılık Kontrolü)
    /*
     * Optimistic Concurrency için timestamp
     * 
     * Senaryo:
     * 1. Kullanıcı A kaydı okur
     * 2. Kullanıcı B kaydı okur
     * 3. Kullanıcı A kaydı günceller
     * 4. Kullanıcı B kaydı güncellemeye çalışır -> HATA!
     * 
     * RowVersion değiştiği için güncelleme başarısız olur.
     * Kullanıcı B'ye "Kayıt değiştirilmiş, tekrar yükleyin" mesajı gösterilir.
     * 
     * SQL Server'da ROWVERSION tipinde saklanır.
     */

    [Timestamp]
    [Display(Name = "Sürüm")]
    public byte[]? RowVersion { get; set; }

    #endregion

    #region Helper Properties (Yardımcı Özellikler)

    /// <summary>
    /// Kayıt aktif mi? (Silinmemiş mi?)
    /// </summary>
    [NotMapped]
    public bool AktifMi => !SilindiMi;

    /// <summary>
    /// Kayıt yeni mi? (Henüz veritabanına kaydedilmemiş)
    /// </summary>
    [NotMapped]
    public bool YeniKayitMi => Id == 0;

    /// <summary>
    /// En son ne zaman değiştirildi? (Oluşturma veya güncelleme)
    /// </summary>
    [NotMapped]
    public DateTime SonDegisiklikTarihi => GuncellenmeTarihi ?? OlusturulmaTarihi;

    #endregion

    #region Helper Methods (Yardımcı Metodlar)

    /// <summary>
    /// Kaydı soft delete ile işaretle
    /// </summary>
    public virtual void SoftDelete(string kullananiciId)
    {
        SilindiMi = true;
        SilinmeTarihi = DateTime.UtcNow;
        SilenKullaniciId = kullananiciId;
    }

    /// <summary>
    /// Soft delete'i geri al
    /// </summary>
    public virtual void Restore()
    {
        SilindiMi = false;
        SilinmeTarihi = null;
        SilenKullaniciId = null;
    }

    #endregion
}

#endregion

#region Audit Only Base Entity (Soft Delete Olmayan)
/*
 * AuditOnlyBaseEntity - Sadece Audit Trail
 * 
 * Bazı entity'ler soft delete'e ihtiyaç duymaz:
 * - Log kayıtları
 * - Geçmiş kayıtları
 * - Sistem tabloları
 * 
 * Bu entity'ler sadece audit trail özelliklerini kullanır.
 */

public abstract class AuditOnlyBaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(450)]
    public string OlusturanKullaniciId { get; set; } = string.Empty;

    [ForeignKey(nameof(OlusturanKullaniciId))]
    public virtual ApplicationUser? OlusturanKullanici { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}

#endregion