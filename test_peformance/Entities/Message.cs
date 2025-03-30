namespace test_peformance.Entities;

public class Message : BaseEntity
{
    public string? Content { get; set; }
    public string? File { get; set; }
    public int ConversationId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    // Navigation property for conversation
    public virtual Conversation? Conversation { get; set; }
}