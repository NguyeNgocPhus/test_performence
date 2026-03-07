using StackExchange.Redis;
using test_peformance.Application.Abstractions;

namespace test_peformance.Infrastructure.Cache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));
        services.AddSingleton<IRedisUnreadStore, RedisUnreadStore>();
        return services;
    }
}
