namespace AuthenticationApi.Domain.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? Timezone { get; set; }
        public string? Culture { get; set; }
    }
}
