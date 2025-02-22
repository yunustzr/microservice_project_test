using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models
{
    public class RegisterRequest : LoginRequest
    {
        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Timezone { get; set; } = string.Empty;
        public string Culture { get; set; } = "tr-TR";
    }
}
