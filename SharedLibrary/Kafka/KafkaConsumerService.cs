using System.Text.Json;
using Confluent.Kafka;

public interface IKafkaConsumerService<T>
{
    void Consume(string topic, Action<T> handleMessage, CancellationToken cancellationToken);
}

public class KafkaConsumerService<T> : IKafkaConsumerService<T>
{
    private readonly IConsumer<Null, string> _consumer;

    public KafkaConsumerService(string bootstrapServers, string groupId)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
    }

    public void Consume(string topic, Action<T> handleMessage, CancellationToken cancellationToken)
    {
        _consumer.Subscribe(topic);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(cancellationToken);
                if (result != null)
                {
                    var message = JsonSerializer.Deserialize<T>(result.Message.Value);
                    handleMessage(message);
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _consumer.Close();
        }
    }
}
