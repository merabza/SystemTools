using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace SystemTools.SerilogStuff.DependencyInjection;

public static class SerilogLoggerServicesExtensions
{
    public static IServiceCollection AddSerilogLoggerService(this IServiceCollection services,
        LogEventLevel consoleLogEventLevel, string appName, string? logFolder = null, string? logFileName = null)
    {
        SerilogLoggerCreator.CreateLogger(consoleLogEventLevel, appName, logFileName, logFolder);
        services.AddLogging(configure => configure.AddSerilog());
        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
        return services;
    }
}
