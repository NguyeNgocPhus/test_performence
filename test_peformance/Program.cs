using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using test_peformance;
using test_peformance.Abstractions;
using test_peformance.Event;
using test_peformance.EventHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string redisConnection = builder.Configuration.GetConnectionString("Redis");
builder.Configuration.GetSection("AppDb").Get<AppDbOption>();
var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings");

// dotnet ef migrations add "Conversation" --project test_peformance --context ApplicationDbContext --startup-project test_peformance --output-dir Migrations
// dotnet ef database update --project test_peformance --startup-project test_peformance --context ApplicationDbContext

builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(option =>
{
    option.UseSqlServer(connectionString, sqlOptions =>
    {
        // sqlOptions.AddBulkOperationSupport();
    }).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    option.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => { loggingBuilder.AddConsole(); }));
});

// // Thêm Redis vào DI container
// string redisConnection  = builder.Configuration.GetConnectionString("Redis");
// builder.Services.AddSingleton(new RedisCacheService(redisConnection));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("Hub", options =>
{
    var Key = "UseQueryTrackingBehaviorQueryTrackingBehavior"u8.ToArray();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // on production make it true
        ValidateAudience = false, // on production make it true
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Key),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/streaming-hub")))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy => { policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});
builder.Services.AddSignalR(hubOptions => { hubOptions.EnableDetailedErrors = true; });
builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase2<,>));
builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(EFUnitOfWork));
builder.Services.AddScoped(typeof(IJwtTokenService), typeof(JwtTokenService));
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

// builder.Services.AddHostedService<GrainBackgroundService>();
builder.Services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();
    var factory = new ConnectionFactory()
    {
        HostName = builder.Configuration["EventBusConnection"],
        DispatchConsumersAsync = true
    };

    if (!string.IsNullOrEmpty(builder.Configuration["EventBusUserName"]))
    {
        factory.UserName = builder.Configuration["EventBusUserName"];
    }

    if (!string.IsNullOrEmpty(builder.Configuration["EventBusPassword"]))
    {
        factory.Password = builder.Configuration["EventBusPassword"];
    }

    var retryCount = 5;
    if (!string.IsNullOrEmpty(builder.Configuration["EventBusRetryCount"]))
    {
        retryCount = int.Parse(builder.Configuration["EventBusRetryCount"]);
    }
    logger.LogInformation($"EventBus connection string: {builder.Configuration["EventBusConnection"]}");
    return new DefaultRabbitMqPersistentConnection(factory, logger, retryCount);
});
builder.Services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
{
    var subscriptionClientName = builder.Configuration["SubscriptionClientName"];
    var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
    var iLifetimeScope = sp.GetRequiredService<IServiceScopeFactory>();
    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
    var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

    var retryCount = 5;
    if (!string.IsNullOrEmpty(builder.Configuration["EventBusRetryCount"]))
    {
        retryCount = int.Parse(builder.Configuration["EventBusRetryCount"]);
    }

    return new EventBusRabbitMq(rabbitMqPersistentConnection, logger, iLifetimeScope, eventBusSubscriptionsManager,
        subscriptionClientName, retryCount);
});
builder.Services.AddHealthChecks();
builder.Services.AddScoped<TestEventHandlerEventHandler>();
var app = builder.Build();
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<TestEvent, TestEventHandlerEventHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHealthChecks("/health");
app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();
app.MapHub<ChatHub>("/streaming-hub").RequireAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();