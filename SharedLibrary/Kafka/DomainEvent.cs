namespace SharedLibrary.Kafka;

public class DomainEvent
{
    public string EventType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string SourceService { get; set; } // Örneğin "auth_api", "product_api", "customer_api"
    public string Payload { get; set; } // JSON olarak event detayları
}
