using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using System.Text.Json;
using SharedLibrary.Models;
using System.Text.Json.Nodes;

namespace SharedLibrary.Kafka
{
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
                EnableAutoCommit = false // Manuel commit yapacağız
            };

            _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
            _consumer.Subscribe(_topics);
        }

        private async Task EnsureIndexExistsAsync(CancellationToken stoppingToken)
        {
            try
            {
                var existsResponse = await _elasticClient.Indices.ExistsAsync("audit_logs", ct: stoppingToken);

                if (!existsResponse.Exists)
                {
                    var createResponse = await _elasticClient.Indices.CreateAsync("audit_logs", c => c
                        .Map<AuditLogDto>(m => m
                            .AutoMap()
                            .Properties(p => p
                                .Keyword(t => t.Name(n => n.Action))
                                .Keyword(t => t.Name(n => n.EntityName))
                                .Keyword(t => t.Name(n => n.EntityId))
                                .Keyword(t => t.Name(n => n.ChangedBy))
                                .Date(t => t.Name(n => n.ChangedAt))
                                .Nested<Dictionary<string, object>>(t => t
                                    .Name(n => n.OldValuesParsed)
                                    .Dynamic())
                                .Nested<Dictionary<string, object>>(t => t
                                    .Name(n => n.NewValuesParsed)
                                    .Dynamic())
                            )
                        ),
                        stoppingToken);

                    if (!createResponse.IsValid)
                    {
                        _logger.LogError("Index oluşturma hatası: {DebugInfo}", createResponse.DebugInformation);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Index kontrol hatası");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await EnsureIndexExistsAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Null, string> consumeResult = null;
                try
                {
                    consumeResult = _consumer.Consume(stoppingToken);
                    if (consumeResult != null)
                    {
                        var message = consumeResult.Message.Value;
                        _logger.LogDebug("Mesaj alındı: {Message}", message);

                        try
                        {
                            var jsonNode = JsonNode.Parse(message);
                            if (jsonNode == null)
                            {
                                _logger.LogWarning("Geçersiz JSON formatı");
                                _consumer.Commit(consumeResult);
                                continue;
                            }

                            var auditLogDto = new AuditLogDto
                            {
                                ServiceName = jsonNode["ServiceName"]?.GetValue<string>() ?? string.Empty,
                                Action = jsonNode["Action"]?.GetValue<string>() ?? string.Empty,
                                EntityName = jsonNode["EntityName"]?.GetValue<string>() ?? string.Empty,
                                EntityId = jsonNode["EntityId"]?.GetValue<string>() ?? string.Empty,
                                ChangedBy = jsonNode["ChangedBy"]?.GetValue<string>() ?? string.Empty,
                                ChangedAt = jsonNode["ChangedAt"]?.GetValue<DateTime>() ?? DateTime.MinValue,
                                OldValuesParsed = ParseJsonNode(jsonNode["OldValues"]),
                                NewValuesParsed = ParseJsonNode(jsonNode["NewValues"])
                            };

                            var response = await _elasticClient.IndexDocumentAsync(auditLogDto, stoppingToken);

                            if (response.IsValid)
                            {
                                _logger.LogInformation("Mesaj başarıyla indexlendi. ID: {Id}", response.Id);
                                _consumer.Commit(consumeResult);
                            }
                            else
                            {
                                _logger.LogError("Indexleme hatası: {DebugInfo}", response.DebugInformation);
                            }
                        }
                        catch (JsonException jsonEx)
                        {
                            _logger.LogError(jsonEx, "JSON deserialization hatası");
                            _consumer.Commit(consumeResult);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Mesaj işleme hatası");
                        }
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Kafka tüketim hatası");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("İşlem iptal edildi");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Beklenmeyen hata");
                    if (consumeResult != null)
                    {
                        _consumer.Commit(consumeResult);
                    }
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }


        private Dictionary<string, object> ParseJsonNode(JsonNode node)
        {
            // Eğer node null ise boş dictionary döndür
            if (node == null)
                return new Dictionary<string, object>();

            // Eğer node bir string ise, öncelikle string değerini alalım.
            if (node is JsonValue jsonValue)
            {
                // Deneyelim, string değeri geçerli bir JSON nesnesine parse edebiliyor muyuz?
                string? strValue = jsonValue.GetValue<string>();
                try
                {
                    // Eğer strValue geçerli bir JSON nesnesi olarak parse edilebiliyorsa
                    var parsedNode = JsonNode.Parse(strValue);
                    if (parsedNode is JsonObject)
                    {
                        // Yeniden nesne olarak işle
                        return ParseJsonNode(parsedNode);
                    }
                    else
                    {
                        // Eğer parse edilince nesne değilse, tek değer olarak dönebiliriz.
                        return new Dictionary<string, object> { { "value", strValue } };
                    }
                }
                catch (JsonException)
                {
                    // Eğer parse edilemezse, string değeri tek değer olarak döndür
                    return new Dictionary<string, object> { { "value", strValue } };
                }
            }

            // Eğer node zaten bir nesne (JsonObject) ise, property'leri iterate edelim.
            if (node is JsonObject jsonObject)
            {
                var dict = new Dictionary<string, object>();
                foreach (var property in jsonObject)
                {
                    // Rekürsif olarak parse edelim. Herhangi bir alt node için de aynı mantık çalışır.
                    dict[property.Key] = property.Value is JsonObject || property.Value is JsonArray
                        ? (object)ParseJsonNode(property.Value)
                        : property.Value?.ToString() ?? string.Empty;
                }
                return dict;
            }

            // Eğer node başka bir türdeyse, string olarak döndür.
            return new Dictionary<string, object> { { "value", node.ToString() } };
        }


        private List<object> ParseJsonArray(JsonElement element)
        {
            var list = new List<object>();
            foreach (var item in element.EnumerateArray())
            {
                list.Add(item.ValueKind switch
                {
                    JsonValueKind.String => item.GetString(),
                    JsonValueKind.Number => item.TryGetInt32(out int intVal) ? intVal :
                                           item.TryGetInt64(out long longVal) ? longVal :
                                           item.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    JsonValueKind.Object => ParseJsonNode(JsonNode.Parse(item.GetRawText())),
                    JsonValueKind.Array => ParseJsonArray(item),
                    _ => item.ToString()
                });
            }
            return list;
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}