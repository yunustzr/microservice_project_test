using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using AuthenticationApi.Configurations;
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
        private readonly LdapConfig _config;
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;

        public LdapService(
            IOptions<LdapConfig> config,
            IUserRepository userRepository,
            PasswordHasher passwordHasher)
        {
            _config = config.Value;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> AuthenticateAsync(LoginRequest request)
        {
            try
            {
                // LDAP ile kimlik doğrulama denemesi
                return await AuthenticateViaLdap(request);
            }
            catch (Exception ex) when (IsLdapConnectionError(ex))
            {
                // LDAP bağlantı hatası durumunda yerel kullanıcıyı dene
                return await AuthenticateLocalUser(request);
            }
        }
        private async Task<User> AuthenticateViaLdap(LoginRequest request)
        {
            using var connection = new LdapConnection
            {
                SecureSocketLayer = _config.Port == 636
            };

            connection.Connect(_config.Server, _config.Port);
            connection.Bind(request.Email, request.Password);

            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new AuthenticationException("User not found");
            }

            // Şifreyi güncelle (LDAP şifresiyle sync)
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            await _userRepository.UpdateAsync(user);

            return user;
        }


        private async Task<User> AuthenticateLocalUser(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new InvalidCredentialsException(
                    "Invalid credentials",
                    failedLoginAttempts: 0,
                    maxAttempts: 10
                );
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                throw new InvalidCredentialsException(
                    "User account is locked. Try again later.",
                    failedLoginAttempts: user.FailedLoginAttempts,
                    maxAttempts: 10,
                    lockoutEnd: user.LockoutEnd);
            }

            // Şifre yanlışsa
            if (string.IsNullOrEmpty(user.PasswordHash) ||
                !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 10)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                }

                await _userRepository.UpdateAsync(user);

                throw new InvalidCredentialsException(
                    "Invalid credentials",
                    failedLoginAttempts: user.FailedLoginAttempts,
                    maxAttempts: 10,
                    lockoutEnd: user.LockoutEnd);
            }

            // Giriş başarılıysa: sayaçları sıfırla
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            if (user.IsLdapUser)
            {
                user.IsLdapUser = false;
            }

            await _userRepository.UpdateAsync(user);
            return user;
        }


        private bool IsLdapConnectionError(Exception ex)
        {
            // Bağlantı hatalarını tespit et
            return ex is LdapException ldapEx &&
                (ldapEx.ResultCode == LdapException.ConnectError ||
                 ldapEx.ResultCode == LdapException.ServerDown);
        }

        public async Task SyncLdapUsersAsync()
        {
            // LDAP'tan kullanıcıları çekip veritabanına senkronize et
            // Detaylı implementasyon LDAP yapısına göre değişir
        }
    }
}
