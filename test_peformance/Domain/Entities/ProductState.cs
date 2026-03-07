using Newtonsoft.Json;

namespace test_peformance.Domain.Entities;

public sealed record class ProductState
{
    public string Id { get; set; } = Random.Shared.Next(1, 1_000_000).ToString();
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string DetailsUrl { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;

    [JsonIgnore]
    public decimal TotalPrice =>
        Math.Round(Quantity * UnitPrice, 2);
}
