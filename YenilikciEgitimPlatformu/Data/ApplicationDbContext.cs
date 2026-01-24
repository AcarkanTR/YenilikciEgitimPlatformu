using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YenilikciEgitimPlatformu.Models;
using YenilikciEgitimPlatformu.Models.Identity;

namespace YenilikciEgitimPlatformu.Data;

/*
 * ApplicationDbContext - EF Core DbContext
 * 
 * Bu sınıf Entity Framework Core için merkezi veritabanı context'idir.
 * 
 * Sorumluluklar:
 * 1. DbSet<> tanımları (Tüm tablolar)
 * 2. Entity ilişkilerinin yapılandırılması (Fluent API)
 * 3. Query Filter'lar (Global filters - Soft Delete)
 * 4. Seed Data hazırlığı
 * 5. SaveChanges override (Audit Trail otomasyonu)
 * 
 * Mimari Kararlar:
 * - Identity entegrasyonu: IdentityDbContext<ApplicationUser>
 * - Convention over Configuration (Mümkün olduğunca)
 * - Fluent API (İlişkiler ve özel kurallar için)
 * - Query Filters (Soft Delete otomasyonu)
 * - UTC Timestamp (Tüm tarihler UTC)
 */

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    #region Constructor

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #endregion

    #region DbSet Tanımları (Tablolar)

    /*
     * DbSet<T> her bir entity için veritabanı tablosunu temsil eder
     * 
     * Tablo isimleri Türkçe (README gereksinimi)
     * Örnek: DbSet<Proje> -> "Projeler" tablosu
     */

    #region Okul ve Konum Tabloları

    public DbSet<Il> Iller { get; set; }
    public DbSet<Ilce> Ilceler { get; set; }
    public DbSet<Okul> Okullar { get; set; }

    #endregion

    #region Proje Yönetimi Tabloları (Sistem 2)

    public DbSet<ProjeYonetimi> ProjeYonetimleri { get; set; }
    public DbSet<ProjeKategori> ProjeKategorileri { get; set; }
    public DbSet<ProjeEkipUyesi> ProjeEkipUyeleri { get; set; }
    public DbSet<ProjeGorevi> ProjeGorevleri { get; set; }
    public DbSet<ProjeAktivitesi> ProjeAktiviteleri { get; set; }
    public DbSet<ProjeDosya> ProjeDosyalari { get; set; }

    #endregion

    #region Sosyal İçerik Tabloları

    public DbSet<Gonderi> Gonderiler { get; set; }
    public DbSet<Yorum> Yorumlar { get; set; }
    public DbSet<Begeni> Begeniler { get; set; }
    public DbSet<Dosya> Dosyalar { get; set; }

    #endregion

    #region Duyuru ve Etkinlik Tabloları

    public DbSet<Duyuru> Duyurular { get; set; }
    public DbSet<DuyuruKategori> DuyuruKategorileri { get; set; }
    public DbSet<DuyuruTakip> DuyuruTakipleri { get; set; }

    // Kullanıcı Etkinlikleri
    public DbSet<Etkinlik> Etkinlikler { get; set; }
    public DbSet<EtkinlikKatilimci> EtkinlikKatilimcilari { get; set; }

    #endregion

    #region Çağrı Bilgi Sistemi Tabloları (Sistem 1)

    public DbSet<CagriBilgisi> CagriBilgileri { get; set; }
    public DbSet<CagriEkDosya> CagriEkDosyalari { get; set; }
    public DbSet<CagriTakip> CagriTakipleri { get; set; }

    #endregion

    #region Bildirim Tablosu

    public DbSet<Bildirim> Bildirimler { get; set; }

    #endregion

    #region Oyunlaştırma Tabloları

    public DbSet<Rozet> Rozetler { get; set; }
    public DbSet<KullaniciRozet> KullaniciRozetleri { get; set; }

    #endregion

    #region Moderasyon ve Audit Tabloları

    public DbSet<ModerasyonKaydi> ModerasyonKayitlari { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    #endregion

    #endregion

    #region Model Configuration (OnModelCreating)

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Global Cascade Delete Disable (SQL Server için)

        /*
         * SQL Server'da çoklu cascade path hatası önleme
         * 
         * Tüm foreign key'leri varsayılan olarak Restrict (NO ACTION) yap
         * Bu sayede cascade delete döngüleri engellenir
         * 
         * ÖNEMLI: Bu kural ÖNCE uygulanmalı, sonra manuel override'lar
         */

        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        #endregion

        #region Identity Tablo İsimleri (Türkçe)

        /*
         * ASP.NET Core Identity'nin varsayılan tablo isimlerini Türkçe'ye çevir
         * 
         * Varsayılan:          Türkçe:
         * AspNetUsers       -> Kullanicilar
         * AspNetRoles       -> Roller
         * AspNetUserRoles   -> KullaniciRolleri
         * ...
         */

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Kullanicilar");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>(entity =>
        {
            entity.ToTable("Roller");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("KullaniciRolleri");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("KullaniciClaim");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("KullaniciLogin");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RolClaim");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("KullaniciToken");
        });

        #endregion

        #region Global Query Filters (Soft Delete)

        /*
         * Soft Delete için global query filter
         * 
         * Bu filter sayesinde:
         * - Her sorguda ".Where(x => !x.SilindiMi)" yazmaya gerek kalmaz
         * - Otomatik olarak silinmemiş kayıtlar getirilir
         * - IgnoreQueryFilters() ile devre dışı bırakılabilir
         */

        // Çağrı Bilgi Sistemi
        modelBuilder.Entity<CagriBilgisi>().HasQueryFilter(c => !c.SilindiMi);
        modelBuilder.Entity<CagriEkDosya>().HasQueryFilter(c => !c.SilindiMi);

        // Proje Yönetimi Sistemi
        modelBuilder.Entity<ProjeYonetimi>().HasQueryFilter(p => !p.SilindiMi);
        modelBuilder.Entity<ProjeGorevi>().HasQueryFilter(g => !g.SilindiMi);
        modelBuilder.Entity<ProjeAktivitesi>().HasQueryFilter(a => !a.SilindiMi);
        modelBuilder.Entity<ProjeDosya>().HasQueryFilter(d => !d.SilindiMi);

        // Sosyal İçerik
        modelBuilder.Entity<Gonderi>().HasQueryFilter(g => !g.SilindiMi);
        modelBuilder.Entity<Yorum>().HasQueryFilter(y => !y.SilindiMi);

        // Duyuru / Etkinlik
        modelBuilder.Entity<Duyuru>().HasQueryFilter(d => !d.SilindiMi);
        modelBuilder.Entity<Etkinlik>().HasQueryFilter(e => !e.SilindiMi);

        // Okul
        modelBuilder.Entity<Okul>().HasQueryFilter(o => !o.SilindiMi);

        // Dosya
        modelBuilder.Entity<Dosya>().HasQueryFilter(d => !d.SilindiMi);

        #endregion

        #region Entity Yapılandırmaları (Fluent API)

        #region ProjeYonetimi İlişkileri (Sistem 2)

        modelBuilder.Entity<ProjeYonetimi>(entity =>
        {
            // Index'ler
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.HasIndex(p => p.Durum);
            entity.HasIndex(p => p.KategoriId);
            entity.HasIndex(p => p.YayindaMi);
            entity.HasIndex(p => p.KurucuKullaniciId);

            // Kategori
            entity.HasOne(p => p.Kategori)
                  .WithMany(k => k.Projeler)
                  .HasForeignKey(p => p.KategoriId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Kurucu
            entity.HasOne(p => p.Kurucu)
                  .WithMany(u => u.KurulanProjeler)
                  .HasForeignKey(p => p.KurucuKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Kaynak Çağrı (Opsiyonel)
            entity.HasOne(p => p.KaynakCagriBilgisi)
                  .WithMany()
                  .HasForeignKey(p => p.KaynakCagriBilgisiId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Okul
            entity.HasOne(p => p.Okul)
                  .WithMany(o => o.ProjeYonetimleri)
                  .HasForeignKey(p => p.OkulId)
                  .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity navigation
            entity.HasOne(p => p.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(p => p.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region CagriBilgisi İlişkileri (Sistem 1)

        modelBuilder.Entity<CagriBilgisi>(entity =>
        {
            // Index'ler
            entity.HasIndex(c => c.Slug).IsUnique();
            entity.HasIndex(c => c.CagriTuru);
            entity.HasIndex(c => c.YayindaMi);

            // BaseEntity navigation
            entity.HasOne(c => c.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(c => c.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CagriEkDosya>(entity =>
        {
            // CASCADE
            entity.HasOne(d => d.CagriBilgisi)
                  .WithMany(c => c.EkDosyalar)
                  .HasForeignKey(d => d.CagriBilgisiId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(d => d.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CagriTakip>(entity =>
        {
            entity.HasIndex(ct => new { ct.CagriBilgisiId, ct.KullaniciId }).IsUnique();

            // CASCADE
            entity.HasOne(ct => ct.CagriBilgisi)
                  .WithMany(c => c.Takipciler)
                  .HasForeignKey(ct => ct.CagriBilgisiId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ct => ct.Kullanici)
                  .WithMany(u => u.CagriTakipleri)
                  .HasForeignKey(ct => ct.KullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region ESKI PROJE SİLİNDİ - ÜSTTEKİ ProjeYonetimi KULLANILACAK

        #region ProjeEkipUyesi (Many-to-Many)

        modelBuilder.Entity<ProjeEkipUyesi>(entity =>
        {
            entity.HasIndex(e => new { e.ProjeYonetimId, e.KullaniciId })
                  .IsUnique();
            entity.HasIndex(e => e.AktifMi);

            // CASCADE
            entity.HasOne(e => e.ProjeYonetim)
                  .WithMany(p => p.EkipUyeleri)
                  .HasForeignKey(e => e.ProjeYonetimId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Kullanici)
                  .WithMany(k => k.ProjeEkipUyelikleri)
                  .HasForeignKey(e => e.KullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region ProjeGorevi İlişkileri

        modelBuilder.Entity<ProjeGorevi>(entity =>
        {
            entity.HasIndex(g => g.ProjeYonetimId);
            entity.HasIndex(g => g.Durum);
            entity.HasIndex(g => g.Oncelik);
            entity.HasIndex(g => g.AtananKullaniciId);

            // CASCADE
            entity.HasOne(g => g.ProjeYonetim)
                  .WithMany(p => p.Gorevler)
                  .HasForeignKey(g => g.ProjeYonetimId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(g => g.AtananKullanici)
                  .WithMany()
                  .HasForeignKey(g => g.AtananKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity navigation
            entity.HasOne(g => g.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(g => g.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjeAktivitesi>(entity =>
        {
            entity.HasIndex(a => a.ProjeYonetimId);

            // CASCADE
            entity.HasOne(a => a.ProjeYonetim)
                  .WithMany(p => p.Aktiviteler)
                  .HasForeignKey(a => a.ProjeYonetimId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(a => a.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjeDosya>(entity =>
        {
            entity.HasIndex(d => d.ProjeYonetimId);

            // CASCADE
            entity.HasOne(d => d.ProjeYonetim)
                  .WithMany(p => p.Dosyalar)
                  .HasForeignKey(d => d.ProjeYonetimId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(d => d.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #endregion

        #region Gonderi İlişkileri

        modelBuilder.Entity<Gonderi>(entity =>
        {
            entity.HasIndex(g => g.YayindaMi);
            entity.HasIndex(g => g.ProjeYonetimId);
            entity.HasIndex(g => g.OlusturulmaTarihi);
            entity.HasIndex(g => g.SabitlenmeTarihi);

            entity.HasOne(g => g.ProjeYonetim)
                  .WithMany()
                  .HasForeignKey(g => g.ProjeYonetimId)
                  .OnDelete(DeleteBehavior.SetNull);

            // BaseEntity navigation property'leri
            entity.HasOne(g => g.OlusturanKullanici)
                  .WithMany(u => u.Gonderiler)
                  .HasForeignKey(g => g.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.GuncelleyenKullanici)
                  .WithMany()
                  .HasForeignKey(g => g.GuncelleyenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.SilenKullanici)
                  .WithMany()
                  .HasForeignKey(g => g.SilenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region Yorum İlişkileri (Nested Comments)

        modelBuilder.Entity<Yorum>(entity =>
        {
            // Index'ler
            entity.HasIndex(y => new { y.YorumTuru, y.IliskiliEntityId }); // Polymorphic ilişki
            entity.HasIndex(y => y.UstYorumId);
            entity.HasIndex(y => y.OnaylandiMi);

            // Self-referencing relationship (Nested comments)
            entity.HasOne(y => y.UstYorum)
                  .WithMany(y => y.AltYorumlar)
                  .HasForeignKey(y => y.UstYorumId)
                  .OnDelete(DeleteBehavior.Restrict); // Üst yorum silinirse alt yorumlar kalabilir

            // BaseEntity navigation property'leri
            entity.HasOne(y => y.OlusturanKullanici)
                  .WithMany(u => u.Yorumlar)
                  .HasForeignKey(y => y.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(y => y.GuncelleyenKullanici)
                  .WithMany()
                  .HasForeignKey(y => y.GuncelleyenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(y => y.SilenKullanici)
                  .WithMany()
                  .HasForeignKey(y => y.SilenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region Begeni İlişkileri (Polymorphic)

        modelBuilder.Entity<Begeni>(entity =>
        {
            // Composite Index (Aynı kullanıcı aynı içeriği bir kez beğenebilir)
            entity.HasIndex(b => new { b.BegeniTuru, b.IliskiliEntityId, b.KullaniciId })
                  .IsUnique();

            // Kullanıcı ile ilişki
            entity.HasOne(b => b.Kullanici)
                  .WithMany(u => u.Begeniler)
                  .HasForeignKey(b => b.KullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region Dosya İlişkileri

        modelBuilder.Entity<Dosya>(entity =>
        {
            entity.HasIndex(d => new { d.IliskiliEntityTuru, d.IliskiliEntityId });
            entity.HasIndex(d => d.Turu);
            entity.HasIndex(d => d.OlusturanKullaniciId);

            // BaseEntity'den gelen navigation property'ler için manuel yapılandırma
            // (Çoklu FK aynı tabloya gittiği için EF Core karışıyor)

            entity.HasOne(d => d.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(d => d.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.GuncelleyenKullanici)
                  .WithMany()
                  .HasForeignKey(d => d.GuncelleyenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.SilenKullanici)
                  .WithMany()
                  .HasForeignKey(d => d.SilenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region Duyuru İlişkileri

        modelBuilder.Entity<Duyuru>(entity =>
        {
            entity.HasIndex(d => d.Slug).IsUnique();
            entity.HasIndex(d => d.YayindaMi);
            entity.HasIndex(d => d.OneCikanMi);
            entity.HasIndex(d => new { d.BaslangicTarihi, d.BitisTarihi });
            entity.HasIndex(d => d.HedefIlId);
            entity.HasIndex(d => d.HedefIlceId);
            entity.HasIndex(d => d.HedefOkulId);

            entity.HasOne(d => d.Kategori)
                  .WithMany(k => k.Duyurular)
                  .HasForeignKey(d => d.KategoriId)
                  .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity navigation property'leri
            entity.HasOne(d => d.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(d => d.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.GuncelleyenKullanici)
                  .WithMany()
                  .HasForeignKey(d => d.GuncelleyenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.SilenKullanici)
                  .WithMany()
                  .HasForeignKey(d => d.SilenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region DuyuruTakip (Many-to-Many)

        modelBuilder.Entity<DuyuruTakip>(entity =>
        {
            entity.HasIndex(dt => new { dt.DuyuruId, dt.KullaniciId })
                  .IsUnique();

            // Duyuru silinirse takipler de silinsin (CASCADE - Override)
            entity.HasOne(dt => dt.Duyuru)
                  .WithMany(d => d.Takipciler)
                  .HasForeignKey(dt => dt.DuyuruId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dt => dt.Kullanici)
                  .WithMany(u => u.DuyuruTakipleri)
                  .HasForeignKey(dt => dt.KullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region Etkinlik İlişkileri

        modelBuilder.Entity<Etkinlik>(entity =>
        {
            entity.HasIndex(e => e.YayindaMi);
            entity.HasIndex(e => new { e.BaslangicTarihi, e.BitisTarihi });
            entity.HasIndex(e => e.Turu);
            entity.HasIndex(e => e.KayitAcik);

            // BaseEntity navigation property'leri
            entity.HasOne(e => e.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(e => e.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.GuncelleyenKullanici)
                  .WithMany()
                  .HasForeignKey(e => e.GuncelleyenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SilenKullanici)
                  .WithMany()
                  .HasForeignKey(e => e.SilenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region EtkinlikKatilimci (Many-to-Many)

        modelBuilder.Entity<EtkinlikKatilimci>(entity =>
        {
            entity.HasIndex(ek => new { ek.EtkinlikId, ek.KullaniciId })
                  .IsUnique();

            // Etkinlik silinirse katılımcılar da silinsin (CASCADE - Override)
            entity.HasOne(ek => ek.Etkinlik)
                  .WithMany(e => e.Katilimcilar)
                  .HasForeignKey(ek => ek.EtkinlikId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ek => ek.Kullanici)
                  .WithMany(u => u.EtkinlikKatilimlari)
                  .HasForeignKey(ek => ek.KullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region Bildirim İlişkileri

        modelBuilder.Entity<Bildirim>(entity =>
        {
            entity.HasIndex(b => b.AliciKullaniciId);
            entity.HasIndex(b => b.OkunduMu);
            entity.HasIndex(b => b.Turu);
            entity.HasIndex(b => new { b.IliskiliEntityTuru, b.IliskiliEntityId });

            // Kullanıcı silinirse bildirimleri de silinsin (CASCADE - Override)
            entity.HasOne(b => b.AliciKullanici)
                  .WithMany(k => k.Bildirimler)
                  .HasForeignKey(b => b.AliciKullaniciId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.GonderenKullanici)
                  .WithMany()
                  .HasForeignKey(b => b.GonderenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region Rozet İlişkileri

        modelBuilder.Entity<Rozet>(entity =>
        {
            entity.HasIndex(r => r.KosulEventAdi);
            entity.HasIndex(r => r.AktifMi);
            entity.HasIndex(r => r.Kategori);
        });

        #endregion

        #region KullaniciRozet (Many-to-Many)

        modelBuilder.Entity<KullaniciRozet>(entity =>
        {
            // Aynı kullanıcı aynı rozeti birden fazla kez kazanamaz
            entity.HasIndex(kr => new { kr.KullaniciId, kr.RozetId })
                  .IsUnique();

            // ÖNEMLI: Eğer shadow property olarak OlusturanKullaniciId oluşturulmuşsa ignore et
            entity.Ignore("OlusturanKullaniciId");

            // Kullanıcı silinirse rozetleri de silinsin (CASCADE - Override)
            entity.HasOne(kr => kr.Kullanici)
                  .WithMany(k => k.Rozetler)
                  .HasForeignKey(kr => kr.KullaniciId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Rozet silinirse kullanıcı rozetleri de silinsin (CASCADE - Override)
            entity.HasOne(kr => kr.Rozet)
                  .WithMany(r => r.KullaniciRozetleri)
                  .HasForeignKey(kr => kr.RozetId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        #endregion

        #region ModerasyonKaydi İlişkileri

        modelBuilder.Entity<ModerasyonKaydi>(entity =>
        {
            // Index'ler (Raporlama için)
            entity.HasIndex(m => new { m.IcerikTuru, m.IcerikId });
            entity.HasIndex(m => m.IcerikSahibiId);
            entity.HasIndex(m => m.OlusturanKullaniciId); // Moderatör
            entity.HasIndex(m => m.Aksiyon);
            entity.HasIndex(m => m.ModerasyonTarihi);

            entity.HasOne(m => m.IcerikSahibi)
                  .WithMany()
                  .HasForeignKey(m => m.IcerikSahibiId)
                  .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silinse bile kayıt kalmalı
        });

        #endregion

        #region AuditLog İlişkileri

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(a => a.KullaniciId);
            entity.HasIndex(a => a.IslemTuru);
            entity.HasIndex(a => new { a.EntityTuru, a.EntityId });
            entity.HasIndex(a => a.IslemTarihi);

            // AuditLog'da OnDelete Restrict (Kullanıcı silinse bile log kalmalı)
            entity.HasOne(a => a.Kullanici)
                  .WithMany()
                  .HasForeignKey(a => a.KullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region ESKI ResmiEtkinlik KALDIRILDI - YERİNE CagriBilgisi SİSTEMİ KULLANILIYOR

        #endregion

        #region Okul ve Konum İlişkileri

        modelBuilder.Entity<Il>(entity =>
        {
            entity.HasIndex(i => i.PlakaKodu).IsUnique();
        });

        modelBuilder.Entity<Ilce>(entity =>
        {
            entity.HasIndex(i => new { i.IlId, i.Ad });

            entity.HasOne(i => i.Il)
                  .WithMany(il => il.Ilceler)
                  .HasForeignKey(i => i.IlId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Okul>(entity =>
        {
            entity.HasIndex(o => o.IlId);
            entity.HasIndex(o => o.IlceId);
            entity.HasIndex(o => o.AktifMi);
            entity.HasIndex(o => o.MEBOkulKodu);

            entity.HasOne(o => o.Il)
                  .WithMany(i => i.Okullar)
                  .HasForeignKey(o => o.IlId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Ilce)
                  .WithMany(i => i.Okullar)
                  .HasForeignKey(o => o.IlceId)
                  .OnDelete(DeleteBehavior.Restrict);

            // BaseEntity navigation property'leri
            entity.HasOne(o => o.OlusturanKullanici)
                  .WithMany()
                  .HasForeignKey(o => o.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.GuncelleyenKullanici)
                  .WithMany()
                  .HasForeignKey(o => o.GuncelleyenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.SilenKullanici)
                  .WithMany()
                  .HasForeignKey(o => o.SilenKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #endregion // Entity Yapılandırmaları sonu
    }

    #endregion // Model Configuration sonu

    #region SaveChanges Override (Audit Trail Otomasyonu)

    /*
     * SaveChanges metodunu override ederek:
     * 1. Audit trail otomatik doldurulur (OlusturanKullanici, OlusturulmaTarihi)
     * 2. Tüm tarihler UTC olarak kaydedilir
     * 3. Soft delete kontrolü yapılır
     * 
     * Bu sayede service layer'da manuel audit trail yazmaya gerek kalmaz
     */

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity || e.Entity is AuditOnlyBaseEntity);

        foreach (var entry in entries)
        {
            // TODO: IHttpContextAccessor entegrasyonu yapılana kadar varsayılan değer
            var currentUserId = "system";

            if (entry.Entity is BaseEntity baseEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.OlusturulmaTarihi = DateTime.UtcNow;

                        // ÖNEMLİ DÜZELTME: Eğer Seeder tarafından gerçek bir ID atanmışsa, onu ezme!
                        if (string.IsNullOrEmpty(baseEntity.OlusturanKullaniciId))
                        {
                            baseEntity.OlusturanKullaniciId = currentUserId;
                        }
                        break;

                    case EntityState.Modified:
                        baseEntity.GuncellenmeTarihi = DateTime.UtcNow;

                        // Güncelleme yapan kullanıcı belli değilse system ata
                        if (string.IsNullOrEmpty(baseEntity.GuncelleyenKullaniciId))
                        {
                            baseEntity.GuncelleyenKullaniciId = currentUserId;
                        }

                        // Soft Delete kontrolü
                        if (baseEntity.SilindiMi && baseEntity.SilinmeTarihi == null)
                        {
                            baseEntity.SilinmeTarihi = DateTime.UtcNow;
                            if (string.IsNullOrEmpty(baseEntity.SilenKullaniciId))
                            {
                                baseEntity.SilenKullaniciId = currentUserId;
                            }
                        }
                        break;
                }
            }
            else if (entry.Entity is AuditOnlyBaseEntity auditEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    auditEntity.OlusturulmaTarihi = DateTime.UtcNow;
                    if (string.IsNullOrEmpty(auditEntity.OlusturanKullaniciId))
                    {
                        auditEntity.OlusturanKullaniciId = currentUserId;
                    }
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }

    #endregion
}