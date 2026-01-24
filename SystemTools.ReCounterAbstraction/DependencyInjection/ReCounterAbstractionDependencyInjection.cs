using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SystemTools.ReCounterAbstraction.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class ReCounterAbstractionDependencyInjection
{
    public static IServiceCollection AddReCounterAbstraction(this IServiceCollection services, ILogger? debugLogger)
    {
        debugLogger?.Information("{MethodName} Started", nameof(AddReCounterAbstraction));

        services.AddSignalR()
            .AddJsonProtocol(options => { options.PayloadSerializerOptions.PropertyNamingPolicy = null; })
            .AddHubOptions<ReCounterMessagesHub>(options => { options.EnableDetailedErrors = true; });

        services.AddSingleton<ReCounterQueuedHostedService>();
        services.AddSingleton<IHostedService>(p => p.GetRequiredService<ReCounterQueuedHostedService>());
        services.AddSingleton<IReCounterBackgroundTaskQueue, ReCounterBackgroundTaskQueue>();

        debugLogger?.Information("{MethodName} Finished", nameof(AddReCounterAbstraction));

        return services;
    }
}
