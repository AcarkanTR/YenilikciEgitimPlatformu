using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YenilikciEgitimPlatformu.Models;
using YenilikciEgitimPlatformu.Models.Identity;

namespace YenilikciEgitimPlatformu.Data;

/*
 * DbSeeder - Veritabanı İlk Veri Yükleyicisi
 * 
 * Bu sınıf veritabanına ilk verileri (seed data) yükler.
 * 
 * Program.cs'de otomatik çalıştırılır:
 * - await DbSeeder.SeedAsync(context, userManager, roleManager);
 * 
 * Yüklenen Veriler:
 * 1. Roller (Admin, Moderator, User)
 * 2. Admin Kullanıcı
 * 3. İller (81 il)
 * 4. Proje Kategorileri
 * 5. Duyuru Kategorileri
 * 6. Rozetler (Başlangıç rozetleri)
 * 
 * NOT: Production'da seed data dikkatli kullanılmalı!
 * Sadece ilk kurulumda çalıştırılmalı.
 */

public static class DbSeeder
{
    #region Ana Seed Metodu

    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        try
        {
            // Sıralama önemli!
            await SeedRolesAsync(roleManager);
            await SeedAdminUserAsync(userManager);

            // Admin kullanıcısını bul (FK hatasını önlemek için ID'sini kullanacağız)
            var adminUser = await userManager.FindByEmailAsync("yep@admin.com");
            var adminId = adminUser?.Id ?? "system"; // Bulunamazsa fallback

            await SeedIllerAsync(context);

            // Parametre olarak adminId gönderiyoruz
            await SeedProjeKategorileriAsync(context, adminId);

            await SeedDuyuruKategorileriAsync(context);
            await SeedRozetlerAsync(context);

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Seed Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    #endregion

    #region Roller (Roles)

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roller = { "Admin", "Moderator", "User" };

        foreach (var rol in roller)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
                Console.WriteLine($"✅ Rol oluşturuldu: {rol}");
            }
        }
    }

    #endregion

    #region Admin Kullanıcı

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = "yep@admin.com";
        var password = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            // Kullanıcı yoksa sıfırdan oluştur
            var newAdmin = new ApplicationUser
            {
                UserName = "Yonetici",
                Email = adminEmail,
                EmailConfirmed = true, // Giriş için önemli!
                Ad = "Sistem",
                Soyad = "Yöneticisi",
                KullaniciTipi = KullaniciTipi.Standart,
                AktifMi = true,
                KayitTarihi = DateTime.UtcNow,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(newAdmin, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✅ Admin kullanıcı oluşturuldu: {adminEmail}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Admin oluşturma hatası: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                Console.ResetColor();
            }
        }
        else
        {
            // Kullanıcı zaten varsa: Onarma İşlemleri
            Console.WriteLine($"ℹ️ Admin kullanıcı ({adminEmail}) zaten var. Bilgileri kontrol ediliyor...");

            // 1. Email Onayını Kontrol Et
            if (!admin.EmailConfirmed)
            {
                admin.EmailConfirmed = true;
                await userManager.UpdateAsync(admin);
                Console.WriteLine("✅ Email onayı 'True' olarak düzeltildi.");
            }

            // 2. Şifreyi Kontrol Et ve Gerekirse Sıfırla
            if (!await userManager.CheckPasswordAsync(admin, password))
            {
                Console.WriteLine("⚠️ Şifre uyuşmuyor, sıfırlanıyor...");

                // Mevcut şifreyi kaldır
                var removeResult = await userManager.RemovePasswordAsync(admin);
                if (removeResult.Succeeded)
                {
                    // Yeni şifreyi ata
                    var addResult = await userManager.AddPasswordAsync(admin, password);
                    if (addResult.Succeeded)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"✅ Şifre başarıyla '{password}' olarak güncellendi.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"❌ Yeni şifre atanamadı: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Eski şifre kaldırılamadı.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine("✅ Şifre zaten doğru.");
            }

            // 3. Rolü Kontrol Et
            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                Console.WriteLine("✅ 'Admin' rolü eksikti, eklendi.");
            }
        }
    }

    #endregion

    #region İller (81 İl)

    private static async Task SeedIllerAsync(ApplicationDbContext context)
    {
        if (await context.Iller.AnyAsync())
            return; // Zaten var

        var iller = new List<Il>
        {
            new Il { Ad = "Adana", PlakaKodu = 1, Bolge = "Akdeniz" },
            new Il { Ad = "Adıyaman", PlakaKodu = 2, Bolge = "Güneydoğu Anadolu" },
            new Il { Ad = "Afyonkarahisar", PlakaKodu = 3, Bolge = "Ege" },
            new Il { Ad = "Ağrı", PlakaKodu = 4, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Amasya", PlakaKodu = 5, Bolge = "Karadeniz" },
            new Il { Ad = "Ankara", PlakaKodu = 6, Bolge = "İç Anadolu" },
            new Il { Ad = "Antalya", PlakaKodu = 7, Bolge = "Akdeniz" },
            new Il { Ad = "Artvin", PlakaKodu = 8, Bolge = "Karadeniz" },
            new Il { Ad = "Aydın", PlakaKodu = 9, Bolge = "Ege" },
            new Il { Ad = "Balıkesir", PlakaKodu = 10, Bolge = "Marmara" },
            new Il { Ad = "Bilecik", PlakaKodu = 11, Bolge = "Marmara" },
            new Il { Ad = "Bingöl", PlakaKodu = 12, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Bitlis", PlakaKodu = 13, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Bolu", PlakaKodu = 14, Bolge = "Karadeniz" },
            new Il { Ad = "Burdur", PlakaKodu = 15, Bolge = "Akdeniz" },
            new Il { Ad = "Bursa", PlakaKodu = 16, Bolge = "Marmara" },
            new Il { Ad = "Çanakkale", PlakaKodu = 17, Bolge = "Marmara" },
            new Il { Ad = "Çankırı", PlakaKodu = 18, Bolge = "İç Anadolu" },
            new Il { Ad = "Çorum", PlakaKodu = 19, Bolge = "Karadeniz" },
            new Il { Ad = "Denizli", PlakaKodu = 20, Bolge = "Ege" },
            new Il { Ad = "Diyarbakır", PlakaKodu = 21, Bolge = "Güneydoğu Anadolu" },
            new Il { Ad = "Edirne", PlakaKodu = 22, Bolge = "Marmara" },
            new Il { Ad = "Elazığ", PlakaKodu = 23, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Erzincan", PlakaKodu = 24, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Erzurum", PlakaKodu = 25, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Eskişehir", PlakaKodu = 26, Bolge = "İç Anadolu" },
            new Il { Ad = "Gaziantep", PlakaKodu = 27, Bolge = "Güneydoğu Anadolu" },
            new Il { Ad = "Giresun", PlakaKodu = 28, Bolge = "Karadeniz" },
            new Il { Ad = "Gümüşhane", PlakaKodu = 29, Bolge = "Karadeniz" },
            new Il { Ad = "Hakkari", PlakaKodu = 30, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Hatay", PlakaKodu = 31, Bolge = "Akdeniz" },
            new Il { Ad = "Isparta", PlakaKodu = 32, Bolge = "Akdeniz" },
            new Il { Ad = "Mersin", PlakaKodu = 33, Bolge = "Akdeniz" },
            new Il { Ad = "İstanbul", PlakaKodu = 34, Bolge = "Marmara" },
            new Il { Ad = "İzmir", PlakaKodu = 35, Bolge = "Ege" },
            new Il { Ad = "Kars", PlakaKodu = 36, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Kastamonu", PlakaKodu = 37, Bolge = "Karadeniz" },
            new Il { Ad = "Kayseri", PlakaKodu = 38, Bolge = "İç Anadolu" },
            new Il { Ad = "Kırklareli", PlakaKodu = 39, Bolge = "Marmara" },
            new Il { Ad = "Kırşehir", PlakaKodu = 40, Bolge = "İç Anadolu" },
            new Il { Ad = "Kocaeli", PlakaKodu = 41, Bolge = "Marmara" },
            new Il { Ad = "Konya", PlakaKodu = 42, Bolge = "İç Anadolu" },
            new Il { Ad = "Kütahya", PlakaKodu = 43, Bolge = "Ege" },
            new Il { Ad = "Malatya", PlakaKodu = 44, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Manisa", PlakaKodu = 45, Bolge = "Ege" },
            new Il { Ad = "Kahramanmaraş", PlakaKodu = 46, Bolge = "Akdeniz" },
            new Il { Ad = "Mardin", PlakaKodu = 47, Bolge = "Güneydoğu Anadolu" },
            new Il { Ad = "Muğla", PlakaKodu = 48, Bolge = "Ege" },
            new Il { Ad = "Muş", PlakaKodu = 49, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Nevşehir", PlakaKodu = 50, Bolge = "İç Anadolu" },
            new Il { Ad = "Niğde", PlakaKodu = 51, Bolge = "İç Anadolu" },
            new Il { Ad = "Ordu", PlakaKodu = 52, Bolge = "Karadeniz" },
            new Il { Ad = "Rize", PlakaKodu = 53, Bolge = "Karadeniz" },
            new Il { Ad = "Sakarya", PlakaKodu = 54, Bolge = "Marmara" },
            new Il { Ad = "Samsun", PlakaKodu = 55, Bolge = "Karadeniz" },
            new Il { Ad = "Siirt", PlakaKodu = 56, Bolge = "Güneydoğu Anadolu" },
            new Il { Ad = "Sinop", PlakaKodu = 57, Bolge = "Karadeniz" },
            new Il { Ad = "Sivas", PlakaKodu = 58, Bolge = "İç Anadolu" },
            new Il { Ad = "Tekirdağ", PlakaKodu = 59, Bolge = "Marmara" },
            new Il { Ad = "Tokat", PlakaKodu = 60, Bolge = "Karadeniz" },
            new Il { Ad = "Trabzon", PlakaKodu = 61, Bolge = "Karadeniz" },
            new Il { Ad = "Tunceli", PlakaKodu = 62, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Şanlıurfa", PlakaKodu = 63, Bolge = "Güneydoğu Anadolu" },
            new Il { Ad = "Uşak", PlakaKodu = 64, Bolge = "Ege" },
            new Il { Ad = "Van", PlakaKodu = 65, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Yozgat", PlakaKodu = 66, Bolge = "İç Anadolu" },
            new Il { Ad = "Zonguldak", PlakaKodu = 67, Bolge = "Karadeniz" },
            new Il { Ad = "Aksaray", PlakaKodu = 68, Bolge = "İç Anadolu" },
            new Il { Ad = "Bayburt", PlakaKodu = 69, Bolge = "Karadeniz" },
            new Il { Ad = "Karaman", PlakaKodu = 70, Bolge = "İç Anadolu" },
            new Il { Ad = "Kırıkkale", PlakaKodu = 71, Bolge = "İç Anadolu" },
            new Il { Ad = "Batman", PlakaKodu = 72, Bolge = "Güneydoğu Anadolu" },
            new Il { Ad = "Şırnak", PlakaKodu = 73, Bolge = "Güneydoğu Anadolu" },
            new Il { Ad = "Bartın", PlakaKodu = 74, Bolge = "Karadeniz" },
            new Il { Ad = "Ardahan", PlakaKodu = 75, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Iğdır", PlakaKodu = 76, Bolge = "Doğu Anadolu" },
            new Il { Ad = "Yalova", PlakaKodu = 77, Bolge = "Marmara" },
            new Il { Ad = "Karabük", PlakaKodu = 78, Bolge = "Karadeniz" },
            new Il { Ad = "Kilis", PlakaKodu = 79, Bolge = "Güneydoğu Anadolu" },
            new Il { Ad = "Osmaniye", PlakaKodu = 80, Bolge = "Akdeniz" },
            new Il { Ad = "Düzce", PlakaKodu = 81, Bolge = "Karadeniz" }
        };

        await context.Iller.AddRangeAsync(iller);
        Console.WriteLine("✅ 81 il eklendi");
    }

    #endregion

    #region Proje Kategorileri

    // Metot imzasını değiştirdik: string adminId parametresi eklendi
    private static async Task SeedProjeKategorileriAsync(ApplicationDbContext context, string adminId)
    {
        if (await context.ProjeKategorileri.AnyAsync())
            return;

        var kategoriler = new List<ProjeKategori>
        {
            new ProjeKategori
            {
                Ad = "Bilim ve Teknoloji",
                Aciklama = "Bilimsel araştırmalar, teknoloji projeleri, yazılım geliştirme",
                Ikon = "fa-solid fa-flask",
                Renk = "blue",
                Sira = 1,
                OlusturanKullaniciId = adminId // Admin ID atanıyor
            },
            new ProjeKategori
            {
                Ad = "Sanat ve Tasarım",
                Aciklama = "Görsel sanatlar, müzik, tasarım projeleri",
                Ikon = "fa-solid fa-palette",
                Renk = "purple",
                Sira = 2,
                OlusturanKullaniciId = adminId
            },
            new ProjeKategori
            {
                Ad = "Sosyal Sorumluluk",
                Aciklama = "Toplumsal fayda, gönüllülük, yardım projeleri",
                Ikon = "fa-solid fa-hands-helping",
                Renk = "green",
                Sira = 3,
                OlusturanKullaniciId = adminId
            },
            new ProjeKategori
            {
                Ad = "Çevre ve Enerji",
                Aciklama = "Çevre koruma, sürdürülebilirlik, yenilenebilir enerji",
                Ikon = "fa-solid fa-leaf",
                Renk = "emerald",
                Sira = 4,
                OlusturanKullaniciId = adminId
            },
            new ProjeKategori
            {
                Ad = "Spor ve Sağlık",
                Aciklama = "Spor aktiviteleri, sağlık bilinci, fitness",
                Ikon = "fa-solid fa-heart",
                Renk = "red",
                Sira = 5,
                OlusturanKullaniciId = adminId
            },
            new ProjeKategori
            {
                Ad = "Edebiyat ve Dil",
                Aciklama = "Yazarlık, şiir, dil öğrenimi, edebiyat çalışmaları",
                Ikon = "fa-solid fa-book",
                Renk = "amber",
                Sira = 6,
                OlusturanKullaniciId = adminId
            },
            new ProjeKategori
            {
                Ad = "Girişimcilik",
                Aciklama = "İş fikirleri, startup projeleri, inovasyon",
                Ikon = "fa-solid fa-lightbulb",
                Renk = "yellow",
                Sira = 7,
                OlusturanKullaniciId = adminId
            },
            new ProjeKategori
            {
                Ad = "Tarih ve Kültür",
                Aciklama = "Tarihi araştırmalar, kültürel projeler, müzecilik",
                Ikon = "fa-solid fa-landmark",
                Renk = "stone",
                Sira = 8,
                OlusturanKullaniciId = adminId
            }
        };

        await context.ProjeKategorileri.AddRangeAsync(kategoriler);
        Console.WriteLine("✅ Proje kategorileri eklendi");
    }
    #endregion

    #region Duyuru Kategorileri

    private static async Task SeedDuyuruKategorileriAsync(ApplicationDbContext context)
    {
        if (await context.DuyuruKategorileri.AnyAsync())
            return;

        var kategoriler = new List<DuyuruKategori>
        {
            new DuyuruKategori { Ad = "Genel Duyuru", Ikon = "fa-solid fa-bullhorn", Renk = "blue" },
            new DuyuruKategori { Ad = "Yarışma", Ikon = "fa-solid fa-trophy", Renk = "yellow" },
            new DuyuruKategori { Ad = "Etkinlik", Ikon = "fa-solid fa-calendar", Renk = "green" },
            new DuyuruKategori { Ad = "Eğitim", Ikon = "fa-solid fa-graduation-cap", Renk = "purple" },
            new DuyuruKategori { Ad = "Güncelleme", Ikon = "fa-solid fa-sync", Renk = "gray" },
            new DuyuruKategori { Ad = "Başarı Hikayesi", Ikon = "fa-solid fa-star", Renk = "amber" }
        };

        await context.DuyuruKategorileri.AddRangeAsync(kategoriler);
        Console.WriteLine("✅ Duyuru kategorileri eklendi");
    }

    #endregion

    #region Rozetler (Başlangıç)

    private static async Task SeedRozetlerAsync(ApplicationDbContext context)
    {
        if (await context.Rozetler.AnyAsync())
            return;

        var rozetler = new List<Rozet>
        {
            // Başlangıç Rozetleri
            new Rozet
            {
                Ad = "İlk Adım",
                Aciklama = "Hoş geldin! İlk gönderini paylaştın.",
                GorselUrl = "/images/badges/ilk-adim.svg",
                Ikon = "fa-solid fa-foot-print",
                Seviye = RozetSeviye.Bronz,
                Renk = "blue",
                XPOdulu = 10,
                Kategori = RozetKategori.Baslangic,
                KosulEventAdi = "IlkGonderi",
                Sira = 1
            },
            new Rozet
            {
                Ad = "Proje Başlangıcı",
                Aciklama = "İlk projeni oluşturdun!",
                GorselUrl = "/images/badges/ilk-proje.svg",
                Ikon = "fa-solid fa-rocket",
                Seviye = RozetSeviye.Bronz,
                Renk = "green",
                XPOdulu = 50,
                Kategori = RozetKategori.Proje,
                KosulEventAdi = "IlkProje",
                Sira = 2
            },
            new Rozet
            {
                Ad = "Profil Tamamlama",
                Aciklama = "Profilini %100 tamamladın!",
                GorselUrl = "/images/badges/profil-tamamlama.svg",
                Ikon = "fa-solid fa-user-check",
                Seviye = RozetSeviye.Bronz,
                Renk = "purple",
                XPOdulu = 20,
                Kategori = RozetKategori.Baslangic,
                KosulEventAdi = "ProfilTamamlama",
                Sira = 3
            },
            new Rozet
            {
                Ad = "Takım Oyuncusu",
                Aciklama = "İlk proje ekibine katıldın!",
                GorselUrl = "/images/badges/takim-oyuncusu.svg",
                Ikon = "fa-solid fa-users",
                Seviye = RozetSeviye.Bronz,
                Renk = "teal",
                XPOdulu = 15,
                Kategori = RozetKategori.Sosyal,
                KosulEventAdi = "IlkEkipKatilimi",
                Sira = 4
            },
            new Rozet
            {
                Ad = "Yorum Ustası",
                Aciklama = "İlk yorumunu yaptın!",
                GorselUrl = "/images/badges/yorum-ustasi.svg",
                Ikon = "fa-solid fa-comment",
                Seviye = RozetSeviye.Bronz,
                Renk = "orange",
                XPOdulu = 5,
                Kategori = RozetKategori.Sosyal,
                KosulEventAdi = "IlkYorum",
                Sira = 5
            },
            // Gümüş Seviye
            new Rozet
            {
                Ad = "Proje Bitirici",
                Aciklama = "İlk projeni tamamladın!",
                GorselUrl = "/images/badges/proje-bitirici.svg",
                Ikon = "fa-solid fa-check-circle",
                Seviye = RozetSeviye.Gumus,
                Renk = "green",
                XPOdulu = 100,
                Kategori = RozetKategori.Proje,
                KosulEventAdi = "ProjeTamamlama",
                Sira = 6
            },
            new Rozet
            {
                Ad = "Sosyal Kelebek",
                Aciklama = "50 yorum yaptın!",
                GorselUrl = "/images/badges/sosyal-kelebek.svg",
                Ikon = "fa-solid fa-comments",
                Seviye = RozetSeviye.Gumus,
                Renk = "pink",
                XPOdulu = 50,
                Kategori = RozetKategori.Sosyal,
                KosulEventAdi = "50Yorum",
                KosulDegeri = 50,
                Sira = 7
            }
        };

        await context.Rozetler.AddRangeAsync(rozetler);
        Console.WriteLine("✅ Başlangıç rozetleri eklendi");
    }

    #endregion
}