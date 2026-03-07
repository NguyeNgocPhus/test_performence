using System.Diagnostics;
using Serilog;

namespace test_peformance.Presentation.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.TraceIdentifier;
        var stopwatch = Stopwatch.StartNew();
        if (context.Request.Path != "/health")
        {
            Log.Information("HTTP {Method} {Path} started",
                context.Request.Method,
                context.Request.Path);
        }

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            if (context.Request.Path != "/health")
            {
                Log.Information("HTTP {Method} {Path} responded {StatusCode} in {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }

            context.Response.Headers["Request-Id"] = traceId;
        }
    }
}
