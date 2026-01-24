using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace YenilikciEgitimPlatformu.Hubs;

/*
 * BildirimHub - SignalR Hub
 * 
 * Gerçek zamanlı bildirim sistemi için WebSocket hub'ı
 * 
 * Kullanım Alanları:
 * - Anlık bildirimler (Yeni yorum, beğeni, mention)
 * - Proje güncellemeleri
 * - Görev atamaları
 * - Sistem duyuruları
 * - Seviye atlama bildirimleri
 * - Rozet kazanma bildirimleri
 * 
 * Client Tarafı (JavaScript):
 * const connection = new signalR.HubConnectionBuilder()
 *     .withUrl("/bildirimHub")
 *     .build();
 * 
 * connection.on("BildirimAl", (bildirim) => {
 *     // Toast göster
 * });
 * 
 * Program.cs'de Endpoint:
 * app.MapHub<BildirimHub>("/bildirimHub");
 */

[Authorize] // Sadece giriş yapmış kullanıcılar bağlanabilir
public class BildirimHub : Hub
{
    #region Connection Yönetimi

    /*
     * Kullanıcı hub'a bağlandığında
     * 
     * Kullanıcı ID'sine göre grup oluşturulur
     * Böylece belirli kullanıcıya özel mesaj gönderilebilir
     */

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userId))
        {
            // Kullanıcıyı kendi grubuna ekle
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

            // Online durumunu güncelle (opsiyonel)
            // await UpdateUserOnlineStatus(userId, true);

            Console.WriteLine($"✅ Kullanıcı bağlandı: {userId} (ConnectionId: {Context.ConnectionId})");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userId))
        {
            // Kullanıcıyı gruptan çıkar
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");

            // Offline durumunu güncelle (opsiyonel)
            // await UpdateUserOnlineStatus(userId, false);

            Console.WriteLine($"❌ Kullanıcı bağlantıyı kesti: {userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    #endregion

    #region Bildirim Gönderme Metodları (Server -> Client)

    /*
     * Bu metodlar Service Layer'dan çağrılır
     * 
     * Örnek:
     * await _hubContext.Clients.Group($"user_{aliciId}")
     *     .SendAsync("BildirimAl", bildirimDto);
     */

    /// <summary>
    /// Belirli bir kullanıcıya bildirim gönder
    /// </summary>
    public static async Task BildirimGonderAsync(
        IHubContext<BildirimHub> hubContext,
        string aliciKullaniciId,
        object bildirim)
    {
        await hubContext.Clients.Group($"user_{aliciKullaniciId}")
            .SendAsync("BildirimAl", bildirim);
    }

    /// <summary>
    /// Tüm kullanıcılara broadcast bildirim
    /// </summary>
    public static async Task HerkeseBildirimGonderAsync(
        IHubContext<BildirimHub> hubContext,
        object bildirim)
    {
        await hubContext.Clients.All
            .SendAsync("SistemBildirimi", bildirim);
    }

    /// <summary>
    /// Belirli bir gruba bildirim gönder
    /// Örnek: Proje ekibine bildirim
    /// </summary>
    public static async Task GrupBildirimGonderAsync(
        IHubContext<BildirimHub> hubContext,
        string grupAdi,
        object bildirim)
    {
        await hubContext.Clients.Group(grupAdi)
            .SendAsync("GrupBildirimi", bildirim);
    }

    #endregion

    #region Client'tan Gelen Metodlar (Client -> Server)

    /*
     * Client tarafından çağrılabilen metodlar
     * 
     * JavaScript:
     * await connection.invoke("BildirimleriOkunduOlarakIsaretle", [1, 2, 3]);
     */

    /// <summary>
    /// Bildirimleri okundu olarak işaretle
    /// </summary>
    public async Task BildirimleriOkunduOlarakIsaretle(List<int> bildirimIdler)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return;

        // TODO: Service katmanında bildirimleri okundu yap
        // await _bildirimService.OkunduIsaretle(bildirimIdler, userId);

        // Client'a onay gönder
        await Clients.Caller.SendAsync("BildirimlerOkundu", bildirimIdler);
    }

    /// <summary>
    /// Typing indicator (Yazıyor bildirimi)
    /// </summary>
    public async Task YaziyorBildirimi(string projeId, bool yaziyor)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = Context.User?.Identity?.Name;

        if (string.IsNullOrEmpty(userId))
            return;

        // Aynı projedeki diğer kullanıcılara gönder
        await Clients.Group($"proje_{projeId}")
            .SendAsync("KullaniciYaziyor", new
            {
                KullaniciId = userId,
                KullaniciAdi = userName,
                Yaziyor = yaziyor
            });
    }

    /// <summary>
    /// Proje grubuna katıl
    /// </summary>
    public async Task ProjeGrubunaKatil(int projeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"proje_{projeId}");
        Console.WriteLine($"Kullanıcı proje grubuna katıldı: {projeId}");
    }

    /// <summary>
    /// Proje grubundan ayrıl
    /// </summary>
    public async Task ProjeGrubundanAyril(int projeId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"proje_{projeId}");
        Console.WriteLine($"Kullanıcı proje grubundan ayrıldı: {projeId}");
    }

    #endregion

    #region Yardımcı Metodlar

    /// <summary>
    /// Kullanıcının online durumunu güncelle (Opsiyonel)
    /// </summary>
    private async Task UpdateUserOnlineStatus(string userId, bool online)
    {
        // TODO: Cache veya DB'de kullanıcının online durumunu güncelle
        // await _cacheService.SetAsync($"user_online_{userId}", online);
        await Task.CompletedTask;
    }

    #endregion
}

#region Bildirim DTO Modelleri

/*
 * SignalR üzerinden gönderilecek bildirim modelleri
 * 
 * Bu DTO'lar Entity modellerinden ayrıdır
 * Sadece client'a gönderilecek minimum bilgiyi içerir
 */

/// <summary>
/// Gerçek zamanlı bildirim DTO
/// </summary>
public class BildirimDto
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Mesaj { get; set; } = string.Empty;
    public string Ikon { get; set; } = "fa-solid fa-bell";
    public string Renk { get; set; } = "blue";
    public string? HedefUrl { get; set; }
    public DateTime OlusturulmaTarihi { get; set; }
    public string Turu { get; set; } = string.Empty;
}

/// <summary>
/// Seviye atlama bildirimi
/// </summary>
public class SeviyeAtlamaBildirimDto
{
    public int YeniSeviye { get; set; }
    public int KazanilanXP { get; set; }
    public int ToplamXP { get; set; }
    public string Mesaj { get; set; } = string.Empty;
    public string AnimasyonUrl { get; set; } = "/animations/level-up.json"; // Lottie animasyon
}

/// <summary>
/// Rozet kazanma bildirimi
/// </summary>
public class RozetKazanimBildirimDto
{
    public string RozetAdi { get; set; } = string.Empty;
    public string RozetAciklama { get; set; } = string.Empty;
    public string RozetGorselUrl { get; set; } = string.Empty;
    public int KazanilanXP { get; set; }
    public string Seviye { get; set; } = string.Empty; // Bronze, Silver, Gold
}

#endregion