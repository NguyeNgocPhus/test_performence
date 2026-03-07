using Newtonsoft.Json;

namespace test_peformance.Grains;

[GenerateSerializer, Immutable]
public sealed record class ProductGrainState
{
    [Id(0)] public string Id { get; set; } = Random.Shared.Next(1, 1_000_000).ToString();
    [Id(1)] public string Name { get; set; } = null!;
    [Id(2)] public string Description { get; set; } = null!;
    [Id(4)] public int Quantity { get; set; }
    [Id(5)] public decimal UnitPrice { get; set; }
    [Id(6)] public string DetailsUrl { get; set; } = null!;
    [Id(7)] public string ImageUrl { get; set; } = null!;

    [JsonIgnore]
    public decimal TotalPrice => Math.Round(Quantity * UnitPrice, 2);
}
