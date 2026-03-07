using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace test_peformance.Infrastructure.Messaging;

public sealed class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly KafkaConsumerOptions _options;

    public KafkaConsumerBackgroundService(
        ILogger<KafkaConsumerBackgroundService> logger,
        IOptions<KafkaConsumerOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Kafka consumer is disabled.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.BootstrapServers) || string.IsNullOrWhiteSpace(_options.Topic))
        {
            _logger.LogWarning("Kafka consumer configuration is invalid. BootstrapServers/Topic is missing.");
            return;
        }

        if (!Enum.TryParse(_options.AutoOffsetReset, true, out AutoOffsetReset autoOffsetReset))
        {
            autoOffsetReset = AutoOffsetReset.Earliest;
        }

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = autoOffsetReset
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(_options.Topic);
        _logger.LogInformation("Kafka consumer started. Topic: {Topic}, GroupId: {GroupId}", _options.Topic, _options.GroupId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumed = consumer.Consume(stoppingToken);

                if (consumed?.Message is null)
                    continue;

                _logger.LogInformation(
                    "Kafka message received. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Value: {Value}",
                    consumed.Topic,
                    consumed.Partition.Value,
                    consumed.Offset.Value,
                    consumed.Message.Value);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while consuming Kafka messages.");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        consumer.Close();
        _logger.LogInformation("Kafka consumer stopped.");
    }
}
