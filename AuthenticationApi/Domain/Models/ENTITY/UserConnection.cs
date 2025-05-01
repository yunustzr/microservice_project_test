using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationApi.Domain.Models.ENTITY
{
    public class UserConnection
    {
        [Key]
        public string ConnectionId { get; set; }
        public DateTime ConnectedAt { get; set; }
        public DateTime? DisconnectedAt { get; set; }

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
