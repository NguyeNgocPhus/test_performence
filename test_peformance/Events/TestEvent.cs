using test_peformance.Infrastructure.Messaging.EventBus;

namespace test_peformance.Events;

public class TestEvent : IntegrationEvent
{
    public string? Name { get; init; }
}
