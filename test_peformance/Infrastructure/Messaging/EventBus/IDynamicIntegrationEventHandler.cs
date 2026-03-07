namespace test_peformance.Infrastructure.Messaging.EventBus;

public interface IDynamicIntegrationEventHandler
{
    Task Handle(dynamic eventData);
}
