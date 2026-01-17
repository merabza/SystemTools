using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ReCounterAbstraction.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class ReCounterAbstractionDependencyInjection
{
    public static IServiceCollection AddReCounterAbstraction(this IServiceCollection services, ILogger logger,
        bool debugMode)
    {
        if (debugMode)
            logger.Information($"{nameof(AddReCounterAbstraction)} Started");

        services.AddSignalR()
            .AddJsonProtocol(options => { options.PayloadSerializerOptions.PropertyNamingPolicy = null; })
            .AddHubOptions<ReCounterMessagesHub>(options => { options.EnableDetailedErrors = true; });

        services.AddSingleton<ReCounterQueuedHostedService>();
        services.AddSingleton<IHostedService>(p => p.GetRequiredService<ReCounterQueuedHostedService>());
        services.AddSingleton<IReCounterBackgroundTaskQueue, ReCounterBackgroundTaskQueue>();

        if (debugMode)
            logger.Information($"{nameof(AddReCounterAbstraction)} Finished");

        return services;
    }
}