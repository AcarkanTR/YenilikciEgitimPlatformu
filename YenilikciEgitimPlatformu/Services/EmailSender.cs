using Microsoft.AspNetCore.Identity.UI.Services;

namespace YenilikciEgitimPlatformu.Services
{
    // Bu sınıf IEmailSender arayüzünü implemente eder
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Şimdilik burası boş bırakıldı. 
            // Gerçek uygulamada burada SMTP veya SendGrid gibi servisler kullanılır.
            // Loglama yaparak e-postanın "gönderildiğini" konsolda görebilirsiniz.
            Console.WriteLine($"Email Gönderildi -> Kime: {email}, Konu: {subject}");
            return Task.CompletedTask;
        }
    }
}