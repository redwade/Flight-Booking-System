using Flight.Core.Repositories;
using Flight.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Flight.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFlightInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Configuration
        var connectionString = configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "FlightDB";

        services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        // Repository Registration
        services.AddScoped<IFlightRepository, FlightRepository>();

        return services;
    }
}
