using Microsoft.Extensions.DependencyInjection;

namespace Flight.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddFlightApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        return services;
    }
}
