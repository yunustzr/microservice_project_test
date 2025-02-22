using AuthenticationApi.Domain.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Username { get; set; }
    public string? NormalizedUsername { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public string Timezone { get; set; } = "Europe/Istanbul";
    public string Culture { get; set; } = "tr-TR";

    // RefreshToken ilişkisi
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}