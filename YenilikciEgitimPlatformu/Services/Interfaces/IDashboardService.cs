using YenilikciEgitimPlatformu.ViewModels.Dashboard;

namespace YenilikciEgitimPlatformu.Services.Interfaces;

/// <summary>
/// Dashboard servis interface'i
/// Admin ve User dashboard'ları için istatistik ve özet bilgiler sağlar
/// </summary>
public interface IDashboardService
{
    #region Admin Dashboard

    /// <summary>
    /// Admin dashboard için genel sistem istatistiklerini getirir
    /// </summary>
    Task<AdminDashboardViewModel> GetAdminDashboardDataAsync();

    /// <summary>
    /// Aylık kullanıcı kayıt istatistiklerini getirir (Chart.js için)
    /// </summary>
    Task<List<AylikIstatistikViewModel>> GetAylikKullaniciKayitlariAsync(int sonKacAy = 6);

    /// <summary>
    /// En aktif kullanıcıları getirir
    /// </summary>
    Task<List<AktifKullaniciViewModel>> GetEnAktifKullanicilarAsync(int adet = 10);

    /// <summary>
    /// Bekleyen onay sayılarını getirir
    /// </summary>
    Task<BekleyenOnaylarViewModel> GetBekleyenOnaylarAsync();

    #endregion

    #region User Dashboard

    /// <summary>
    /// User dashboard için kişisel istatistikleri getirir
    /// </summary>
    Task<UserDashboardViewModel> GetUserDashboardDataAsync(string userId);

    /// <summary>
    /// Kullanıcının aylık aktivite istatistiklerini getirir
    /// </summary>
    Task<List<AylikIstatistikViewModel>> GetKullaniciAylikAktiviteAsync(string userId, int sonKacAy = 6);

    /// <summary>
    /// Kullanıcının son aktivitelerini getirir
    /// </summary>
    Task<List<AktiviteViewModel>> GetKullaniciSonAktivitelerAsync(string userId, int adet = 10);

    #endregion

    #region Ortak Metotlar

    /// <summary>
    /// Proje durum dağılımını getirir (Chart.js Pie Chart için)
    /// </summary>
    Task<List<DurumDagilimViewModel>> GetProjeDurumDagilimAsync(string? userId = null);

    /// <summary>
    /// Kategori bazlı içerik dağılımını getirir
    /// </summary>
    Task<List<KategoriDagilimViewModel>> GetKategoriDagilimAsync();

    #endregion
}