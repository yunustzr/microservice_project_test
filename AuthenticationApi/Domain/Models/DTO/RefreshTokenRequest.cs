using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.DTO;

public class RefreshTokenRequest
{
   [Required]
    public string RefreshToken { get; set; }
}
