namespace AuthenticationApi.Domain.Models.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Timezone { get; set; }
        public string? Culture { get; set; }
        public string? Email { get; internal set; }
        public bool IsLdapUser { get; internal set; }
    }
}
