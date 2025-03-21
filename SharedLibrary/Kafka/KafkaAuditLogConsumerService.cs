using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using System.Text.Json;


namespace SharedLibrary.Kafka;

public class KafkaAuditLogConsumerService : BackgroundService
{
    private readonly ILogger<KafkaAuditLogConsumerService> _logger;
    private readonly IConsumer<Null, string> _consumer;
    private readonly IElasticClient _elasticClient;
    private readonly List<string> _topics;

    public KafkaAuditLogConsumerService(
        ILogger<KafkaAuditLogConsumerService> logger,
        IElasticClient elasticClient,
        IConfiguration configuration)
    {
        _logger = logger;
        _elasticClient = elasticClient;

        var bootstrapServers = configuration["KafkaConfig:BootstrapServers"] ?? "localhost:9092";
        var consumerGroupId = configuration["KafkaConfig:ConsumerGroupId"] ?? $"consumer-{Guid.NewGuid()}";

        _topics = configuration.GetSection("KafkaConfig:Topics").Get<List<string>>() ?? new List<string> { "default_topic" };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = consumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
        _consumer.Subscribe(_topics);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    if (consumeResult != null)
                    {
                        var message = consumeResult.Message.Value;
                        var topic = consumeResult.Topic; // Hangi topic'ten geldiğini öğreniyoruz

                        _logger.LogInformation("Topic: {Topic} - Mesaj alındı: {Message}", topic, message);

                        var auditLog = JsonSerializer.Deserialize<object>(message);
                        if (auditLog != null)
                        {
                            var response = _elasticClient.IndexDocument(auditLog);
                            if (!response.IsValid)
                            {
                                _logger.LogError("Elasticsearch indexleme hatası: {Error}", response.OriginalException?.Message);
                            }
                        }
                        _consumer.Commit(consumeResult);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kafka tüketiminde hata oluştu");
                }
            }
        }, stoppingToken);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}