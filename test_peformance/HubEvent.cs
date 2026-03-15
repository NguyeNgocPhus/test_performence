using test_peformance.Events;

namespace test_peformance;

public class HubEvent : IIntegrationEvent
{
    public string EventId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

}