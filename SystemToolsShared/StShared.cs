//using FilesProcessing;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneOf;
using SystemToolsShared.Errors;

namespace SystemToolsShared;

public static class StShared
{
    public static string TimeTakenMessage(DateTime startDateTime)
    {
        var endDateTime = DateTime.Now; //პროცესის დასრულების დრო
        var taken = endDateTime - startDateTime; //დავიანგარიშოთ რა დრო დასჭირდა მთლიანად პროცესს
        var totalHours = (int)taken.TotalHours; //საათები თუ არ დასჭირდა რომ არ გამოვიტანოთ
        var totalMinutes = (int)taken.TotalMinutes; //საათები თუ არ დასჭირდა რომ არ გამოვიტანოთ
        //დახარჯული დროის შესახებ ინფორმაციის გამოტანა ფორმაზე.
        return
            $"Time taken {(totalHours == 0 ? string.Empty : $"{totalHours} hours, ")}{(totalMinutes == 0 ? string.Empty : $"{taken.Minutes} minutes, ")}{taken.Seconds} seconds";
    }

    public static OneOf<(string, int), IEnumerable<Err>> RunProcessWithOutput(bool useConsole, ILogger? logger,
        string programFileName, string arguments, int[]? allowExitCodes = null)
    {
        ConsoleWriteInformationLine(logger, useConsole, "Running{0}{1} {2}", Environment.NewLine, programFileName,
            arguments);

        // ReSharper disable once using
        // ReSharper disable once DisposableConstructor
        using var proc = new Process();
        proc.StartInfo = new ProcessStartInfo
        {
            FileName = programFileName,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        StringBuilder sb = new();
        proc.Start();
        while (!proc.StandardOutput.EndOfStream)
        {
            var line = proc.StandardOutput.ReadLine();
            if (useConsole)
                Console.WriteLine(line);
            sb.AppendLine(line);
        }
        //message = "output for '{0} {1}' is{2}{3}";


        if (IsAllowExitCode(proc.ExitCode, allowExitCodes))
        {
            ConsoleWriteInformationLine(logger, useConsole, "output for '{0} {1}' is{2}{3}", programFileName, arguments,
                Environment.NewLine, sb);
            return (sb.ToString(), proc.ExitCode);
        }

        var errorMessage = $"{programFileName} {arguments} process was finished with errors. ExitCode={proc.ExitCode}";
        if (useConsole || logger is not null)
            WriteErrorLine(errorMessage, useConsole, logger);

        return new[] { SystemToolsErrors.RunProcessError(errorMessage) };
    }


    private static bool IsAllowExitCode(int exitCode, int[]? allowExitCodes)
    {
        if (exitCode == 0)
            return true;
        return allowExitCodes is not null && allowExitCodes.Contains(exitCode);
    }


    public static Option<IEnumerable<Err>> RunProcess(bool useConsole, ILogger? logger, string programFileName,
        string arguments, int[]? allowExitCodes = null, bool useErrorLine = true, int waitForExit = Timeout.Infinite)
    {
        ConsoleWriteInformationLine(logger, useConsole, "Running {0} {1}...", programFileName, arguments);

        // ReSharper disable once using
        using var proc = Process.Start(programFileName, arguments);

        if (waitForExit == 0)
            return null;

        ConsoleWriteInformationLine(logger, useConsole, "Wait For Exit {0}", programFileName);

        proc.WaitForExit(waitForExit < 0 ? Timeout.Infinite : waitForExit);

        ConsoleWriteInformationLine(logger, useConsole, "{0} finished", programFileName);

        if (IsAllowExitCode(proc.ExitCode, allowExitCodes))
            return null;

        var errorMessage = $"{programFileName} {arguments} process was finished with errors. ExitCode={proc.ExitCode}";
        if (useErrorLine && (useConsole || logger is not null))
            WriteErrorLine(errorMessage, useConsole, logger);

        return new[] { SystemToolsErrors.RunProcessError(errorMessage) };
    }

    public static bool RunCmdProcess(string command, string? projectPath = null)
    {
        var psiNpmRunDist = new ProcessStartInfo
        {
            FileName = "cmd",
            RedirectStandardInput = true,
            WorkingDirectory = projectPath ?? Directory.GetCurrentDirectory()
        };
        // ReSharper disable once using
        using var pNpmRunDist = Process.Start(psiNpmRunDist);
        if (pNpmRunDist is null)
            return false;
        pNpmRunDist.StandardInput.WriteLine($"{command} & exit");
        pNpmRunDist.WaitForExit();

        return true;
    }

    public static bool CreateFolder(string path, bool useConsole)
    {
        var checkedPath = FileStat.CreateFolderIfNotExists(path, useConsole);

        if (checkedPath is not null)
            return true;
        WriteErrorLine($"Cannot create Folder {path}.", useConsole);
        return false;
    }


    public static void Pause()
    {
        Console.WriteLine("press any key...");
        Console.ReadKey(true);
    }

    public static void ConsoleWriteInformationLine(ILogger? logger, bool useConsole, string message,
        params object?[] args)
    {
        logger?.LogInformation(message, args);
        if (!useConsole)
            return;
        ConsoleWriteFormattedLine(message, args);
    }


    private static void ConsoleWriteFormattedLine(string message, params object?[] args)
    {
        //var vsb = new StringBuilder(256);
        var scanIndex = 0;
        var endIndex = message.Length;
        var argIndex = 0;

        while (scanIndex < endIndex)
        {
            var openBraceIndex = FindBraceIndex(message, '{', scanIndex, endIndex);
            if (scanIndex == 0 && openBraceIndex == endIndex)
            {
                // No holes found.
                Console.WriteLine(message);
                return;
            }

            var closeBraceIndex = FindBraceIndex(message, '}', openBraceIndex, endIndex);

            if (closeBraceIndex == endIndex)
            {
                Console.Write(message[scanIndex..endIndex]);
                scanIndex = endIndex;
            }
            else
            {
                if (openBraceIndex > scanIndex)
                    Console.Write(message[scanIndex..openBraceIndex]);
                var existingColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Cyan;
                if (argIndex < args.Length)
                    Console.Write(args[argIndex++]);
                else
                    Console.Write(message.Substring(openBraceIndex + 1, closeBraceIndex - 1)); //value
                Console.ForegroundColor = existingColor;

                scanIndex = closeBraceIndex + 1;
            }
        }

        Console.WriteLine();
    }

    private static int FindBraceIndex(string format, char brace, int startIndex, int endIndex)
    {
        // Example: {{prefix{{{Argument}}}suffix}}.
        var braceIndex = endIndex;
        var scanIndex = startIndex;
        var braceOccurrenceCount = 0;

        while (scanIndex < endIndex)
        {
            if (braceOccurrenceCount > 0 && format[scanIndex] != brace)
            {
                if (braceOccurrenceCount % 2 == 0)
                {
                    // Even number of '{' or '}' found. Proceed search with next occurrence of '{' or '}'.
                    braceOccurrenceCount = 0;
                    braceIndex = endIndex;
                }
                else
                    // An unescaped '{' or '}' found.
                {
                    break;
                }
            }
            else if (format[scanIndex] == brace)
            {
                if (brace == '}')
                {
                    if (braceOccurrenceCount == 0)
                        // For '}' pick the first occurrence.
                        braceIndex = scanIndex;
                }
                else
                    // For '{' pick the last occurrence.
                {
                    braceIndex = scanIndex;
                }

                braceOccurrenceCount++;
            }

            scanIndex++;
        }

        return braceIndex;
    }

    public static void WriteWarningLine(string warningText, bool useConsole, ILogger? logger = null,
        bool pauseAfter = false)
    {
        logger?.LogWarning(warningText);
        if (!useConsole)
            return;
        var existingColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[warning] ");
        Console.ForegroundColor = existingColor;
        Console.WriteLine(warningText);
        if (pauseAfter)
            Pause();
    }

    public static void WriteErrorLine(string errorText, bool useConsole, ILogger? logger = null, bool pauseAfter = true)
    {
        logger?.LogError(errorText);
        if (!useConsole)
            return;
        var existingColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR] ");
        Console.ForegroundColor = existingColor;
        Console.WriteLine(errorText);
        if (pauseAfter)
            Pause();
    }

    public static void WriteSuccessMessage(string messageText)
    {
        var currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(messageText);
        Console.ForegroundColor = currentColor;
    }

    public static void WriteException(Exception? ex, string? additionalMessage, bool useConsole, ILogger? logger = null,
        bool pauseAfter = true)
    {
        logger?.LogError(ex, additionalMessage ?? string.Empty);
        if (!useConsole)
            return;
        var existingColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR] ");
        Console.ForegroundColor = existingColor;
        if (!string.IsNullOrWhiteSpace(additionalMessage))
            Console.WriteLine(additionalMessage);
        Console.WriteLine($"{ex?.GetType().Name} thrown with message: {ex?.Message}");
        Console.WriteLine($"Error message is: {ex?.Message}");
        Console.WriteLine($"StackTrace: {ex?.StackTrace}");
        if (pauseAfter)
            Pause();
    }

    public static void WriteException(Exception? ex, bool useConsole, ILogger? logger = null, bool pauseAfter = true)
    {
        WriteException(ex, null, useConsole, logger, pauseAfter);
    }

    public static void LogSerilogFilePath(IConfigurationRoot config)
    {
        var serilogSettings = config.GetSection("Serilog");

        //if (serilogSettings is null)
        //{
        //    Console.WriteLine("Serilog settings not set");
        //    return;
        //}

        var writeToSection = serilogSettings.GetChildren().SingleOrDefault(s => s.Key == "WriteTo");

        if (writeToSection is null)
        {
            Console.WriteLine("Serilog WriteTo Section not set");
            return;
        }

        var writeToWithNameFile = writeToSection.GetChildren().FirstOrDefault(child => child["Name"] == "File");
        if (writeToWithNameFile is null)
        {
            Console.WriteLine("Serilog WriteTo File Section not set");
            return;
        }

        var argsSection = writeToWithNameFile.GetChildren().SingleOrDefault(s => s.Key == "Args");
        if (argsSection is null)
        {
            Console.WriteLine("Serilog WriteTo File Args Section not set");
            return;
        }

        var path = argsSection.GetChildren().SingleOrDefault(s => s.Key == "path");
        if (path is null)
        {
            Console.WriteLine("Serilog WriteTo File Args path not set");
            return;
        }

        Console.WriteLine($"Serilog WriteTo File Path is: {path.Value}");
    }


    public static string? GetMainModulePath()
    {
        // ReSharper disable once using
        using var processModule = Process.GetCurrentProcess().MainModule;
        var pathToExe = processModule?.FileName;
        return pathToExe is not null ? Path.GetDirectoryName(pathToExe) : null;
    }

    public static string? GetMainModuleFileName()
    {
        // ReSharper disable once using
        using var processModule = Process.GetCurrentProcess().MainModule;
        var pathToExe = processModule?.FileName;
        return pathToExe is not null ? Path.GetFileName(pathToExe) : null;
    }
}