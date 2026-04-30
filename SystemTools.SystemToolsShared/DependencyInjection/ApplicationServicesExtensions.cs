using System;
using Microsoft.Extensions.DependencyInjection;
using SystemTools.SystemToolsShared.App;

namespace SystemTools.SystemToolsShared.DependencyInjection;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services,
        Action<ApplicationOptions> setupAction)
    {
        services.AddSingleton<IApplication, Application>();
        services.Configure(setupAction);
        return services;
    }
}
