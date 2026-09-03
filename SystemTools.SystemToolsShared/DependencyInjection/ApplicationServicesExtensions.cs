using System;
using Microsoft.Extensions.DependencyInjection;
using SystemTools.SystemToolsShared.App;

namespace SystemTools.SystemToolsShared.DependencyInjection;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApp(this IServiceCollection services, Action<AppOptions> setupAction)
    {
        services.AddSingleton<IApplication, App.App>();
        services.Configure(setupAction);
        return services;
    }
}
