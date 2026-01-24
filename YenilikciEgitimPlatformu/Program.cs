using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YenilikciEgitimPlatformu.Data;
using YenilikciEgitimPlatformu.Models.Identity;
//using YenilikciEgitimPlatformu.Services.Interfaces;
using YenilikciEgitimPlatformu.Services;
using YenilikciEgitimPlatformu.Hubs;
using Serilog;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

#region Program Başlangıç ve Loglama Yapılandırması
/*
 * YenilikciEgitimPlatformu - Program.cs
 * 
 * Bu dosya uygulamanın ana giriş noktasıdır ve aşağıdaki kritik yapılandırmaları içerir:
 * - Veritabanı bağlantısı (SQL Server)
 * - ASP.NET Core Identity (Kullanıcı yönetimi ve yetkilendirme)
 * - Dependency Injection (Servis kayıtları)
 * - Middleware Pipeline (HTTP request işleme hattı)
 * - Serilog (Merkezi loglama sistemi)
 * - Localization (Türkçe dil desteği)
 * - Güvenlik ayarları (HTTPS, Cookie, CORS)
 * 
 * Mimari: Clean/Onion Architecture prensipleri ile katmanlı yapı
 * Framework: .NET 10 LTS
 * UI: Razor Pages
 */

// Serilog yapılandırması - Tüm sistem logları merkezi olarak yönetilir
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    Log.Information("🚀 Yenilikçi Eğitim Platformu başlatılıyor...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog'u ASP.NET Core logging sistemine entegre et
    builder.Host.UseSerilog();

    #endregion

    #region Veritabanı ve Identity Yapılandırması
    /*
     * SQL Server bağlantısı ve ASP.NET Core Identity yapılandırması
     * 
     * ApplicationDbContext: EF Core DbContext sınıfı (tüm entity'ler burada yönetilir)
     * ApplicationUser: Identity'yi genişletilmiş kullanıcı modeli
     * 
     * Identity Options:
     * - Şifre gereksinimleri
     * - Email doğrulama
     * - Kullanıcı kilitleme ayarları
     * - Oturum yönetimi
     */

    // SQL Server bağlantı stringi appsettings.json'dan okunur
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("'DefaultConnection' bağlantı stringi bulunamadı!");

    // DbContext kaydı - SQL Server kullanımı
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            connectionString,
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null
                );
                sqlOptions.CommandTimeout(30); // 30 saniye timeout
            }
        ));

    // Database geliştirme hatalarını göster (Development ortamında)
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    // ASP.NET Core Identity yapılandırması
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        #region Şifre Gereksinimleri
        // Güvenli ama kullanıcı dostu şifre politikası
        options.Password.RequireDigit = true;              // En az 1 rakam
        options.Password.RequireLowercase = true;          // En az 1 küçük harf
        options.Password.RequireUppercase = true;          // En az 1 büyük harf
        options.Password.RequireNonAlphanumeric = false;   // Özel karakter zorunlu değil (kullanıcı deneyimi için)
        options.Password.RequiredLength = 6;               // Minimum 6 karakter
        options.Password.RequiredUniqueChars = 1;
        #endregion

        #region Kullanıcı Ayarları
        options.User.RequireUniqueEmail = true; // Her email benzersiz olmalı
        #endregion

        #region Email Doğrulama
        // V1'de email doğrulama pasif (2FA için altyapı hazır)
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        #endregion

        #region Hesap Kilitleme (Brute Force Koruması)
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // 15 dakika kilitleme
        options.Lockout.MaxFailedAccessAttempts = 5;                       // 5 yanlış denemeden sonra
        options.Lockout.AllowedForNewUsers = true;
        #endregion
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<TurkishIdentityErrorDescriber>(); // Türkçe hata mesajları

    #endregion

    #region Cookie ve Oturum Ayarları
    /*
     * Cookie tabanlı authentication yapılandırması
     * 
     * - Login sayfası yönlendirmeleri
     * - Access Denied sayfası
     * - Cookie süresi ve güvenlik ayarları
     */

    builder.Services.ConfigureApplicationCookie(options =>
    {
        // Yönlendirme sayfaları
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";

        // Cookie ayarları
        options.Cookie.Name = "YEP.Auth"; // YEP: Yenilikçi Eğitim Platformu
        options.Cookie.HttpOnly = true;    // XSS koruması
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Sadece HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax;

        // Oturum süresi
        options.ExpireTimeSpan = TimeSpan.FromDays(30);    // 30 gün hatırla
        options.SlidingExpiration = true;                   // Sliding window (aktif kullanımda süre uzar)
    });

    #endregion

    #region Dependency Injection - Service Layer Kayıtları
    /*
     * Tüm servisler burada DI container'a kaydedilir
     * 
     * Scoped: Her HTTP request için yeni instance
     * Singleton: Uygulama boyunca tek instance
     * Transient: Her çağrıda yeni instance
     * 
     * Pattern: Interface -> Implementation (Test edilebilir kod için)
     */

    // IEmailSender istendiğinde EmailSender sınıfını kullan demektir.
    builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();

    /*
    // Kullanıcı ve Profil Servisleri
    builder.Services.AddScoped<IKullaniciService, KullaniciService>();
    builder.Services.AddScoped<IProfilService, ProfilService>();

    // İçerik ve Etkileşim Servisleri
    builder.Services.AddScoped<IDuyuruService, DuyuruService>();
    builder.Services.AddScoped<IEtkinlikService, EtkinlikService>();
    builder.Services.AddScoped<IGonderiService, GonderiService>();
    builder.Services.AddScoped<IYorumService, YorumService>();

    // Proje Yönetimi Servisleri
    builder.Services.AddScoped<IProjeService, ProjeService>();
    builder.Services.AddScoped<IGorevService, GorevService>();

    // Bildirim ve Sistem Servisleri
    builder.Services.AddScoped<IBildirimService, BildirimService>();
    builder.Services.AddScoped<IOyunlastirmaService, OyunlastirmaService>();
    builder.Services.AddScoped<IDosyaService, DosyaService>();

    // Okul ve Konum Servisleri
    builder.Services.AddScoped<IOkulService, OkulService>();
    builder.Services.AddScoped<IIlIlceService, IlIlceService>();

    // Yardımcı Servisler (Utility)
    builder.Services.AddSingleton<IEmailService, EmailService>(); // Singleton: Email gönderimi için pool kullanımı
    builder.Services.AddScoped<IAuditService, AuditService>();
    builder.Services.AddScoped<IModerasyonService, ModerasyonService>();
    */
    #endregion

    #region SignalR Yapılandırması
    /*
     * Gerçek zamanlı bildirimler için SignalR
     * 
     * Hub: BildirimHub (kullanıcılara anlık bildirim gönderir)
     */

    builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    });

    #endregion

    #region Redis Cache Yapılandırması (Opsiyonel - V1'de basit Memory Cache)
    /*
     * V1: In-Memory Cache (basit ve hızlı)
     * V2: Redis (dağıtık sistemler için)
     * 
     * Cache kullanım alanları:
     * - Sık erişilen statik veriler (İl/İlçe listesi)
     * - Kullanıcı profil bilgileri
     * - Okul listesi
     */

    if (builder.Configuration.GetValue<bool>("UseRedis"))
    {
        // Redis kullan (Production ortamı için)
        var redisConnection = builder.Configuration.GetConnectionString("Redis");
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "YEP:";
        });
        Log.Information("✅ Redis Cache aktif");
    }
    else
    {
        // Memory Cache kullan (Development ve V1 için yeterli)
        builder.Services.AddMemoryCache();
        Log.Information("✅ Memory Cache aktif (Development)");
    }

    #endregion

    #region Localization (Türkçe Dil Desteği)
    /*
     * Uygulama Türkçe olarak çalışır
     * 
     * - Tarih/saat formatları
     * - Sayı formatları
     * - Para birimi formatları
     * - Validation mesajları
     */

    var supportedCultures = new[] { new CultureInfo("tr-TR") };
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new RequestCulture("tr-TR");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
    });

    // Türkçe validation mesajları için
    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

    #endregion

    #region Razor Pages ve MVC Yapılandırması
    /*
     * Razor Pages + JSON serialization ayarları
     * 
     * - Antiforgery token (CSRF koruması)
     * - Model binding
     * - JSON döngüsel referans yönetimi
     */

    builder.Services.AddRazorPages(options =>
    {
        // Yetkilendirme gerektiren klasörler
        options.Conventions.AuthorizeFolder("/Dashboard");
        options.Conventions.AuthorizeFolder("/Projeler");
        options.Conventions.AuthorizeFolder("/Profil");

        // Admin sayfaları için özel yetkilendirme
        options.Conventions.AuthorizeFolder("/Admin", "AdminPolicy");

        // Antiforgery token ayarları (CSRF koruması)
        options.Conventions.ConfigureFilter(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
    })
    .AddJsonOptions(options =>
    {
        // JSON serialization ayarları (API response için)
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

    #endregion

    #region Authorization Policies
    /*
     * Rol ve Claim tabanlı yetkilendirme politikaları
     * 
     * Roller:
     * - Admin: Sistem yöneticisi (tüm yetkiler)
     * - Moderator: İçerik moderasyonu
     * - Teacher: Öğretmen (proje yönetimi, öğrenci yönetimi)
     * - Student: Öğrenci (temel üyelik)
     * - User: Standart kullanıcı (sınırlı erişim)
     */

    builder.Services.AddAuthorization(options =>
    {
        // Admin Policy: Sadece Admin rolüne sahip kullanıcılar
        options.AddPolicy("AdminPolicy", policy =>
            policy.RequireRole("Admin"));

        // Moderator Policy: Admin veya Moderator
        options.AddPolicy("ModeratorPolicy", policy =>
            policy.RequireRole("Admin", "Moderator"));

        // Teacher Policy: Admin, Moderator veya Teacher
        options.AddPolicy("TeacherPolicy", policy =>
            policy.RequireRole("Admin", "Moderator", "Teacher"));

        // Verified User Policy: Email doğrulaması yapılmış kullanıcılar (V2 için hazır)
        options.AddPolicy("VerifiedUserPolicy", policy =>
            policy.RequireAuthenticatedUser());
        // .RequireClaim("EmailVerified", "true")); // V2'de aktif edilecek
    });

    #endregion

    #region HTTPS ve Güvenlik Ayarları
    /*
     * Production güvenlik gereksinimleri
     * 
     * - HTTPS yönlendirmesi
     * - HSTS (HTTP Strict Transport Security)
     * - CORS ayarları (gerekirse)
     */

    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365);
        options.IncludeSubDomains = true;
        options.Preload = true;
    });

    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
        options.HttpsPort = 443;
    });

    #endregion

    #region Application Build
    /*
     * Builder'dan app instance'ı oluştur
     * Middleware pipeline aşağıda yapılandırılır
     */

    var app = builder.Build();

    #endregion

    #region Database Migration ve Seed (İlk Çalıştırma)
    /*
     * Uygulama başlangıcında veritabanı kontrolü
     * 
     * - Migration'lar otomatik uygulanır
     * - Seed data eklenir (Admin kullanıcı, roller, test verileri)
     * 
     * ⚠️ Production'da migration otomatik uygulanmamalı!
     * Bu kod sadece Development ortamında çalışmalı.
     */

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Development ortamında pending migration'ları otomatik uygula
            if (app.Environment.IsDevelopment())
            {
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    Log.Information($"🔄 {pendingMigrations.Count()} adet pending migration uygulanıyor...");
                    await context.Database.MigrateAsync();
                    Log.Information("✅ Migration tamamlandı");
                }
            }

            // Seed data çalıştır (İlk roller ve admin kullanıcı)
            await DbSeeder.SeedAsync(context, userManager, roleManager);
            Log.Information("✅ Veritabanı seed işlemi tamamlandı");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "❌ Veritabanı migration/seed hatası!");
            throw;
        }
    }

    #endregion

    #region Middleware Pipeline
    /*
     * HTTP Request işleme hattı (sıralama ÖNEMLİ!)
     * 
     * İstek akışı:
     * 1. HTTPS Redirect
     * 2. HSTS
     * 3. Static Files (CSS, JS, images)
     * 4. Routing
     * 5. Authentication
     * 6. Authorization
     * 7. Endpoint (Razor Page çalıştırılır)
     */

    // Hata yönetimi (Environment'a göre)
    if (app.Environment.IsDevelopment())
    {
        // Development: Detaylı hata sayfası
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint(); // Database error page
    }
    else
    {
        // Production: Kullanıcı dostu hata sayfası
        app.UseExceptionHandler("/Error");
        app.UseHsts(); // HSTS (HTTP Strict Transport Security)
    }

    // HTTPS yönlendirmesi (HTTP -> HTTPS)
    app.UseHttpsRedirection();

    // Static files (wwwroot klasörü)
    app.UseStaticFiles();

    // Localization (Türkçe dil desteği)
    app.UseRequestLocalization();

    // Routing middleware
    app.UseRouting();

    // Authentication & Authorization (sıralama önemli!)
    app.UseAuthentication(); // Kim olduğunu belirle
    app.UseAuthorization();  // Yetkisini kontrol et

    // Serilog HTTP request logging
    app.UseSerilogRequestLogging();

    // Endpoint mapping
    app.MapRazorPages();

    // SignalR Hub endpoint
    app.MapHub<BildirimHub>("/bildirimHub");

    #endregion

    #region Uygulama Başlatma
    /*
     * Uygulama çalıştırılır ve port dinlemeye başlanır
     */

    Log.Information("✅ Yenilikçi Eğitim Platformu başarıyla başlatıldı!");
    Log.Information($"🌐 Ortam: {app.Environment.EnvironmentName}");
    Log.Information($"🔗 URL: {builder.Configuration["ApplicationUrl"] ?? "https://localhost:5001"}");

    await app.RunAsync();

    #endregion
}
catch (Exception ex)
{
    // Kritik başlatma hatası
    Log.Fatal(ex, "❌ Uygulama başlatılamadı!");
    throw;
}
finally
{
    // Uygulama kapatılırken logları temizle
    Log.Information("🛑 Yenilikçi Eğitim Platformu kapatılıyor...");
    await Log.CloseAndFlushAsync();
}