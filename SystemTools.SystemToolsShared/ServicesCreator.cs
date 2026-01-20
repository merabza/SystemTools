using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace SystemTools.SystemToolsShared;

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
        //services.AddHttpClient();
    }

    public ServiceProvider? CreateServiceProvider(LogEventLevel consoleLogEventLevel)
    {
        try
        {
            string? logFileName = null;
            if (!string.IsNullOrWhiteSpace(_logFileName))
            {
                logFileName = _logFileName;
            }
            else if (_logFolder is not null)
            {
                logFileName = Path.Combine(_logFolder, _appName, $"{_appName}.log");
            }

            ////check with regex logFileName is valid filename
            //if (!FileNameValidator.IsValidFileName(logFileName))
            //    return null;

            if (logFileName is not null)
            {
                const string extension = ".log";
                if (logFileName.ToUpperInvariant().EndsWith(".LOG", StringComparison.Ordinal) ||
                    logFileName.ToUpperInvariant().EndsWith(".TXT", StringComparison.Ordinal))
                    //extension = logFileName.Substring(logFileName.Length - 5);
                {
                    logFileName = logFileName[..^4];
                }

                logFileName += extension;
                Log.Logger = new LoggerConfiguration().WriteTo
                    .Console(consoleLogEventLevel, formatProvider: CultureInfo.InvariantCulture).WriteTo
                    .File(logFileName, encoding: Encoding.UTF8, rollingInterval: RollingInterval.Day,
                        formatProvider: CultureInfo.InvariantCulture).CreateLogger();
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
