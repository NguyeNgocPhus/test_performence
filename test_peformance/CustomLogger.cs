using test_peformance.Abstractions;

namespace test_peformance;

public class CustomLogger: ICustomLogger
{
    private readonly ILogger<CustomLogger> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomLogger(ILogger<CustomLogger> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public void LogInformation(string message, params object[] args)
    {
        var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "N/A";
        using (Serilog.Context.LogContext.PushProperty("TraceId", traceId))
        {
            _logger.LogInformation(message, args);
        }
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "N/A";
        using (Serilog.Context.LogContext.PushProperty("TraceId", traceId))
        {
            _logger.LogError(ex, message, args);
        }
    }

    public void LogWarning(string message, params object[] args)
    {
        var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? "N/A";
        using (Serilog.Context.LogContext.PushProperty("TraceId", traceId))
        {
            _logger.LogWarning(message, args);
        }
    }
}