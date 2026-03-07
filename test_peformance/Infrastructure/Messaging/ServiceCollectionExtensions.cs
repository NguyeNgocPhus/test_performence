namespace test_peformance.Infrastructure.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaConsumerOptions>(configuration.GetSection("Kafka"));
        services.AddHostedService<KafkaConsumerBackgroundService>();
        return services;
    }
}
