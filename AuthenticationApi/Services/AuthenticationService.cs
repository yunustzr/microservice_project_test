using AuthenticationApi.Domain.Models;

namespace AuthenticationApi.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly ILdapService _ldapService;
        public AuthenticationService(IUserService userService, ILdapService ldapService)
        {
            _userService = userService;
            _ldapService = ldapService;
        }
        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
        {
            var user = await _userService.GetByUsernameAsync(username);
            if (user != null)
            {
                if (await _userService.ValidateLocalCredentialsAsync(username, password))
                    return AuthenticationResult.Success(user);
                else
                    return AuthenticationResult.Failure("Invalid local credentials");
            }
            else
            {
                if (_ldapService.Authenticate(username, password))
                {
                    // LDAP doğrulaması başarılı; isteğe bağlı olarak LDAP kullanıcısını local'e kaydedebilirsiniz.
                    var ldapUser = new User { Username = username, Role = "LDAP" };
                    return AuthenticationResult.Success(ldapUser);
                }
                else
                    return AuthenticationResult.Failure("Authentication failed");
            }
        }
    }
}
