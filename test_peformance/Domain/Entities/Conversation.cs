namespace test_peformance.Domain.Entities;

public class Conversation : BaseEntity
{
    public string? Name { get; set; }
    public string? OrderId { get; set; }
    public DateTime? LastActivityAt { get; set; }

    public virtual ICollection<Message>? Messages { get; set; }
}
