using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ReCounterDom.DependencyInjection;

// ReSharper disable once UnusedType.Global
public static class ReCounterDomDependencyInjection
{
    public static IServiceCollection AddReCounterDom(this IServiceCollection services, bool debugMode)
    {
        if (debugMode)
            Console.WriteLine($"{nameof(AddReCounterDom)} Started");

        services.AddSignalR()
            .AddJsonProtocol(options => { options.PayloadSerializerOptions.PropertyNamingPolicy = null; })
            .AddHubOptions<ReCounterMessagesHub>(options => { options.EnableDetailedErrors = true; });

        services.AddSingleton<ReCounterQueuedHostedService>();
        services.AddSingleton<IHostedService>(p => p.GetRequiredService<ReCounterQueuedHostedService>());
        services.AddSingleton<IReCounterBackgroundTaskQueue, ReCounterBackgroundTaskQueue>();


        if (debugMode)
            Console.WriteLine($"{nameof(AddReCounterDom)} Finished");

        return services;
    }
}