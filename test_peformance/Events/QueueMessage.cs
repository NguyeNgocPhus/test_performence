namespace test_peformance.Events;

public class QueueMessage : IIntegrationEvent
{
    public string EventId { get; set; }
    public string EntityName { get; set; }
    public string ObjEvent { get; set; }
}