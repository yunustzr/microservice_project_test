namespace SharedLibrary.Models;

public class AuditLogDto
{
        public string Action { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
}
