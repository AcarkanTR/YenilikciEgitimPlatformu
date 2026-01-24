# 🎓 YENİLİKÇİ EĞİTİM PLATFORMU (YEP)

## 📋 PROJE ÖZETİ

**Proje Adı:** Yenilikçi Eğitim Platformu  
**Framework:** ASP.NET Core Razor Pages (.NET 10)  
**Mimari:** Clean Architecture (Tek Proje - Klasör Bazlı)  
**Veritabanı:** SQL Server + Entity Framework Core  
**UI Framework:** Tailwind CSS + Glassmorphism  
**Gerçek Zamanlı:** SignalR  
**Cache:** Memory Cache (Development) / Redis (Production)  
**Logging:** Serilog

---

## 🎯 PROJE VİZYONU

Türkiye'deki eğitim kurumlarının (MEB, TÜBİTAK, TEKNOFEST vb.) proje/yarışma/etkinlik çağrılarını merkezi bir platformda toplamak ve öğrencilerin kendi projelerini oluşturup yönetebilecekleri sosyal bir ekosistem sağlamak.

---

## 🏗️ MİMARİ KARARLARI

### 1. İKİ SİSTEM MİMARİSİ ⭐

Platform **iki bağımsız ama entegre sistem** üzerine kurulmuştur:

#### 🏛️ SİSTEM 1: ÇAĞRI BİLGİ SİSTEMİ
**Amaç:** Resmi kurum çağrılarının (MEB, TÜBİTAK, TEKNOFEST) tek merkezden yayınlanması

**Özellikler:**
- Admin/Moderatör yönetir
- Kullanıcılar takip eder ve bildirim alır
- Çağrı türleri: Proje, Yarışma, Etkinlik, Fon Programı, Eğitim
- Başvuru/katılım takibi
- Hedef kitle filtreleme (İl, İlçe, Okul)
- Ek dosya yönetimi (PDF, video, link)

**Ana Modeller:**
- `CagriBilgisi` - Çağrı detayları
- `CagriEkDosya` - Çağrı ekleri
- `CagriTakip` - Kullanıcı takibi

#### 👥 SİSTEM 2: PROJE YÖNETİMİ SİSTEMİ
**Amaç:** Kullanıcıların kendi projelerini oluşturması ve yönetmesi

**Özellikler:**
- Kullanıcı sahipliği
- Ekip kurma ve yönetimi
- Görev atama ve takibi
- Proje aktiviteleri (Sprint, toplantı, demo)
- İlerleme takibi
- Dosya yönetimi
- Opsiyonel olarak Sistem 1 çağrılarına bağlanabilir

**Ana Modeller:**
- `ProjeYonetimi` - Kullanıcı projeleri
- `ProjeEkipUyesi` - Ekip üyelikleri
- `ProjeGorevi` - Görev yönetimi
- `ProjeAktivitesi` - Aktivite takibi
- `ProjeDosya` - Dosya yönetimi
- `ProjeKategori` - Proje kategorileri

#### 🔗 İKİ SİSTEM ARASINDAKİ BAĞLANTI
```csharp
// ProjeYonetimi entity'sinde
public int? KaynakCagriBilgisiId { get; set; }
public virtual CagriBilgisi? KaynakCagriBilgisi { get; set; }
```
- Bir kullanıcı projesi opsiyonel olarak bir çağrıya referans verebilir
- Örnek: "Bu proje TÜBİTAK 2204-A yarışması için oluşturuldu"

---

### 2. Tek Proje Yapısı

**Karar:** Multi-project yerine **klasör bazlı katmanlama**

**Klasör Yapısı:**
```
YenilikciEgitimPlatformu/
├── Data/                           # EF Core, DbContext, Migrations
│   ├── ApplicationDbContext.cs
│   ├── DbSeeder.cs
│   └── Migrations/
├── Models/                         # Entity Modeller
│   ├── Identity/
│   │   └── ApplicationUser.cs
│   ├── CagriBilgisi.cs      # Sistem 1 modeller
│   ├── ProjeYonetimi.cs      # Sistem 2 modeller
│   ├── OkulKonum.cs
│   ├── SosyalIcerik.cs
│   ├── DuyuruEtkinlikBildirim.cs
│   ├── OyunlastirmaModerasyon.cs
│   └── BaseEntity.cs
├── Services/                       # İş Mantığı
│   ├── Interfaces/                 # (OLUŞTURULACAK)
│   ├── Implementations/            # (OLUŞTURULACAK)
│   └── TurkishIdentityErrorDescriber.cs
├── ViewModels/                     # DTO'lar (OLUŞTURULACAK)
├── Pages/                          # Razor Pages (OLUŞTURULACAK)
├── Hubs/                           # SignalR
│   └── BildirimHub.cs
├── wwwroot/                        # Static dosyalar (OLUŞTURULACAK)
└── Program.cs
```

---

## 📊 VERİTABANI MİMARİSİ

### Entity Modelleri (Toplam: 35+)

#### 🏛️ Sistem 1: Çağrı Bilgi (3 Entity)
| Entity | Açıklama | Özellikler |
|--------|----------|-----------|
| `CagriBilgisi` | Resmi çağrılar | BaseEntity, Soft Delete, Slug, Hedef filtreleme |
| `CagriEkDosya` | Çağrı dosyaları | CASCADE delete, Dosya türü enum |
| `CagriTakip` | Kullanıcı takibi | Junction table, E-posta bildirimi |

#### 👥 Sistem 2: Proje Yönetimi (6 Entity)
| Entity | Açıklama | Özellikler |
|--------|----------|-----------|
| `ProjeYonetimi` | Kullanıcı projeleri | BaseEntity, Soft Delete, Slug, Kurucu |
| `ProjeKategori` | Proje kategorileri | Seed data |
| `ProjeEkipUyesi` | Ekip üyelikleri | Junction table, Rol sistemi |
| `ProjeGorevi` | Görev yönetimi | BaseEntity, Durum/Öncelik enum |
| `ProjeAktivitesi` | Aktivite takibi | Sprint, toplantı, demo |
| `ProjeDosya` | Proje dosyaları | Versiyon takibi |

#### 🏫 Okul ve Konum (3 Entity)
| Entity | Açıklama | Özellikler |
|--------|----------|-----------|
| `Il` | 81 İl | Plaka kodu, Bölge |
| `Ilce` | İlçeler | İl ilişkisi |
| `Okul` | Eğitim kurumları | Soft Delete, MEB kodu, Tür enum |

#### 📱 Sosyal İçerik (4 Entity)
| Entity | Açıklama | Özellikler |
|--------|----------|-----------|
| `Gonderi` | Kullanıcı paylaşımları | Polymorphic, Medya, Hashtag |
| `Yorum` | Nested comments | Self-referencing, Onay sistemi |
| `Begeni` | Polymorphic beğeni | Unique constraint |
| `Dosya` | Medya dosyaları | GUID isimlendirme, Thumbnail |

#### 📢 Duyuru/Etkinlik/Bildirim (6 Entity)
| Entity | Açıklama | Özellikler |
|--------|----------|-----------|
| `Duyuru` | Sosyal duyurular | Soft Delete, Hedef filtreleme |
| `DuyuruKategori` | Duyuru kategorileri | Seed data |
| `DuyuruTakip` | Takip sistemi | Junction table |
| `Etkinlik` | Kullanıcı etkinlikleri | Online/Fiziksel, Kontenjan |
| `EtkinlikKatilimci` | Katılım takibi | Junction table |
| `Bildirim` | Gerçek zamanlı bildirimler | SignalR entegrasyonu |

#### 🎮 Oyunlaştırma (2 Entity)
| Entity | Açıklama | Özellikler |
|--------|----------|-----------|
| `Rozet` | Başarı rozetleri | Event-based, XP ödülü |
| `KullaniciRozet` | Rozet kazanımları | Junction table, Kazanım açıklaması |

#### 🛡️ Moderasyon/Audit (2 Entity)
| Entity | Açıklama | Özellikler |
|--------|----------|-----------|
| `ModerasyonKaydi` | İçerik moderasyonu | SOFT DELETE YOK, Hukuki iz |
| `AuditLog` | Sistem audit | SOFT DELETE YOK, Teknik iz |

---

### Base Entity Stratejisi

#### `BaseEntity` (Audit Trail + Soft Delete)
```csharp
- Id (int)
- OlusturulmaTarihi (DateTime UTC)
- OlusturanKullaniciId (string)
- GuncellenmeTarihi (DateTime? UTC)
- GuncelleyenKullaniciId (string?)
- SilindiMi (bool)
- SilinmeTarihi (DateTime? UTC)
- SilenKullaniciId (string?)
- RowVersion (byte[]) // Concurrency
```

**Kullanım:** Tüm ana entity'ler (CagriBilgisi, ProjeYonetimi, Gonderi, Duyuru, vb.)

#### `AuditOnlyBaseEntity` (Sadece Audit Trail)
```csharp
- Id (int)
- OlusturulmaTarihi (DateTime UTC)
- OlusturanKullaniciId (string)
```

**Kullanım:** Moderasyon ve Audit kayıtları (değiştirilemez)

#### Manuel ID (Junction Tablolar)
Junction tablolar (ProjeEkipUyesi, CagriTakip, vb.) **Cascade path çakışması önlemek için** BaseEntity kullanmaz, manuel ID ve timestamp'ler içerir.

---

### Foreign Key ve Cascade Stratejisi

**Global Kural:** Tüm FK'lar varsayılan olarak `DeleteBehavior.Restrict`

**CASCADE İzni Verilen İlişkiler:**
- CagriBilgisi → CagriEkDosya
- CagriBilgisi → CagriTakip
- ProjeYonetimi → ProjeEkipUyesi
- ProjeYonetimi → ProjeGorevi
- ProjeYonetimi → ProjeAktivitesi
- ProjeYonetimi → ProjeDosya
- Duyuru → DuyuruTakip
- Etkinlik → EtkinlikKatilimci
- Kullanici → Bildirim
- Kullanici/Rozet → KullaniciRozet

---

## 🔐 GÜVENLİK VE KİMLİK DOĞRULAMA

### ASP.NET Core Identity

**Genişletilmiş Kullanıcı Modeli:** `ApplicationUser`

**Ek Alanlar:**
```csharp
// Profil
- Ad, Soyad, TamAd
- Cinsiyet, DogumTarihi
- Telefon, BiografiMetni
- ProfilFotoUrl, KapakFotoUrl

// Okul Bilgileri
- OkulId (optional)
- Sinif, OgrenciNo

// Kullanıcı Tipi (NOT ROLE!)
- KullaniciTipi (Enum: Standart, Ogrenci, Ogretmen, Diger)

// Oyunlaştırma
- DeneyimPuani (XP)
- Seviye (computed: (Seviye - 1) * 150 + 100)

// Durum
- EmailDogrulandiMi
- AktifMi, SonGirisTarihi
```

### Roller

**Seed Data:**
- Admin
- Moderator
- User

**Admin Hesabı:**
- Email: admin@yep.com
- Şifre: Admin123!

### Türkçe Hata Mesajları

`TurkishIdentityErrorDescriber` - Tüm Identity hataları Türkçe

---

## 💾 VERİTABANI YAPILANDIRMASI

### ApplicationDbContext Özellikleri

1. **Global Query Filters** (Soft Delete Otomasyonu)
2. **Cascade Delete Kontrol** (SQL Server uyumlu)
3. **SaveChanges Override** (Audit Trail otomasyonu)
4. **Fluent API Yapılandırmaları**
5. **Index'ler** (Performans)

### Seed Data (DbSeeder)

**Roller:** 5 rol  
**Admin:** 1 hesap  
**İller:** 81 il (Plaka kodu + Bölge)  
**Proje Kategorileri:** 8 kategori (İkon + Renk)  
**Duyuru Kategorileri:** 6 kategori  
**Rozetler:** 7 başlangıç rozeti

---

## 🔔 GERÇEK ZAMANLI BİLDİRİMLER

### SignalR Hub: `BildirimHub`

**Connection Management:**
- Kullanıcı bazlı gruplar: `user_{userId}`
- Proje bazlı gruplar: `proje_{projeId}`

**Server → Client Metodlar:**
```javascript
BildirimGonderAsync(kullaniciId, bildirim)
HerkeseBildirimGonderAsync(bildirim)
GrupBildirimGonderAsync(grupAdi, bildirim)
```

**Client → Server Metodlar:**
```javascript
BildirimleriOkunduOlarakIsaretle(bildirimIds)
YaziyorBildirimi(projeId, yaziyor)
ProjeGrubunaKatil(projeId)
ProjeGrubundanAyril(projeId)
```

**DTO Modelleri:**
- `BildirimDto`
- `SeviyeAtlamaBildirimDto`
- `RozetKazanimBildirimDto`

---

## 🎨 UI/UX STANDARTLARI

### Tasarım Sistemi

**Ana Renk:** Koyu Petrol Mavisi (#0f3460, #16213e)  
**Vurgu Rengi:** Canlı Turuncu (#e94560, #f97316)  
**Nötr Renk:** Soğuk Gri Tonları (Slate/CoolGray)

### Tasarım Dili

- **Glassmorphism:** Kartlar, modaller, navbar
- **Floating Labels:** Tüm form input'ları
- **Soft Shadows:** Yumuşak gölgelendirme
- **Gradient:** Arka plan ve vurgularda

### Animasyonlar (animate.css)

- `fadeIn` - Sayfa açılışları
- `slideUp` - Kartların yüklenmesi
- `bounceIn` - Bildirimler
- `pulse` - Önemli öğeler
- `skeleton` - Loading states

### Geri Bildirim Sistemi (SweetAlert2)

**Merkezi AlertService (JS Module)**

**Kurallar:**
- ❌ YASAK: `alert()`, `confirm()`, `prompt()` (Tarayıcı varsayılan)
- ✅ ZORUNLU: SweetAlert2 kullanımı

**İşlem Türlerine Göre:**

1. **Onay (Confirm)** - Silme, iptal gibi kritik işlemler
   - `icon: 'warning'`
   - `focusCancel: true`
   - Confirm button: Kırmızı

2. **Başarı (Success)**
   - Küçük işlemler → Toast (2-3sn, sağ üst)
   - Büyük işlemler → Modal (Manuel kapatma)

3. **Hata (Error)**
   - ❌ Toast kullanma
   - ✅ Modal kullan (Manuel kapatma zorunlu)

---

## 🎮 OYUNLAŞTIRMA SİSTEMİ

### XP ve Seviye

**Formula:** `(Seviye - 1) * 150 + 100`

**Seviye Tablosu:**
- Seviye 1: 0-100 XP
- Seviye 2: 100-250 XP
- Seviye 3: 250-400 XP
- vb.

### Rozet Sistemi

**Event-Based Trigger:**
```csharp
KosulEventAdi: "IlkGonderi", "IlkProje", "ProjeTamamlanmasi"
KosulDegeri: 1
```

**V1 Rozetler:**
- İlk Adım (10 XP)
- Proje Başlangıcı (50 XP)
- Profil Tamamlama (20 XP)
- Takım Oyuncusu (15 XP)
- Yorum Ustası (5 XP)
- Proje Bitirici (100 XP)
- Sosyal Kelebek (50 XP)

**Rozet Seviyeleri:**
- Bronz
- Gümüş
- Altın
- Platin
- Elmas

---

## 🏛️ EVENTUAL CONSISTENCY STRATEJİSİ

### İşlem Sırası (Değiştirilemez)

1. **Veritabanı yazımı** (SaveChangesAsync) - ATOMIC
2. **Cache güncelleme** - Side effect
3. **SignalR bildirim** - Side effect

**Kritik Kural:** Cache veya SignalR hatası DB rollback sebebi DEĞİLDİR

### Hata Yönetimi

```csharp
// [Mimari Notu]
// Bu metot eventual consistency kabul eder.
// DB atomic, cache ve SignalR side-effect olarak ele alınır.
```

- Hatalar Serilog ile loglanır
- Kullanıcıya UI seviyesinde hata gösterilmez
- Otomatik retry yok (V1)

---

## 📁 DOSYA SİSTEMİ

### Dosya Yükleme

**Konum:** `wwwroot/uploads/`

**İsimlendirme:** GUID + Extension
```
wwwroot/uploads/
├── resimler/
│   └── {guid}.jpg
├── videolar/
│   └── {guid}.mp4
├── dokumanlar/
│   └── {guid}.pdf
└── diger/
    └── {guid}.zip
```

### Dosya Türleri (Enum)

```csharp
public enum DosyaTuru
{
    Resim = 1,
    Video = 2,
    Dokuman = 3,
    Arsiv = 4,
    Diger = 5
}
```

### Optimizasyon

- Sunucu tarafında otomatik WebP dönüşümü
- Resize (max 1920px genişlik)
- Thumbnail oluşturma

---

## 🚫 MODERASYON VE AUDIT

### Moderasyon vs Audit Ayrımı

#### ModerasyonKaydi (Domain - Hukuki İz)
- Hangi içerik
- Hangi aksiyon
- Hangi gerekçe
- Hangi moderatör
- **SOFT DELETE YOK**

#### AuditLog (Generic - Teknik İz)
- Kim, ne zaman, neyi yaptı
- İçerik oluşturma/güncelleme/silme
- Rol değişimi
- Login/Logout
- **SOFT DELETE YOK**

### Soft Delete Politikası

| Senaryo | Davranış |
|---------|----------|
| Kullanıcı kendi içeriğini silerse | Soft Delete |
| Moderatör silerse | Soft Delete + ModerasyonKaydi |
| Fiziksel silme | ❌ V1'de YOK |

---

## 📊 TAMAMLANAN İŞLER

### ✅ Yapılandırma (3/3)
- [x] Program.cs (DI, Identity, Serilog, SignalR)
- [x] appsettings.json (Production)
- [x] appsettings.Development.json (Development)

### ✅ Entity Modeller (35/35)
- [x] BaseEntity, AuditOnlyBaseEntity
- [x] ApplicationUser (Identity)
- [x] CagriBilgisiModels (3 entity) - **Sistem 1**
- [x] ProjeYonetimModels (6 entity) - **Sistem 2**
- [x] OkulKonumModels (3 entity)
- [x] SosyalIcerikModels (4 entity)
- [x] DuyuruEtkinlikBildirimModels (6 entity)
- [x] OyunlastirmaModerasyonModels (4 entity)

### ✅ Veritabanı Katmanı (2/2)
- [x] ApplicationDbContext (İki sistem yapılandırması)
- [x] DbSeeder (Seed data)

### ✅ Gerçek Zamanlı (1/1)
- [x] BildirimHub (SignalR)

### ✅ Diğer (2/2)
- [x] TurkishIdentityErrorDescriber
- [x] NuGet paketleri belirlendi

---

## 🔨 SONRAKİ ADIMLAR

### 📌 ŞİMDİ YAPILACAK (Öncelik Sırası)

#### 1. Migration ve Veritabanı Oluşturma ⚡
```powershell
# Eski migration'ları sil
Remove-Migration

# Yeni migration oluştur
Add-Migration TwoSystemArchitecture

# Veritabanını güncelle
Update-Database
```

**Beklenen Sonuç:**
- CagriBilgileri tablosu
- ProjeYonetimleri tablosu
- 35+ tablo oluşturulmalı
- Seed data yüklensin

---

#### 2. Service Layer Oluşturma 🔧

**Klasör Yapısı:**
```
Services/
├── Interfaces/
│   ├── ICagriBilgisiService.cs       # Sistem 1
│   ├── IProjeYonetimService.cs       # Sistem 2
│   ├── IProjeGorevService.cs
│   ├── IGonderiService.cs
│   ├── IYorumService.cs
│   ├── IDuyuruService.cs
│   ├── IEtkinlikService.cs
│   ├── IBildirimService.cs
│   ├── IOyunlastirmaService.cs
│   ├── IDosyaService.cs
│   ├── IKullaniciService.cs
│   ├── IProfilService.cs
│   ├── IOkulService.cs
│   ├── IEmailService.cs
│   └── IModerasyonService.cs
│
└── Implementations/
    ├── CagriBilgisiService.cs
    ├── ProjeYonetimService.cs
    ├── ProjeGorevService.cs
    ├── GonderiService.cs
    ├── YorumService.cs
    ├── DuyuruService.cs
    ├── EtkinlikService.cs
    ├── BildirimService.cs
    ├── OyunlastirmaService.cs
    ├── DosyaService.cs
    ├── KullaniciService.cs
    ├── ProfilService.cs
    ├── OkulService.cs
    ├── EmailService.cs
    └── ModerasyonService.cs
```

**Gereksinimler:**
- ✅ Async/Await zorunlu
- ✅ Try/Catch + Serilog
- ✅ Eventual Consistency (DB → Cache → SignalR)
- ✅ Türkçe açıklama satırları
- ✅ Region ile bölümlendirme
- ✅ ViewModel mapping (Entity → DTO)

**İlk Modül:** Sistem 1 - Çağrı Bilgi Servisleri
- ICagriBilgisiService + CagriBilgisiService
- CRUD + Takip + Filtre + Arama

---

#### 3. ViewModel (DTO) Oluşturma 📦

```
ViewModels/
├── CagriBilgisi/                    # Sistem 1
│   ├── CagriListViewModel.cs
│   ├── CagriDetayViewModel.cs
│   ├── CagriOlusturViewModel.cs
│   └── CagriFiltreleViewModel.cs
│
├── ProjeYonetim/                    # Sistem 2
│   ├── ProjeListViewModel.cs
│   ├── ProjeDetayViewModel.cs
│   ├── ProjeOlusturViewModel.cs
│   ├── ProjeGuncelleViewModel.cs
│   └── GorevViewModel.cs
│
├── Gonderi/
├── Duyuru/
└── ... (diğer modüller)
```

---

#### 4. Razor Pages - Layout ve Shared 🎨

**Öncelik Sırası:**

##### 4.1. Shared Components
```
Pages/Shared/
├── _Layout.cshtml                   # Ana layout
├── _LayoutAdmin.cshtml              # Admin layout
├── _LoginPartial.cshtml             # Kullanıcı menüsü
├── Error.cshtml
├── Components/
│   ├── Navbar/
│   │   ├── Navbar.cshtml
│   │   └── Navbar.cshtml.cs
│   └── Footer/
│       ├── Footer.cshtml
│       └── Footer.cshtml.cs
```

**_Layout.cshtml Özellikleri:**
- Tailwind CSS entegrasyonu
- Glassmorphism navbar (sticky, scroll'da renk değişir)
- Modern footer (Grid yapısı, sosyal medya hover efekti)
- SignalR client bağlantısı
- Dark/Light mode toggle (localStorage)
- Toast notification container

##### 4.2. Ana Sayfa
```
Pages/
└── Index.cshtml                     # Landing page
    └── Index.cshtml.cs
```

**Index.cshtml Özellikleri:**
- Hero section (Dinamik slogan, hareketli gradient)
- İstatistik şeridi (Canlı sayılar)
- Keşif blokları:
  - Sistem 1: Öne çıkan çağrılar
  - Sistem 2: Popüler projeler
- Kurumsal footer

---

#### 5. Razor Pages - Sistem 1 (Çağrı Bilgi) 🏛️

```
Pages/CagriBilgileri/
├── Index.cshtml                     # Liste sayfası
├── Index.cshtml.cs
├── Detay.cshtml                     # Detay sayfası
├── Detay.cshtml.cs
└── Admin/
    ├── Olustur.cshtml               # Çağrı oluştur (Admin)
    ├── Olustur.cshtml.cs
    ├── Duzenle.cshtml               # Çağrı düzenle (Admin)
    └── Duzenle.cshtml.cs
```

**Özellikler:**
- Akıllı arama (Autocomplete)
- Filtreleme (Tür, tarih, konum)
- Takip butonu (SignalR)
- Ek dosya görüntüleme
- Paylaş butonu

---

#### 6. Razor Pages - Sistem 2 (Proje Yönetimi) 👥

```
Pages/Projeler/
├── Index.cshtml                     # Liste sayfası
├── Index.cshtml.cs
├── Detay.cshtml                     # Detay sayfası
├── Detay.cshtml.cs
├── Olustur.cshtml                   # Proje oluştur
├── Olustur.cshtml.cs
├── Duzenle.cshtml                   # Proje düzenle
├── Duzenle.cshtml.cs
└── Partials/
    ├── _EkipYonetimi.cshtml
    ├── _GorevListesi.cshtml
    └── _AktiviteZamanTuneli.cshtml
```

**Özellikler:**
- Proje wizard (Adım adım)
- Ekip yönetimi (Üye ekleme/çıkarma)
- Kanban board (Görev yönetimi)
- Aktivite timeline
- Dosya yükleme (Drag & drop)

---

#### 7. Admin Dashboard 👑

```
Pages/Admin/
├── _LayoutAdmin.cshtml
├── Dashboard/
│   └── Index.cshtml                 # Admin ana sayfa
├── CagriBilgileri/
│   ├── Index.cshtml                 # Çağrı yönetimi
│   ├── Olustur.cshtml
│   └── Duzenle.cshtml
├── Kullanicilar/
│   └── Index.cshtml                 # Kullanıcı yönetimi
├── Moderasyon/
│   └── Index.cshtml                 # İçerik moderasyonu
└── Raporlar/
    └── Index.cshtml                 # İstatistikler
```

**Dashboard Özellikleri:**
- Sistem özeti (Kullanıcı, çağrı, proje sayıları)
- Bekleyen onaylar (Çağrı, içerik)
- Grafik ve metrikler
- Son aktiviteler
- Hızlı aksiyonlar

---

#### 8. JavaScript Modülleri 📜

```
wwwroot/js/
├── modules/
│   ├── alertService.js              # SweetAlert2 wrapper
│   ├── notificationService.js       # SignalR bildirimleri
│   ├── signalrConnection.js         # SignalR bağlantı yönetimi
│   ├── cagri.js                     # Çağrı bilgi modülü
│   ├── proje.js                     # Proje yönetimi
│   ├── gorev.js                     # Görev yönetimi
│   ├── gonderi.js                   # Sosyal akış
│   ├── fileUpload.js                # Dosya yükleme
│   └── infiniteScroll.js            # Infinite scroll
└── site.js                          # Ana dosya (minimum)
```

**alertService.js Örnek:**
```javascript
export const AlertService = {
    confirmDelete: async (message) => { ... },
    toastSuccess: (message) => { ... },
    showError: (message) => { ... }
};
```

---

#### 9. Tailwind CSS Yapılandırması 🎨

```
wwwroot/
├── css/
│   ├── tailwind.css                 # Tailwind base
│   ├── glassmorphism.css            # Özel stiller
│   └── animations.css               # Custom animasyonlar
└── tailwind.config.js
```

**tailwind.config.js:**
```javascript
module.exports = {
  theme: {
    extend: {
      colors: {
        'petrol': {
          dark: '#0f3460',
          darker: '#16213e'
        },
        'accent': {
          orange: '#e94560',
          bright: '#f97316'
        }
      }
    }
  }
}
```

---

#### 10. Diğer Modüller (Sırayla) 📋

1. **Sosyal Akış** (Gonderi/Yorum/Beğeni)
2. **Kullanıcı Profili**
3. **Duyuru/Etkinlik**
4. **Dashboard** (Kullanıcı)
5. **Ayarlar**

---

## 🔒 KRİTİK HATIRLATMALAR

### Kod Standartları
- ✅ **Türkçe:** Tablo/kolon, açıklama, hata mesajları
- ✅ **Region:** Her dosya bölümlendirilmeli
- ✅ **Async/Await:** Tüm DB işlemlerinde
- ✅ **Try/Catch:** Her service metodunda
- ✅ **No Bootstrap:** SADECE Tailwind CSS
- ✅ **SweetAlert2:** Tüm kullanıcı geri bildirimleri

### Mimari Kurallar
- ✅ **Interface Pattern:** Her service için
- ✅ **ViewModel:** Entity PageModel'e GEÇEMEz
- ✅ **Thin PageModel:** İş mantığı service'te
- ✅ **Eventual Consistency:** DB → Cache → SignalR
- ✅ **Soft Delete:** BaseEntity kullanan tüm entity'ler

### UI/UX Kuralları
- ✅ **Glassmorphism:** Kartlar, modaller
- ✅ **Floating Labels:** Tüm input'lar
- ✅ **Toast:** Küçük başarı (2-3sn)
- ✅ **Modal:** Hatalar, kritik bilgi
- ✅ **Confirm:** Sadece silme/iptal
- ✅ **No alert():** YASAK

---

## 📞 PROJE YÖNETİMİ

### Geliştirme Ortamı

**Gerekli Araçlar:**
- Visual Studio 2026 (.NET 10 SDK)
- SQL Server 2022
- Node.js (Tailwind CLI için)
- Git

**Çalıştırma:**
```bash
# Package Manager Console'da:
Update-Database

# Uygulamayı başlat:
F5 veya dotnet run
```

**Varsayılan URL:** https://localhost:7001

---

## 📝 ÖNEMLİ NOTLAR

### İki Sistem Farkı
- **Sistem 1 (CagriBilgisi):** Resmi çağrılar, admin yönetir
- **Sistem 2 (ProjeYonetimi):** Kullanıcı projeleri, kullanıcı sahiptir
- Birbirinden bağımsız ama opsiyonel bağlantı mümkün

### Veritabanı
- Tüm tarihler **UTC**
- Soft delete **global query filter** ile
- Cascade path hatası önlendi
- SQL Server uyumlu

### Güvenlik
- E-posta doğrulama altyapısı hazır (pasif)
- 2FA altyapısı hazır (pasif)
- Rol + Claim + Policy bazlı yetkilendirme
- Moderasyon manual (V1)

---

## 🚀 BAŞLANGIÇ KOMUTLARİ

```powershell
# 1. Migration oluştur
Add-Migration TwoSystemArchitecture

# 2. Veritabanını oluştur
Update-Database

# 3. Uygulamayı çalıştır
dotnet run
# veya Visual Studio'da F5

# 4. Admin ile giriş yap
# Email: admin@yep.com
# Şifre: Admin123!
```

---

## 📚 KAYNAKLAR

- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [EF Core Docs](https://docs.microsoft.com/ef/core)
- [Tailwind CSS](https://tailwindcss.com)
- [SweetAlert2](https://sweetalert2.github.io)
- [SignalR](https://docs.microsoft.com/aspnet/core/signalr)

---

**Son Güncelleme:** 22 Ocak 2026  
**Versiyon:** 1.0.0-alpha  
**Durum:** Migration öncesi (Entity modelleri tamamlandı)