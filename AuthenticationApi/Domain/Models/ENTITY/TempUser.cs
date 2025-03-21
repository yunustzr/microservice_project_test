using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class TempUser
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? UserName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    // Email doğrulama için token veya kod
    public string? VerificationCode { get; set; }
    public bool AcceptTerms { get; set; }
    public bool AcceptPrivacyPolicy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Doğrulama süresi (örneğin 30 dakika)
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
    public bool IsDeleted { get; set; } = false;

}
