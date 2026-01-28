using YenilikciEgitimPlatformu.Models;
using YenilikciEgitimPlatformu.ViewModels.ProjeYonetim;

namespace YenilikciEgitimPlatformu.Services.Interfaces;

/// <summary>
/// Proje Yönetimi (Sistem 2) servisi için interface
/// Kullanıcı projelerinin CRUD ve yönetim işlemleri
/// </summary>
public interface IProjeYonetimService
{
    #region CRUD İşlemleri

    /// <summary>
    /// Tüm projeleri filtreli şekilde getirir
    /// </summary>
    Task<(List<ProjeListViewModel> Data, int TotalCount)> GetAllAsync(ProjeFiltreleViewModel filtre);

    /// <summary>
    /// ID'ye göre proje detayını getirir
    /// </summary>
    Task<ProjeDetayViewModel?> GetByIdAsync(int id);

    /// <summary>
    /// Slug'a göre proje detayını getirir
    /// </summary>
    Task<ProjeDetayViewModel?> GetBySlugAsync(string slug);

    /// <summary>
    /// Yeni proje oluşturur
    /// </summary>
    Task<(bool Success, int? Id, string Message)> CreateAsync(ProjeOlusturViewModel model, string userId);

    /// <summary>
    /// Proje bilgilerini günceller
    /// </summary>
    Task<(bool Success, string Message)> UpdateAsync(int id, ProjeGuncelleViewModel model, string userId);

    /// <summary>
    /// Projeyi soft-delete yapar
    /// </summary>
    Task<(bool Success, string Message)> DeleteAsync(int id, string userId);

    #endregion

    #region Yetkilendirme Kontrolleri

    /// <summary>
    /// Kullanıcının projeyi düzenleme yetkisi var mı?
    /// </summary>
    Task<bool> CanUserEditAsync(int projeId, string userId);

    /// <summary>
    /// Kullanıcının projeyi silme yetkisi var mı?
    /// </summary>
    Task<bool> CanUserDeleteAsync(int projeId, string userId);

    /// <summary>
    /// Kullanıcı proje kurucusu mu?
    /// </summary>
    Task<bool> IsUserFounderAsync(int projeId, string userId);

    #endregion

    #region Ekip Yönetimi

    /// <summary>
    /// Projeye ekip üyesi ekler
    /// </summary>
    Task<(bool Success, string Message)> AddTeamMemberAsync(int projeId, string uyeUserId, ProjeRol rol, string userId);

    /// <summary>
    /// Ekip üyesini projeden çıkarır
    /// </summary>
    Task<(bool Success, string Message)> RemoveTeamMemberAsync(int projeId, string uyeUserId, string userId);

    /// <summary>
    /// Ekip üyesinin rolünü değiştirir
    /// </summary>
    Task<(bool Success, string Message)> ChangeTeamMemberRoleAsync(int projeId, string uyeUserId, ProjeRol yeniRol, string userId);

    /// <summary>
    /// Projenin tüm ekip üyelerini getirir
    /// </summary>
    Task<List<EkipUyesiViewModel>> GetTeamMembersAsync(int projeId);

    #endregion

    #region İstatistikler

    /// <summary>
    /// Kullanıcının toplam proje sayısı
    /// </summary>
    Task<int> GetUserProjectCountAsync(string userId);

    /// <summary>
    /// Proje görüntüleme sayısını artırır
    /// </summary>
    Task IncrementViewCountAsync(int projeId);

    /// <summary>
    /// Popüler projeleri getirir
    /// </summary>
    Task<List<ProjeListViewModel>> GetPopularProjectsAsync(int adet = 5);

    /// <summary>
    /// Son eklenen projeleri getirir
    /// </summary>
    Task<List<ProjeListViewModel>> GetRecentProjectsAsync(int adet = 5);

    #endregion

    #region Yardımcı Metotlar

    /// <summary>
    /// Benzersiz slug oluşturur
    /// </summary>
    Task<string> GenerateUniqueSlugAsync(string baslik);

    /// <summary>
    /// Proje durumunu günceller
    /// </summary>
    Task<(bool Success, string Message)> UpdateStatusAsync(int projeId, ProjeDurumu yeniDurum, string userId);

    #endregion
}