using AuthenticationApi.Domain.Models;
using AuthenticationApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<bool> ValidateLocalCredentialsAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null)
                return false;
            return PasswordHasher.Verify(password, user.PasswordHash);
        }

        public async Task<User> RegisterLocalUserAsync(RegisterRequest request)
        {
            var hashedPassword = PasswordHasher.Hash(request.Password);
            var user = new User
            {
                Username = request.Username,
                PasswordHash = hashedPassword,
                Role = request.Role
            };
            return await _userRepository.AddUserAsync(user);
        }
    }
}
