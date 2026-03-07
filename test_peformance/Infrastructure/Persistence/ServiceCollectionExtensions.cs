using Microsoft.EntityFrameworkCore;
using test_peformance.Domain.Abstractions;
using test_peformance.Infrastructure.Persistence.Repositories;

namespace test_peformance.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionStrings");

        services.AddDbContext<ApplicationDbContext>(option =>
        {
            option.UseSqlServer(connectionString)
                  .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            option.UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
        });

        services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
        services.AddScoped<IUnitOfWork, EFUnitOfWork>();

        return services;
    }
}
