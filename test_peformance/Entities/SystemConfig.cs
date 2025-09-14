namespace test_peformance.Entities;

public class SystemConfig
    : BaseEntity
{
    public string? Name { get; set; }
    public string? Value { get; set; }
    public DateTime TimeChanged { get; set; }
}