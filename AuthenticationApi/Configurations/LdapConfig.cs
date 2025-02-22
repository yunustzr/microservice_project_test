namespace AuthenticationApi.Configurations
{
    public class LdapConfig
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; }
        public string BaseDn { get; set; } = string.Empty;
    }

}
