using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


public interface IKafkaProducerService
{
    Task ProduceAsync<T>(string topic, T message);
    Task ProduceBulkAsync<T>(string topic, IEnumerable<T> messages);
}
public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService(IConfiguration configuration)
    {

        var _config = new ProducerConfig
        {
            BootstrapServers = configuration["KafkaConfig:BootstrapServers"]
        };
        _producer = new ProducerBuilder<Null, string>(_config).Build();
    }
    public async Task ProduceBulkAsync<T>(string topic, IEnumerable<T> messages)
    {
        var tasks = new List<Task<DeliveryResult<Null, string>>>();

        foreach (var message in messages)
        {
            var json = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<Null, string> { Value = json };
            tasks.Add(_producer.ProduceAsync(topic, kafkaMessage));
        }

        // Tüm görevlerin tamamlanmasını bekliyoruz
        var deliveryResults = await Task.WhenAll(tasks);

        // İsteğe bağlı: Sonuçları kontrol edebiliriz
        foreach (var result in deliveryResults)
        {
            if (result.Status != PersistenceStatus.Persisted)
            {
                throw new Exception($"Message delivery failed: {result.Status} - {result.Message}");
            }
        }
    }
    public async Task ProduceAsync<T>(string topic, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var kafkaMessage = new Message<Null, string> { Value = json };
        await _producer.ProduceAsync(topic, kafkaMessage);
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
