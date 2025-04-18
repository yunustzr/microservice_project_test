namespace AuthenticationApi.Domain.Models.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? NormalizedLastName { get; set; }
        public string? Timezone { get; set; }
        public string? Culture { get; set; }
        public string? Email { get; internal set; }
        public bool IsLdapUser { get; internal set; }
        public int TokenVersion { get; internal set; }
        public bool IsActive { get; internal set; }
        public int FailedLoginAttempts { get; internal set; }
        public DateTime? LockoutEnd { get; internal set; }
    }
}
