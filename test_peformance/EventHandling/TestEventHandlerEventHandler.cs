using System.Text.Json;
using EventBus.Abstractions;
using test_peformance.Event;

namespace test_peformance.EventHandling;
public class TestEventHandlerEventHandler : IIntegrationEventHandler<TestEvent>
{
    
    private readonly ILogger<TestEventHandlerEventHandler> _logger;

    public TestEventHandlerEventHandler(
      
        ILogger<TestEventHandlerEventHandler> logger)
    {
     
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Event handler which confirms that the grace period
    /// has been completed and order will not initially be cancelled.
    /// Therefore, the order process continues for validation. 
    /// </summary>
    /// <param name="event">       
    /// </param>
    /// <returns></returns>
    public async Task Handle(TestEvent @event)
    {
        _logger.LogInformation($"GracePeriodConfirmed Integration Event {JsonSerializer.Serialize(@event)}");
    }
}
