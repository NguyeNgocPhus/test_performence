using test_peformance.Abstractions;
using test_peformance.Events;

namespace test_peformance;

public class IntegrationEventProcessJob : BackgroundService
{
    private readonly InMemoryMessageQueue _inMemoryMessageQueue;
    private readonly ILogger<IntegrationEventProcessJob> _logger;
    
    public IntegrationEventProcessJob(InMemoryMessageQueue inMemoryMessageQueue, ILogger<IntegrationEventProcessJob> logger)
    {
        _inMemoryMessageQueue = inMemoryMessageQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     var listData = new List<IIntegrationEvent>();
        //     
        //     for (int i = 0; i < 10000; i++) 
        //     {
        //         var a = _inMemoryMessageQueue.Reader.TryRead(out var message);
        //         if (a)
        //             listData.Add(message);
        //     }
        //     WeatherForecast.Sum += listData.Count;
        //     // _logger.LogInformation($"{WeatherForecast.Sum} integration events processed.");
        //     await Task.Delay(2000, stoppingToken);
        // }
    }
}