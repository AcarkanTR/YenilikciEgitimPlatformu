using YenilikciEgitimPlatformu.Models;
using YenilikciEgitimPlatformu.ViewModels.CagriBilgisi;

namespace YenilikciEgitimPlatformu.Services.Interfaces;

/// <summary>
/// Çağrı Bilgisi (Sistem 1) servisi için interface
/// Resmi kurum çağrılarının yönetimi
/// </summary>
public interface ICagriBilgisiService
{
    #region CRUD İşlemleri

    /// <summary>
    /// Tüm çağrıları filtreli şekilde getirir
    /// </summary>
    Task<(List<CagriListViewModel> Data, int TotalCount)> GetAllAsync(CagriFiltreleViewModel filtre);

    /// <summary>
    /// ID'ye göre çağrı detayını getirir
    /// </summary>
    Task<CagriDetayViewModel?> GetByIdAsync(int id);

    /// <summary>
    /// Slug'a göre çağrı detayını getirir (SEO-friendly URL)
    /// </summary>
    Task<CagriDetayViewModel?> GetBySlugAsync(string slug);

    /// <summary>
    /// Yeni çağrı oluşturur (Admin)
    /// </summary>
    Task<(bool Success, int? Id, string Message)> CreateAsync(CagriOlusturViewModel model, string userId);

    /// <summary>
    /// Çağrı bilgilerini günceller (Admin)
    /// </summary>
    Task<(bool Success, string Message)> UpdateAsync(int id, CagriGuncelleViewModel model, string userId);

    /// <summary>
    /// Çağrıyı soft-delete yapar (Admin)
    /// </summary>
    Task<(bool Success, string Message)> DeleteAsync(int id, string userId);

    #endregion

    #region Takip İşlemleri

    /// <summary>
    /// Kullanıcı çağrıyı takip eder
    /// </summary>
    Task<(bool Success, string Message)> TakipEtAsync(int cagriId, string userId);

    /// <summary>
    /// Kullanıcı çağrı takibini bırakır
    /// </summary>
    Task<(bool Success, string Message)> TakipBirakAsync(int cagriId, string userId);

    /// <summary>
    /// Kullanıcının bu çağrıyı takip edip etmediğini kontrol eder
    /// </summary>
    Task<bool> TakipEdiyorMuAsync(int cagriId, string userId);

    /// <summary>
    /// Çağrının takipçi sayısını getirir
    /// </summary>
    Task<int> GetTakipciSayisiAsync(int cagriId);

    #endregion

    #region Arama ve Filtreleme

    /// <summary>
    /// Anahtar kelimeye göre çağrı arar
    /// </summary>
    Task<List<CagriListViewModel>> SearchAsync(string keyword, int maxResults = 10);

    /// <summary>
    /// Popüler (en çok takip edilen) çağrıları getirir
    /// </summary>
    Task<List<CagriListViewModel>> GetPopulerCagrilarAsync(int adet = 5);

    /// <summary>
    /// Yaklaşan son başvuru tarihli çağrıları getirir
    /// </summary>
    Task<List<CagriListViewModel>> GetYaklasanCagrilarAsync(int adet = 5);

    #endregion

    #region Yardımcı Metotlar

    /// <summary>
    /// Çağrı görüntüleme sayısını artırır
    /// </summary>
    Task IncrementViewCountAsync(int cagriId);

    /// <summary>
    /// Benzersiz slug oluşturur
    /// </summary>
    Task<string> GenerateUniqueSlugAsync(string baslik);

    #endregion
}