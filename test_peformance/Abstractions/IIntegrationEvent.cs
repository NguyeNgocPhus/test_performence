namespace test_peformance.Events;

public interface IIntegrationEvent
{
    public string EventId { get; set; }
    public string EntityName { get; set; }
}