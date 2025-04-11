using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY
{
    public class UserGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "char(36)")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; } = null!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        public string? AddedBy { get; set; }
    }
}
