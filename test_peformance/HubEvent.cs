using test_peformance.Events;

namespace test_peformance;

public class HubEvent : IIntegrationEvent
{
    public string EventId { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
}