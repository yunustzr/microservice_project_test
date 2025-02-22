using AuthenticationApi.Domain.Models;

namespace AuthenticationApi.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync(LoginRequest request);
        Task<AuthenticationResult> RegisterAsync(RegisterRequest request);
        Task<AuthenticationResult> RefreshTokenAsync(string token);
        Task RevokeTokenAsync(string token);
    }
}
