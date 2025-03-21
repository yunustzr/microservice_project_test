using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


public interface IKafkaProducerService
{
    Task ProduceAsync<T>(string topic, T message);
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
