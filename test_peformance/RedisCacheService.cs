using System.Text.Json;
using StackExchange.Redis;

namespace test_peformance;

public class RedisCacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(string redisConnectionString)
    {
        var redis = ConnectionMultiplexer.Connect(redisConnectionString);
        _db = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        string jsonData = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, jsonData, expiry);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var jsonData = await _db.StringGetAsync(key);
        return jsonData.HasValue ? JsonSerializer.Deserialize<T>(jsonData!) : default;
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}