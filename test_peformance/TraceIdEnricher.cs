using Serilog.Core;
using Serilog.Events;

public class TraceIdEnricher : ILogEventEnricher
{
    private readonly string _traceId;

    public TraceIdEnricher(string traceId)
    {
        _traceId = traceId;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceId", _traceId));
    }
}