using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Core.Repositories;
using Payment.Infrastructure.Data;
using Payment.Infrastructure.Repositories;

namespace Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // PostgreSQL Configuration
        var connectionString = configuration.GetConnectionString("PostgreSQL") 
            ?? "Host=localhost;Database=PaymentDB;Username=postgres;Password=postgres";

        services.AddDbContext<PaymentDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repository Registration
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}
