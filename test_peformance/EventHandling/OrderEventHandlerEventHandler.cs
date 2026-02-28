using System.Text.Json;
using EventBus.Abstractions;
using Serilog;
using Serilog.Context;
using test_peformance.Event;

namespace test_peformance.EventHandling;
// ádasd
public class OrderEventHandlerEventHandler : IIntegrationEventHandler<OrderEvent>
{

    /// <summary>
    /// Event handler which confirms that the grace period
    /// has been completed and order will not initially be cancelled.
    /// Therefore, the order process continues for validation. 
    /// </summary>
    /// <param name="event">       
    /// </param>
    /// <returns></returns>
    public async Task Handle(OrderEvent @event)
    {
        LogContext.PushProperty("RequestId",  @event.TraceId);

        Log.Information($"Integration Event {JsonSerializer.Serialize(@event)}");
    }
}