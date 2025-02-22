using AuthenticationApi.Domain.Models;

namespace AuthenticationApi.Services
{
    public interface IUserService
    {
        Task<bool> ValidateLocalCredentialsAsync(string username, string password);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> RegisterLocalUserAsync(RegisterRequest request);
    }
}
