using AuthenticationApi.Domain.Models;
using AuthenticationApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher();
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
            return _passwordHasher.VerifyPassword(password, user.PasswordHash);
        }

        public async Task<User> RegisterLocalUserAsync(RegisterRequest request)
        {
            var hashedPassword = _passwordHasher.HashPassword(request.Password);
            var user = new User
            {
                Email = request.Email,
                PasswordHash = hashedPassword
            };
            return await _userRepository.AddUserAsync(user);
        }
    }
}
