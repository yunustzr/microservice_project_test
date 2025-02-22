using AuthenticationApi.Domain.Models;

namespace AuthenticationApi.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string username, string password);
    }
}
