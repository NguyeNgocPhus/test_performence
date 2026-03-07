using StackExchange.Redis;

namespace test_peformance.Application.Abstractions;

public interface IRedisUnreadStore
{
    IDatabase GetDatabase();
}
