using System;
using System.Globalization;
using System.IO;
using System.Text;
using Serilog;
using Serilog.Events;

namespace SystemTools.SerilogStuff;

public static class SerilogLoggerCreator
{
    public static void CreateLogger(LogEventLevel consoleLogEventLevel, string appName, string? logFileName,
        string? logFolder)
    {
        try
        {
            string? logFile = null;
            if (!string.IsNullOrWhiteSpace(logFileName))
            {
                logFile = logFileName;
            }
            else if (logFolder is not null)
            {
                logFile = Path.Combine(logFolder, appName, $"{appName}.log");
            }

            if (logFile is null)
            {
                return;
            }

            const string extension = ".log";
            if (logFile.ToUpperInvariant().EndsWith(".LOG", StringComparison.Ordinal) ||
                logFile.ToUpperInvariant().EndsWith(".TXT", StringComparison.Ordinal))
            {
                logFile = logFile[..^4];
            }

            logFile += extension;
            Log.Logger = new LoggerConfiguration().WriteTo
                .Console(consoleLogEventLevel, formatProvider: CultureInfo.InvariantCulture).WriteTo.File(logFile,
                    encoding: Encoding.UTF8, rollingInterval: RollingInterval.Day,
                    formatProvider: CultureInfo.InvariantCulture).CreateLogger();
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
