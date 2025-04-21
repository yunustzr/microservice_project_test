namespace AuthenticationApi.Domain.Models.ENTITY
{
    public class LdapConfiguration
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string BaseDn { get; set; }
        public string AdminUser { get; set; }
        public string AdminPassword { get; set; }
        public bool Enabled { get; set; }
        public int Priority { get; set; }
    }
}
