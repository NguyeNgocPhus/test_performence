using test_peformance.Application.Conversations;
using test_peformance.Application.Cronjob;
using test_peformance.Application.Departments;
using test_peformance.Application.Messages;
using test_peformance.Domain.Abstractions;
using test_peformance.Infrastructure;

namespace test_peformance.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICronjobService, CronjobService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        return services;
    }
}
