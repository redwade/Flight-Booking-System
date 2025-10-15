using AI.Core.Repositories;
using AI.Core.Services;
using AI.Infrastructure.Repositories;
using AI.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAIInfrastructure(this IServiceCollection services, string ollamaBaseUrl = "http://localhost:11434")
    {
        // Register repositories
        services.AddSingleton<IChatMessageRepository, InMemoryChatMessageRepository>();

        // Register AI service with HttpClient
        services.AddHttpClient<IAIService, OllamaAIService>(client =>
        {
            client.BaseAddress = new Uri(ollamaBaseUrl);
            client.Timeout = TimeSpan.FromMinutes(2); // LLM responses can take time
        });

        return services;
    }
}
