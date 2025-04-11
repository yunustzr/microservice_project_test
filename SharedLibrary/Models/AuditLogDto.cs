using Nest;

namespace SharedLibrary.Models;
public class AuditLogDto
{

        [Keyword]
        public string ServiceName { get; set; } = string.Empty;
        [Keyword]
        public string Action { get; set; } = string.Empty;

        [Keyword]
        public string EntityName { get; set; } = string.Empty;

        [Keyword]
        public string EntityId { get; set; } = string.Empty;

        [Keyword]
        public string ChangedBy { get; set; } = string.Empty;

        [Date]
        public DateTime ChangedAt { get; set; }

        [Text]
        public string OldValues { get; set; } = string.Empty;

        [Text]
        public string NewValues { get; set; } = string.Empty;

        [Nested]
        public Dictionary<string, object> OldValuesParsed { get; set; } = new Dictionary<string, object>();

        [Nested]
        public Dictionary<string, object> NewValuesParsed { get; set; } = new Dictionary<string, object>();
}