using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Domain.Models.ENTITY
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Benzersiz kullanıcı kimliği

        public string? UserName { get; set; } // Kullanıcının adı

        public string? LastName { get; set; } // Kullanıcının soyadı

        public string? NormalizedUserName { get; set; } // Kullanıcı adının normalize edilmiş hali

        public string? NormalizedLastName { get; set; } // Kullanıcının soyadının normalize edilmiş hali

        public string? Email { get; set; } // Kullanıcının e-posta adresi

        public string? NormalizedEmail { get; set; } // Kullanıcının e-posta adresinin normalize edilmiş hali

        public string? PasswordHash { get; set; } // Kullanıcının şifre hash değeri

        public string? PublicKey { get; set; }
        public string? EncryptedPrivateKey { get; set; }

        public bool IsActive { get; set; } = true; // Kullanıcının aktif olup olmadığı

        public DateTime CreatedAt { get; set; } // Kullanıcının hesap oluşturulma tarihi

        public DateTime? LastLogin { get; set; } // Kullanıcının en son giriş yaptığı tarih

        public int FailedLoginAttempts { get; set; } // Başarısız giriş denemeleri sayısı

        public DateTime? LockoutEnd { get; set; } // Hesap kilitlenme süresinin bitiş tarihi

        public string Timezone { get; set; } = "Europe/Istanbul"; // Kullanıcının zaman dilimi

        public string Culture { get; set; } = "tr-TR"; // Kullanıcının tercih ettiği kültür bilgisi

        public bool IsLdapUser { get; set; } // Kullanıcının LDAP ile giriş yapıp yapmadığı

        public DateTime? TermsAcceptedAt { get; set; } // Kullanıcının kullanım politikasını kabul ettiği tarih

        public DateTime? PrivacyPolicyAcceptedAt { get; set; } // Kullanıcının gizlilik politikasını kabul ettiği tarih

        // İki Faktörlü Kimlik Doğrulama Alanları
        public bool IsTwoFactorEnabled { get; set; } = false; // İki faktörlü kimlik doğrulama etkin mi?

        public string? TwoFactorVerificationCode { get; set; } // İki faktörlü doğrulama kodu (SMS/Email)

        public DateTime? TwoFactorCodeExpiresAt { get; set; } // İki faktörlü doğrulama kodunun geçerlilik süresi

        public string? PreferredTwoFactorMethod { get; set; } // Kullanıcının tercih ettiği iki faktörlü doğrulama yöntemi (Email, SMS, AuthenticatorApp)
        public int TokenVersion { get; set; } = 1; // Token versiyonu, token yenileme işlemlerinde kullanılacak
        // İlişkili Tablolar
        public List<RefreshTokens> RefreshTokens { get; set; } = new List<RefreshTokens>(); // Kullanıcıya ait refresh token listesi
        public List<UserRoles> UserRoles { get; set; } = new List<UserRoles>(); // Kullanıcının rollerinin listesi
    }
}
