using AuthenticationApi.Domain.Models;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User> AddUserAsync(User user);
    }
}
