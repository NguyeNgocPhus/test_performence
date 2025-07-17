// using System.Text.Json;
// using test_peformance.Abstractions;
// using test_peformance.Events;
//
// namespace test_peformance;
//
// public class IntegrationEventProcessJob : BackgroundService
// {
//     private readonly ILogger<IntegrationEventProcessJob> _logger;
//     private readonly IMessageQueueService _queueService;
//     private readonly IServiceScopeFactory _serviceScopeFactory;
//     private readonly SemaphoreSlim _semaphore;
//     private readonly int _workerCount = 3;
//     private readonly List<Task> _workers;
//
//     public IntegrationEventProcessJob(ILogger<IntegrationEventProcessJob> logger, IMessageQueueService queueService,
//         IServiceScopeFactory serviceScopeFactory, SemaphoreSlim semaphore)
//     {
//         _logger = logger;
//         _queueService = queueService;
//         _serviceScopeFactory = serviceScopeFactory;
//         _semaphore = new SemaphoreSlim(_workerCount);
//         _workers = new List<Task>();
//     }
//
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         _logger.LogInformation("Starting {WorkerCount} message processors", _workerCount);
//
//         // Start multiple workers
//         for (int i = 0; i < _workerCount; i++)
//         {
//             var workerId = $"Worker-{i + 1}";
//             var worker = RunWorkerAsync(workerId, stoppingToken);
//             _workers.Add(worker);
//         }
//
//         // Wait for all workers to complete
//         await Task.WhenAll(_workers);
//     }
//
//     private async Task RunWorkerAsync(string workerId, CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 await _semaphore.WaitAsync(stoppingToken);
//                 var message = await _queueService.DequeueAsync(stoppingToken);
//                 await ProcessMessageAsync(message);
//             }
//             catch (OperationCanceledException)
//             {
//                 // Service is stopping
//                 break;
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error processing message");
//             }
//             finally
//             {
//                 _semaphore.Release();
//             }
//         }
//     }
//
//     private async Task ProcessMessageAsync(QueueMessage message)
//     {
//         _logger.LogInformation("Processing message {MessageId}", message.EventId);
//         using (var scope = _serviceScopeFactory.CreateScope())
//         {
//             var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//
//             var type = Type.GetType(message.EntityName);
//             if (type != null)
//             {
//                 var dataObj = JsonSerializer.Deserialize(message.ObjEvent, type);
//                 if (dataObj != null)
//                 {
//                     dbContext.Add(dataObj);
//                     await dbContext.SaveChangesAsync();
//                 }
//             }
//         }
//
//
//         _logger.LogInformation("Message {MessageId} processed successfully", message.EventId);
//     }
// }