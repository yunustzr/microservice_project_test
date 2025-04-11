using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        // Eğer bu alan her zaman dolu olacaksa, boş string ile başlatabilirsiniz.
        [Required]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        public string Action { get; set; } = string.Empty; // "Create", "Update", "Delete"

        [Required]
        public string EntityName { get; set; } = string.Empty; // "User", "Role"

        [Required]
        public string EntityId { get; set; } = string.Empty; // Örneğin "123"

        [Required]
        public string ChangedBy { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; }

        // Bu alanların veritabanında NULL olabilme ihtimali varsa nullable yapıyoruz.
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? DiffValues { get; set; }
    }
}
