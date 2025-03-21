namespace SharedLibrary.Kafka;

public class KafkaConfig
{
    public string BootstrapServers { get; set; }
    public List<string> Topics { get; set; } = new List<string>(); // Birden fazla topic desteği
    public string ConsumerGroupId { get; set; }
}
