using Microsoft.EntityFrameworkCore;
using test_peformance;
using test_peformance.Abstractions;
using test_peformance.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// dotnet ef migrations add "$2" --project Identity.Infrastructure --context ApplicationDbContext --startup-project Identity.WebApi --output-dir Databases/Migrations
// dotnet ef database update --project Identity.Infrastructure --startup-project Identity.WebApi --context ApplicationDbContext
var appDb = builder.Configuration.GetSection("AppDb").Get<AppDbOption>();
var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings");
builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(option =>
{
    option.UseSqlServer(connectionString, sqlOptions =>
    {
        // sqlOptions.AddBulkOperationSupport();
    }).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    option.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => { loggingBuilder.AddConsole(); }));
});

builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase2<,>));
builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(EFUnitOfWork));
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddSingleton<InMemoryMessageQueue>();
builder.Services.AddSingleton<IMessageQueueService, MessageQueueService>();
builder.Services.AddSingleton<IEventBus, EventBus>();
builder.Services.AddHostedService<IntegrationEventProcessJob>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();