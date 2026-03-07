using System.Text.Json;
using test_peformance.Infrastructure.Messaging.EventBus;
using Serilog;
using Serilog.Context;
using test_peformance.Events;

namespace test_peformance.EventHandling;

public class TestEventHandlerEventHandler : IIntegrationEventHandler<TestEvent>
{
    public async Task Handle(TestEvent @event)
    {
        LogContext.PushProperty("RequestId", @event.TraceId);
        Log.Information($"Integration Event {JsonSerializer.Serialize(@event)}");
    }
}
