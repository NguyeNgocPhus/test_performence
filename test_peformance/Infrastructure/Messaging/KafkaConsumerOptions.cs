namespace test_peformance.Infrastructure.Messaging;

public sealed class KafkaConsumerOptions
{
    public bool Enabled { get; set; } = true;
    public string BootstrapServers { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string GroupId { get; set; } = "test_peformance-consumer";
    public string AutoOffsetReset { get; set; } = "Earliest";
}
