using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using System.Security.Authentication;
using AuthenticationApi.Domain.Models.ENTITY;
using AuthenticationApi.Domain.Models.DTO;
using AuthenticationApi.Domain.Exceptions;

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

        public LdapService(
            ILdapConfigService configService,
            IUserRepository userRepository,
            PasswordHasher passwordHasher,
            ILogger<LdapService> logger)
        {
            _configService = configService;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<User> AuthenticateAsync(LoginRequest request)
        {
            var ldapConfigs = await _configService.GetLdapConfigurationsAsync();

            foreach (var config in ldapConfigs)
            {
                try
                {
                    var user = await AuthenticateViaLdap(request, config);
                    if (user != null)
                    {
                        return user;
                    }
                }
                catch (LdapException ex) when (IsLdapConnectionError(ex))
                {
                    _logger.LogWarning($"[LDAP] Connection error on server '{config.Server}': {ex.Message}");
                    continue; // diğer konfigürasyona geç
                }
                catch (AuthenticationException ex)
                {
                    _logger.LogWarning($"[LDAP] Authentication failed for server '{config.Server}': {ex.Message}");
                    // kullanıcı yoksa diğerini dene ama tek bir tane LDAP üzerinden kullanıcı varsa, local'e geçme
                    return null!;
                }
            }

            // LDAP ile giriş yapılamadıysa, local kullanıcı ile dene
            return await AuthenticateLocalUser(request);
        }

        private async Task<User> AuthenticateViaLdap(LoginRequest request, LdapConfiguration config)
        {
            using var connection = new LdapConnection
            {
                SecureSocketLayer = config.UseSSL
            };

            connection.Connect(config.Server, config.Port);
            connection.Bind(request.Email, request.Password);

            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new AuthenticationException("User not found");
            }

            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            user.IsLdapUser = true;
            await _userRepository.UpdateAsync(user);

            return user;
        }

        private async Task<User> AuthenticateLocalUser(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new InvalidCredentialsException("Invalid credentials", 0, 10);
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                throw new InvalidCredentialsException(
                    "User account is locked.",
                    user.FailedLoginAttempts,
                    10,
                    user.LockoutEnd);
            }

            if (string.IsNullOrEmpty(user.PasswordHash) || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 10)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                }

                await _userRepository.UpdateAsync(user);

                throw new InvalidCredentialsException(
                    "Invalid credentials",
                    user.FailedLoginAttempts,
                    10,
                    user.LockoutEnd);
            }

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.IsLdapUser = false;

            await _userRepository.UpdateAsync(user);
            return user;
        }

        private bool IsLdapConnectionError(Exception ex)
        {
            return ex is LdapException ldapEx &&
                   (ldapEx.ResultCode == LdapException.ConnectError ||
                    ldapEx.ResultCode == LdapException.ServerDown);
        }

        public Task SyncLdapUsersAsync()
        {
            // İsteğe bağlı olarak LDAP kullanıcılarını senkronize etme işlemi burada yapılabilir.
            return Task.CompletedTask;
        }
    }
}
