using AuthenticationApi.Domain.Models;
using AuthenticationApi.Infrastructure.Repositories;
using System.Security.Authentication;

namespace AuthenticationApi.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthenticationService(
            IUserRepository userRepository,
            PasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

   

        public async Task<AuthenticationResult> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new AuthenticationException("Invalid credentials");

            if (user.LockoutEnd > DateTime.UtcNow)
                throw new AuthenticationException($"Account locked until {user.LockoutEnd}");

            return await GenerateAuthenticationResult(user);
        }


        public async Task<AuthenticationResult> RegisterAsync(RegisterRequest request)
        {
            // Kullanıcının veritabanında olup olmadığını kontrol et
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new AuthenticationException("Email is already registered");

            // Şifreyi hashleyerek yeni kullanıcı oluştur
            var hashedPassword = _passwordHasher.HashPassword(request.Password);
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                Timezone = request.Timezone,
                Culture = request.Culture,
                CreatedAt = DateTime.UtcNow,
                RefreshTokens = new List<RefreshToken>()
            };

            // Veritabanına kaydet
            await _userRepository.AddAsync(newUser);

            return await GenerateAuthenticationResult(newUser);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(token);

            if (user == null)
                throw new AuthenticationException("Invalid refresh token");

            var existingToken = user.RefreshTokens.FirstOrDefault(t => t.Token == token);
            if (existingToken == null || existingToken.ExpiryDate < DateTime.UtcNow)
                throw new AuthenticationException("Refresh token is expired or invalid");

            // Eski token'ı iptal et ve yeni bir token üret
            user.RefreshTokens.Remove(existingToken);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);

            await _userRepository.UpdateAsync(user);

            return new AuthenticationResult
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Timezone = user.Timezone,
                    Culture = user.Culture
                }
            };
        }

        public async Task RevokeTokenAsync(string token)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(token);

            if (user == null)
                throw new AuthenticationException("Invalid refresh token");

            var tokenToRevoke = user.RefreshTokens.FirstOrDefault(t => t.Token == token);
            if (tokenToRevoke != null)
            {
                user.RefreshTokens.Remove(tokenToRevoke);
                await _userRepository.UpdateAsync(user);
            }
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResult(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userRepository.UpdateAsync(user);

            return new AuthenticationResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Timezone = user.Timezone,
                    Culture = user.Culture
                }
            };
        }
    }
}
