using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.DTO;

public class RevokeTokenRequest
{
    [Required]
    public string RefreshToken { get; set; }
}
