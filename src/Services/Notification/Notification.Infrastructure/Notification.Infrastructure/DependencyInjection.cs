using Microsoft.Extensions.DependencyInjection;
using Notification.Core.Repositories;
using Notification.Infrastructure.Repositories;

namespace Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services)
    {
        // Repository Registration (In-Memory)
        services.AddSingleton<INotificationRepository, NotificationRepository>();

        return services;
    }
}
