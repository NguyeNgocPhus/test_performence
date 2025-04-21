namespace test_peformance.Entities;

[GenerateSerializer]
public class UpdateUnreadConversation
{
    [Id(0)]
    public string OrderId { get; set; }
    [Id(1)]
    public string UserId { get; set; }
    [Id(2)]
    public string BranchId { get; set; }
    [Id(3)]
    public string BrandName { get; set; }
}