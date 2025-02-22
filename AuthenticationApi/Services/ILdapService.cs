namespace AuthenticationApi.Services
{
    public interface ILdapService
    {
        bool Authenticate(string username, string password);
    }
}
