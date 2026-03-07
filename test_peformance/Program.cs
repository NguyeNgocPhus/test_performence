using Microsoft.Extensions.FileProviders;
using test_peformance.Infrastructure.Messaging.EventBus;
using test_peformance.Infrastructure.Messaging.RabbitMQ;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using test_peformance.Application;
using test_peformance.Events;
using test_peformance.EventHandling;
using test_peformance.Infrastructure.Cache;
using test_peformance.Infrastructure.Messaging;
using test_peformance.Infrastructure.Persistence;
using test_peformance.Infrastructure.Security;
using test_peformance.Presentation.Hubs;
using test_peformance.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authorization", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.HttpsPolicy", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] [TraceId : {RequestId}] {Message:lj}{NewLine}{Exception}",
        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
    .WriteTo.File("logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] [TraceId : {RequestId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

LogContext.PushProperty("RequestId", "SYSTEM");
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var isLoadRedis = builder.Configuration.GetValue<bool>("IsLoadRedis");
var isLoadRabbit = builder.Configuration.GetValue<bool>("IsLoadRabbit");
var isLoadKafka = builder.Configuration.GetValue<bool>("IsLoadKafka");

builder.Services
    .AddApplicationServices()
    .AddPersistence(builder.Configuration)
    .AddJwtAuth(builder.Configuration);

if (isLoadKafka)
    builder.Services.AddKafka(builder.Configuration);

if (isLoadRedis)
    builder.Services.AddRedis(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});
builder.Services.AddSignalR(hubOptions => { hubOptions.EnableDetailedErrors = true; });

if (isLoadRabbit)
{
    builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
    builder.Services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();
        var factory = new ConnectionFactory()
        {
            HostName = builder.Configuration["EventBusConnection"],
            DispatchConsumersAsync = true
        };

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusUserName"]))
            factory.UserName = builder.Configuration["EventBusUserName"];

        if (!string.IsNullOrEmpty(builder.Configuration["EventBusPassword"]))
            factory.Password = builder.Configuration["EventBusPassword"];

        var retryCount = 5;
        if (!string.IsNullOrEmpty(builder.Configuration["EventBusRetryCount"]))
            retryCount = int.Parse(builder.Configuration["EventBusRetryCount"]);

        logger.LogInformation("EventBus connection string: {Host}", builder.Configuration["EventBusConnection"]);
        return new DefaultRabbitMqPersistentConnection(factory, logger, retryCount);
    });
    builder.Services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
    {
        var subscriptionClientName = builder.Configuration["SubscriptionClientName"];
        var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
        var iLifetimeScope = sp.GetRequiredService<IServiceScopeFactory>();
        var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        var retryCount = 5;
        if (!string.IsNullOrEmpty(builder.Configuration["EventBusRetryCount"]))
            retryCount = int.Parse(builder.Configuration["EventBusRetryCount"]);

        return new EventBusRabbitMq(rabbitMqPersistentConnection, iLifetimeScope, eventBusSubscriptionsManager,
            subscriptionClientName, retryCount);
    });
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

if (isLoadRabbit)
{
    builder.Services.AddScoped<TestEventHandlerEventHandler>();
    builder.Services.AddScoped<OrderEventHandlerEventHandler>();
}

var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter());

var app = builder.Build();

if (isLoadRabbit)
{
    var eventBus = app.Services.GetRequiredService<IEventBus>();
    eventBus.Subscribe<TestEvent, TestEventHandlerEventHandler>();
    eventBus.Subscribe<OrderEvent, OrderEventHandlerEventHandler>();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
    RequestPath = "/uploads"
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.MapHealthChecks("/health");
app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();
app.MapHub<ChatHub>("/streaming-hub").RequireAuthorization();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting web host");
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
