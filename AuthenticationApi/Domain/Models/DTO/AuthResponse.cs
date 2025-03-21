namespace AuthenticationApi.Domain.Models.DTO;

public class AuthResponse
{
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
}
