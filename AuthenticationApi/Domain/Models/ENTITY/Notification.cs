using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? UserId { get; set; }  // null ise tüm kullanıcılara bildirim
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsSent { get; set; } = false;        // Gönderildi mi?
        public bool ShouldDisplay { get; set; } = true; // Oluşturulduğunda gösterim durumu
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DisplayAt { get; set; }        // Gösterim zamanı, null ise hemen
        public DateTime? SentAt { get; set; }           // Gönderim zamanı

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }

}
