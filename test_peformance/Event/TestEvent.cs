using EventBus.Events;

namespace test_peformance.Event;

public class TestEvent : IntegrationEvent
{
    public string? Name { get; init; }
}