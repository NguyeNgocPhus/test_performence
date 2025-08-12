using System.Diagnostics;
using Serilog;
using Serilog.Context;

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
        // using (LogContext.PushProperty("TraceId", traceId))
        // {
        var stopwatch = Stopwatch.StartNew();
        if (context.Request.Path != "/health")
        {
            // Log request
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
                // Log response
                Log.Information("HTTP {Method} {Path} responded {StatusCode} in {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            
            // Gắn vào response header
            context.Response.Headers["Request-Id"] = traceId;
        }
        // }
    }
}