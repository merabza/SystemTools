using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace SystemToolsShared;

public /*open*/ class ServicesCreator
{
    private readonly string _appName;
    private readonly string? _logFileName;
    private readonly string? _logFolder;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ServicesCreator(string? logFolder, string? logFileName, string appName)
    {
        _logFolder = logFolder;
        _logFileName = logFileName;
        _appName = appName;
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(configure => configure.AddSerilog());
        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
    }

    public ServiceProvider? CreateServiceProvider(LogEventLevel consoleLogEventLevel)
    {
        try
        {
            string? logFileName = null;
            if (_logFileName is not null)
                logFileName = _logFileName;
            else if (_logFolder is not null)
                logFileName = Path.Combine(_logFolder, _appName, $"{_appName}.log");

            if (logFileName is not null)
            {
                var extension = ".log";
                if (logFileName.ToLower().EndsWith(".log") || logFileName.ToLower().EndsWith(".txt"))
                    //extension = logFileName.Substring(logFileName.Length - 5);
                    logFileName = logFileName[..^4];

                logFileName += extension;
                Log.Logger = new LoggerConfiguration().WriteTo.Console(consoleLogEventLevel).WriteTo
                    .File(logFileName, encoding: Encoding.UTF8, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }
        catch (Exception)
        {
            return null;
        }
    }
}