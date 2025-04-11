using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required, StringLength(255)]
        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public List<UserGroup> UserGroups { get; set; } = new();
        public List<GroupRole> GroupRoles { get; set; } = new();
    }
}
