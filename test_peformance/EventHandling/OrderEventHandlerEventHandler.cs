using System.Text.Json;
using test_peformance.Infrastructure.Messaging.EventBus;
using Serilog;
using Serilog.Context;
using test_peformance.Events;

namespace test_peformance.EventHandling;

public class OrderEventHandlerEventHandler : IIntegrationEventHandler<OrderEvent>
{
    public async Task Handle(OrderEvent @event)
    {
        LogContext.PushProperty("RequestId", @event.TraceId);
        Log.Information($"Integration Event {JsonSerializer.Serialize(@event)}");
    }
}
