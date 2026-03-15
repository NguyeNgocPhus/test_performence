namespace test_peformance.Entities;

[GenerateSerializer]
public class UpdateUnreadConversation
{
    [Id(0)]
    public string OrderId { get; set; } = string.Empty;
    [Id(1)]
    public string UserId { get; set; } = string.Empty;
    [Id(2)]
    public string BranchId { get; set; } = string.Empty;
    [Id(3)]
    public string BrandName { get; set; } = string.Empty;
}