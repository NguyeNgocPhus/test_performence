﻿using EventBus.Events;

namespace IntegrationEventLogEF.Services;

public class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
{
    private readonly DbConnection _dbConnection;
    private readonly List<Type> _eventTypes;
    private readonly IntegrationEventLogContext _integrationEventLogContext;
    private volatile bool _disposedValue;

    public IntegrationEventLogService(DbConnection dbConnection)
    {
        _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        _integrationEventLogContext = new IntegrationEventLogContext(
            new DbContextOptionsBuilder<IntegrationEventLogContext>()
                .UseSqlServer(_dbConnection)
                .Options);

        _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
            .GetTypes()
            .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
            .ToList();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        var tid = transactionId.ToString();

        var result = await _integrationEventLogContext.IntegrationEventLogs
            .Where(e => e.TransactionId == tid && e.State == EventStateEnum.NotPublished).ToListAsync();

        if (result.Any())
            return result.OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));

        return new List<IntegrationEventLogEntry>();
    }

    public Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

        _integrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
        _integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);

        return _integrationEventLogContext.SaveChangesAsync();
    }

    public Task MarkEventAsPublishedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, EventStateEnum.Published);
    }

    public Task MarkEventAsInProgressAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, EventStateEnum.InProgress);
    }

    public Task MarkEventAsFailedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
    }

    private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
    {
        var eventLogEntry = _integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
        eventLogEntry.State = status;

        if (status == EventStateEnum.InProgress)
            eventLogEntry.TimesSent++;

        _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

        return _integrationEventLogContext.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing) _integrationEventLogContext?.Dispose();


            _disposedValue = true;
        }
    }
}