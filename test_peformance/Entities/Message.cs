namespace test_peformance.Entities;

public class Message : BaseEntity
{
    public string? Name { get; set; }
    public int ConversationId { get; set; }
    public DateTime? CreatedAt { get; set; }

    // Navigation property for conversation
    public virtual Conversation? Conversation { get; set; }
} 