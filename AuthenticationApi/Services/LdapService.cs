using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using AuthenticationApi.Configurations;

namespace AuthenticationApi.Services
{
    public class LdapService : ILdapService
    {
        private readonly LdapConfig _config;
        public LdapService(IOptions<LdapConfig> config)
        {
            _config = config.Value;
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
            catch
            {
                return false;
            }
        }
    }
}
