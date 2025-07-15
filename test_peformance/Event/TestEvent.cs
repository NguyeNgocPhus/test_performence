using EventBus.Events;

namespace test_peformance.Event;

public record TestEvent : IntegrationEvent
{
    public string? Name { get; init; }
}