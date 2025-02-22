using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using AuthenticationApi.Configurations;
using AuthenticationApi.Infrastructure.Repositories;
using System.Security.Authentication;

namespace AuthenticationApi.Services
{
    public class LdapService : ILdapService
    {
        private readonly LdapConfig _config;
        private readonly IUserRepository _userRepository;

        public LdapService(
            IOptions<LdapConfig> config,
            IUserRepository userRepository)
        {
            _config = config.Value;
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            using var connection = new LdapConnection
            {
                SecureSocketLayer = _config.Port == 636
            };

            try
            {
                connection.Connect(_config.Server, _config.Port);
                connection.Bind(username, password);

                var user = await _userRepository.GetByUsernameAsync(username);
                return user ?? throw new AuthenticationException("LDAP user not synced");
            }
            catch (LdapException ex)
            {
                throw new AuthenticationException("LDAP authentication failed", ex);
            }
        }

        public async Task SyncLdapUsersAsync()
        {
            // LDAP'tan kullanıcıları çekip veritabanına senkronize et
            // Detaylı implementasyon LDAP yapısına göre değişir
        }
    }
}
