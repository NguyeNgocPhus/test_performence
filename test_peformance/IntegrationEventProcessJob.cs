using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using test_peformance.Abstractions;
using test_peformance.Events;

namespace test_peformance;

public class IntegrationEventProcessJob : BackgroundService
{
    private readonly ILogger<IntegrationEventProcessJob> _logger;
    private readonly IMessageQueueService _queueService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public IntegrationEventProcessJob(ILogger<IntegrationEventProcessJob> logger, IMessageQueueService queueService,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _queueService = queueService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _queueService.DequeueAsync(stoppingToken);
                await ProcessMessageAsync(message);
            }
            catch (OperationCanceledException)
            {
                // Service is stopping
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }
    }

    private async Task ProcessMessageAsync(QueueMessage message)
    {
        _logger.LogInformation("Processing message {MessageId}", message.EventId);
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var type = Type.GetType(message.EntityName);
            if (type != null)
            {
                var dataObj = JsonSerializer.Deserialize(message.ObjEvent, type);
                if (dataObj != null)
                {
                    dbContext.Add(dataObj);
                    await dbContext.SaveChangesAsync();
                }
            }
        }


        _logger.LogInformation("Message {MessageId} processed successfully", message.EventId);
    }
}