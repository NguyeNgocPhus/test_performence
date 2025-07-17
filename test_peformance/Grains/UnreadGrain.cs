using System.Collections.Concurrent;
using Orleans.Runtime;
using StackExchange.Redis;
using test_peformance.Abstractions;
using test_peformance.Entities;

namespace test_peformance.Grains;

public class UnreadGrain : Grain<UnreadState>, IUnreadGrain
{
    private List<UserBranchAssign> listUserBranch = new()
    {
        new UserBranchAssign(){BranchId = "001", UserId = "phunn"},
        new UserBranchAssign(){BranchId = "002", UserId = "haicn"},
        new UserBranchAssign(){BranchId = "001", UserId = "vancn"}

    };
    private List<UserBrandAssign> listUserBrand = new()
    {
        new UserBrandAssign(){BrandName = "KV01", UserId = "phuho"},
        new UserBrandAssign(){BrandName = "KV01", UserId = "ngocho"},
        new UserBrandAssign(){BrandName = "KV02", UserId = "vanho"}
    };

    private IDisposable timer;
    public UnreadGrain()
    {
        Console.WriteLine("Creating DashboardGrain");
        timer = RegisterTimer(UpdateUnread, null!, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
    }
    

    public Task UpdateUnreadConversation(UpdateUnreadConversation message)
    {
        State.ConcurrentQueue.Enqueue(message);
        
        return Task.CompletedTask;
    }

    private async Task UpdateUnread(object? state)
    {
        var redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
        IDatabase db = redis.GetDatabase();

        var count = State.ConcurrentQueue.Count;
        var listUnread = new List<UpdateUnreadConversation>();
        while (listUnread.Count < count && State.ConcurrentQueue.TryDequeue(out var item))
        {
            if (listUnread.Any(x => x.OrderId == item.OrderId && x.UserId == item.UserId))
                continue;
            listUnread.Add(item);
        }

        if (listUnread.Count == 0) return;
        var dicUpdate = new Dictionary<string, HashEntry[]>();
        foreach (var item in listUnread)
        {
            var listUBranch = listUserBranch.Where(x => x.BranchId == item.BranchId && x.UserId != item.UserId).ToList();
            var listUBrand = listUserBrand.Where(x => x.BrandName == item.BrandName  && x.UserId != item.UserId).ToList();
            foreach (var u in listUBranch)
            {
                AddFieldIfNotExists(dicUpdate, u.UserId, item.OrderId, "1");
            }

            foreach (var u in listUBrand)
            {
                AddFieldIfNotExists(dicUpdate, u.UserId, item.OrderId, "1");
            }
        }

        foreach (var data in dicUpdate)
        {
            await db.HashSetAsync(data.Key, data.Value);
        }

        await Task.CompletedTask;
    }

    private void AddFieldIfNotExists(Dictionary<string, HashEntry[]> dict, string key, string field, string value)
    {
        if (!dict.ContainsKey(key))
        {
            // Nếu chưa có key, thêm mới luôn
            dict[key] = new[] { new HashEntry(field, value) };
        }
        else
        {
            var existingEntries = dict[key];

            // Kiểm tra field đã tồn tại chưa
            bool exists = existingEntries.Any(e => e.Name == field);

            if (!exists)
            {
                // Thêm mới field nếu chưa có
                var updated = existingEntries.Append(new HashEntry(field, value)).ToArray();
                dict[key] = updated;
            }
        }
    }
}
public class UnreadState
{
    public ConcurrentQueue<UpdateUnreadConversation> ConcurrentQueue { get; set; } = new();
}
class UserBranchAssign
{
    public string UserId { get; set; }
    public string BranchId { get; set; }
}

class UserBrandAssign
{
    public string UserId { get; set; }
    public string BrandName { get; set; }
}