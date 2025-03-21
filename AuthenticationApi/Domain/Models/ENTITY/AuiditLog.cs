using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class AuditLog
{
    [Key]
    public int Id { get; set; }
    public string Action { get; set; } // "Create", "Update", "Delete"
    public string EntityName { get; set; } // "User", "Role"
    public string EntityId { get; set; } // "123"
    public string ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
}
