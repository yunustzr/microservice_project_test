using System;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Models.DTO;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Domain.Exceptions;
using AuthenticationApi.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;

namespace AuthenticationApi.Services
{
    public interface ILdapService
    {
        Task<User> AuthenticateAsync(LoginRequest request);
        Task SyncLdapUsersAsync();
    }

    public class LdapService : ILdapService
    {
        private readonly ILdapConfigService _configService;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly ILogger<LdapService> _logger;
        private readonly ISystemSettingService _settings;
        private readonly IRsaKeyService _rsaKeyService;

        public LdapService(
            ILdapConfigService configService,
            IUserRepository userRepository,
            PasswordHasher passwordHasher,
            ILogger<LdapService> logger,
            ISystemSettingService settings,
            IRsaKeyService rsaKeyService)
        {
            _configService = configService;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _settings = settings;
            _rsaKeyService = rsaKeyService;
        }

        public async Task<User> AuthenticateAsync(LoginRequest request)
        {
            // 1️⃣ Kullanıcıyı al ve private key'i edin
            var user = await _userRepository.GetByEmailAsync(request.Email)
                       ?? throw new AuthenticationException("Invalid credentials");

            // 2️⃣ Şifreyi RSA ile decrypt et
            var password = await _rsaKeyService.DecryptDataAsync(request.Password, user.EncryptedPrivateKey);

            // 3️⃣ LDAP ile doğrula
            var ldapConfigs = await _configService.GetLdapConfigurationsAsync();
            foreach (var config in ldapConfigs)
            {
                try
                {
                    var ldapUser = await AuthenticateViaLdapInternal(request.Email, password, config);
                    if (ldapUser != null)
                        return ldapUser;
                }
                catch (LdapException ex) when (IsLdapConnectionError(ex))
                {
                    _logger.LogWarning($"[LDAP] Connection error on server '{config.Server}': {ex.Message}");
                    continue;
                }
                catch (AuthenticationException ex)
                {
                    _logger.LogWarning($"[LDAP] Authentication failed for server '{config.Server}': {ex.Message}");
                    // LDAP'da kullanıcı bulunursa local'e geçme
                    return null!;
                }
            }

            // 4️⃣ Local kullanıcıyı doğrula
            return await AuthenticateLocalUser(request.Email, password);
        }

        private async Task<User> AuthenticateViaLdapInternal(string email, string password, LdapConfiguration config)
        {
            using var connection = new LdapConnection { SecureSocketLayer = config.UseSSL };
            connection.Connect(config.Server, config.Port);
            connection.Bind(email, password);

            var user = await _userRepository.GetByEmailAsync(email)
                       ?? throw new AuthenticationException("User not found");

            // Şifreyi bcrypt ile hash'le ve işaretle
            user.PasswordHash = _passwordHasher.HashPassword(password);
            user.IsLdapUser = true;
            await _userRepository.UpdateAsync(user);
            return user;
        }

        private async Task<User> AuthenticateLocalUser(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email)
                       ?? throw new InvalidCredentialsException("Invalid credentials", 0, await _settings.GetIntAsync("MaxLoginAttempts", 10));

            var maxAttempts = await _settings.GetIntAsync("MaxLoginAttempts", 10);

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                throw new InvalidCredentialsException(
                    "User account is locked.",
                    user.FailedLoginAttempts,
                    maxAttempts,
                    user.LockoutEnd);

            if (string.IsNullOrEmpty(user.PasswordHash) || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= maxAttempts)
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);

                await _userRepository.UpdateAsync(user);
                throw new InvalidCredentialsException(
                    "Invalid credentials",
                    user.FailedLoginAttempts,
                    maxAttempts,
                    user.LockoutEnd);
            }

            // Başarılı local login
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.IsLdapUser = false;
            await _userRepository.UpdateAsync(user);
            return user;
        }

        private bool IsLdapConnectionError(LdapException ex)
        {
            return ex.ResultCode == LdapException.ConnectError || ex.ResultCode == LdapException.ServerDown;
        }

        public Task SyncLdapUsersAsync() => Task.CompletedTask;
    }
}
