using StackExchange.Redis;
using test_peformance.Application.Abstractions;

namespace test_peformance.Infrastructure.Cache;

public class RedisUnreadStore : IRedisUnreadStore
{
    private readonly IConnectionMultiplexer _multiplexer;

    public RedisUnreadStore(IConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
    }

    public IDatabase GetDatabase() => _multiplexer.GetDatabase();
}
