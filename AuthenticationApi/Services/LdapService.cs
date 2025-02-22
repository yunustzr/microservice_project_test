using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using Microsoft.Extensions.Logging;
using AuthenticationApi.Configurations;

namespace AuthenticationApi.Services
{
    public interface ILdapService
    {
        bool Authenticate(string username, string password);
    }

    public class LdapService : ILdapService
    {
        private readonly LdapConfig _config;
        private readonly ILogger<LdapService> _logger;

        public LdapService(IOptions<LdapConfig> config, ILogger<LdapService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public bool Authenticate(string username, string password)
        {
            try
            {
                using var connection = new LdapConnection();
                connection.Connect(_config.Server, _config.Port);
                connection.Bind($"uid={username},{_config.BaseDn}", password);

                return connection.Bound;
            }
            catch (LdapException ex)
            {
                _logger.LogError($"LDAP Authentication failed for user {username}. Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during LDAP authentication. Error: {ex.Message}");
                return false;
            }
        }
    }
}
