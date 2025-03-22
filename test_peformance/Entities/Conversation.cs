namespace test_peformance.Entities;

public class Conversation : BaseEntity
{
    public string? Name { get; set; }
    public string? OrderId { get; set; }
    public DateTime? LastActivityAt { get; set; }

    // Navigation property for messages
    public virtual ICollection<Message>? Messages { get; set; }
} 