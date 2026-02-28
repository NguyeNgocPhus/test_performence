using EventBus.Events;

namespace test_peformance.Event;

public class OrderEvent: IntegrationEvent
{
    public string Name { get; init; }
    public decimal Price { get; init; }
}