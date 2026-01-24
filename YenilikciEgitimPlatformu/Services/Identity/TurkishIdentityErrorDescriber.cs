using Microsoft.AspNetCore.Identity;

namespace YenilikciEgitimPlatformu.Services;

/*
 * TurkishIdentityErrorDescriber - Türkçe Identity Hata Mesajları
 * 
 * Bu sınıf ASP.NET Core Identity'nin varsayılan İngilizce hata mesajlarını
 * Türkçe'ye çevirir.
 * 
 * Kullanım Alanları:
 * - Kayıt formunda şifre hataları
 * - Email çakışması
 * - Kullanıcı adı geçersizliği
 * - Şifre sıfırlama hataları
 * - Rol yönetimi hataları
 * 
 * Program.cs'de kaydedilir:
 * services.AddIdentity<ApplicationUser, IdentityRole>()
 *     .AddErrorDescriber<TurkishIdentityErrorDescriber>();
 * 
 * Not: Tüm metodlar override edilmiştir, eksik bırakılmamıştır.
 */

public class TurkishIdentityErrorDescriber : IdentityErrorDescriber
{
    #region Genel Hatalar

    public override IdentityError DefaultError()
    {
        return new IdentityError
        {
            Code = nameof(DefaultError),
            Description = "Bilinmeyen bir hata oluştu."
        };
    }

    public override IdentityError ConcurrencyFailure()
    {
        return new IdentityError
        {
            Code = nameof(ConcurrencyFailure),
            Description = "İşlem sırasında veriler değiştirildi. Lütfen tekrar deneyin."
        };
    }

    #endregion

    #region Şifre Hataları

    public override IdentityError PasswordMismatch()
    {
        return new IdentityError
        {
            Code = nameof(PasswordMismatch),
            Description = "Şifre hatalı."
        };
    }

    public new IdentityError InvalidToken()
    {
        return new IdentityError
        {
            Code = nameof(InvalidToken),
            Description = "Geçersiz token."
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Şifre en az bir rakam ('0'-'9') içermelidir."
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Şifre en az bir küçük harf ('a'-'z') içermelidir."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Şifre en az bir büyük harf ('A'-'Z') içermelidir."
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Şifre en az bir özel karakter (!@#$%^&* vb.) içermelidir."
        };
    }

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = $"Şifre en az {uniqueChars} farklı karakter içermelidir."
        };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"Şifre en az {length} karakter uzunluğunda olmalıdır."
        };
    }

    #endregion

    #region Kullanıcı Hataları

    public override IdentityError InvalidUserName(string? userName)
    {
        return new IdentityError
        {
            Code = nameof(InvalidUserName),
            Description = $"Kullanıcı adı '{userName}' geçersizdir. Kullanıcı adı sadece harf, rakam ve nokta içerebilir."
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = $"'{userName}' kullanıcı adı zaten kullanılıyor."
        };
    }

    public override IdentityError InvalidEmail(string? email)
    {
        return new IdentityError
        {
            Code = nameof(InvalidEmail),
            Description = $"E-posta adresi '{email}' geçersizdir."
        };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = $"'{email}' e-posta adresi zaten kullanılıyor."
        };
    }

    public override IdentityError UserAlreadyHasPassword()
    {
        return new IdentityError
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = "Kullanıcı zaten bir şifreye sahip."
        };
    }

    public override IdentityError UserLockoutNotEnabled()
    {
        return new IdentityError
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = "Bu kullanıcı için hesap kilitleme etkin değil."
        };
    }

    public override IdentityError UserAlreadyInRole(string role)
    {
        return new IdentityError
        {
            Code = nameof(UserAlreadyInRole),
            Description = $"Kullanıcı zaten '{role}' rolüne sahip."
        };
    }

    public override IdentityError UserNotInRole(string role)
    {
        return new IdentityError
        {
            Code = nameof(UserNotInRole),
            Description = $"Kullanıcı '{role}' rolüne sahip değil."
        };
    }

    #endregion

    #region Rol Hataları

    public override IdentityError InvalidRoleName(string? role)
    {
        return new IdentityError
        {
            Code = nameof(InvalidRoleName),
            Description = $"Rol adı '{role}' geçersizdir."
        };
    }

    public override IdentityError DuplicateRoleName(string role)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateRoleName),
            Description = $"'{role}' rolü zaten mevcut."
        };
    }

    #endregion

    #region Login Hataları

    public override IdentityError LoginAlreadyAssociated()
    {
        return new IdentityError
        {
            Code = nameof(LoginAlreadyAssociated),
            Description = "Bu giriş bilgisi zaten başka bir kullanıcıya ait."
        };
    }

    #endregion

    #region Token Hataları

    public override IdentityError RecoveryCodeRedemptionFailed()
    {
        return new IdentityError
        {
            Code = nameof(RecoveryCodeRedemptionFailed),
            Description = "Kurtarma kodu kullanılamadı."
        };
    }

    #endregion
}