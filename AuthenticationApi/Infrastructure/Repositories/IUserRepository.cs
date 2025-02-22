using AuthenticationApi.Domain.Models;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<User> AddUserAsync(User user);
        Task<bool> ExistsAsync(string email);
        Task<User> GetUserByRefreshTokenAsync(string refreshToken);
    }
}
