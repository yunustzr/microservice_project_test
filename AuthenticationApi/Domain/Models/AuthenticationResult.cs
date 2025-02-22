namespace AuthenticationApi.Domain.Models
{
    public class AuthenticationResult
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserDto? User { get; set; }
     
    }
}
