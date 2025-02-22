namespace AuthenticationApi.Services
{
    public interface ILdapService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task SyncLdapUsersAsync();
    }
}
