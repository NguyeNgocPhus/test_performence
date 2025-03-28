using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using test_peformance;
using test_peformance.Abstractions;
using test_peformance.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// dotnet ef migrations add "Conversation" --project test_peformance --context ApplicationDbContext --startup-project test_peformance --output-dir Migrations
// dotnet ef database update --project test_peformance --startup-project test_peformance --context ApplicationDbContext
builder.Configuration.GetSection("AppDb").Get<AppDbOption>();
var connectionString = builder.Configuration.GetConnectionString("ConnectionStrings");
builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(option =>
{
    option.UseSqlServer(connectionString, sqlOptions =>
    {
        // sqlOptions.AddBulkOperationSupport();
    }).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    option.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => { loggingBuilder.AddConsole(); }));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("Hub",options =>
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
builder.Services.AddCors(cors => cors.AddDefaultPolicy(policy => policy
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithExposedHeaders("Content-Disposition")));

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
});
builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase2<,>));
builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(EFUnitOfWork));
builder.Services.AddScoped(typeof(IJwtTokenService), typeof(JwtTokenService));
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddSingleton<InMemoryMessageQueue>();
builder.Services.AddSingleton<IMessageQueueService, MessageQueueService>();
builder.Services.AddSingleton<IEventBus, EventBus>();
// builder.Services.AddHostedService<IntegrationEventProcessJob>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();
app.MapHub<ChatHub>("/streaming-hub").RequireAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();