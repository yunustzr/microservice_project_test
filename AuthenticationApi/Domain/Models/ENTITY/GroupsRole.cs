using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY
{
    public class GroupRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; } = null!;

        [Required]
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [Required, StringLength(255)]
        public string AssignedBy { get; set; }
    }
}
