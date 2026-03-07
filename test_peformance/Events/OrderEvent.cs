using test_peformance.Infrastructure.Messaging.EventBus;

namespace test_peformance.Events;

public class OrderEvent : IntegrationEvent
{
    public string Name { get; init; }
    public decimal Price { get; init; }
}
