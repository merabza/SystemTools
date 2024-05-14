//using FilesProcessing;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneOf;
using SystemToolsShared.ErrorModels;

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
            $"Time taken {(totalHours == 0 ? "" : $"{totalHours} hours, ")}{(totalMinutes == 0 ? "" : $"{taken.Minutes} minutes, ")}{taken.Seconds} seconds";
    }

    public static OneOf<(string, int), Err[]> RunProcessWithOutput(bool useConsole, ILogger? logger,
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
        if (useConsole || logger != null)
            WriteErrorLine(errorMessage, useConsole, logger);

        return new[] { SystemToolsErrors.RunProcessError(errorMessage) };
    }


    private static bool IsAllowExitCode(int exitCode, int[]? allowExitCodes)
    {
        if (exitCode == 0)
            return true;
        return allowExitCodes is not null && allowExitCodes.Contains(exitCode);
    }


    public static Option<Err[]> RunProcess(bool useConsole, ILogger? logger, string programFileName, string arguments,
        int[]? allowExitCodes = null, bool useErrorLine = true, int waitForExit = Timeout.Infinite)
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
        if (useErrorLine && (useConsole || logger != null))
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
        if (pNpmRunDist == null)
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
        Console.ReadKey(false);
    }

    public static void ConsoleWriteInformationLine(ILogger? logger, bool useConsole, string message,
        params object?[] args)
    {
        logger?.LogInformation(message, args);
        if (!useConsole)
            return;
        Console.WriteLine(message, args);
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

    public static void WriteException(Exception? ex, string? additionalMessage, bool useConsole,
        ILogger? logger = null, bool pauseAfter = true)
    {
        logger?.LogError(ex, additionalMessage ?? "");
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

        //if (serilogSettings == null)
        //{
        //    Console.WriteLine("Serilog settings not set");
        //    return;
        //}

        var writeToSection =
            serilogSettings.GetChildren().SingleOrDefault(s => s.Key == "WriteTo");

        if (writeToSection == null)
        {
            Console.WriteLine("Serilog WriteTo Section not set");
            return;
        }

        var writeToWithNameFile =
            writeToSection.GetChildren().FirstOrDefault(child => child["Name"] == "File");
        if (writeToWithNameFile == null)
        {
            Console.WriteLine("Serilog WriteTo File Section not set");
            return;
        }

        var argsSection = writeToWithNameFile.GetChildren().SingleOrDefault(s => s.Key == "Args");
        if (argsSection == null)
        {
            Console.WriteLine("Serilog WriteTo File Args Section not set");
            return;
        }

        var path = argsSection.GetChildren().SingleOrDefault(s => s.Key == "path");
        if (path == null)
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
        return pathToExe != null ? Path.GetDirectoryName(pathToExe) : null;
    }

    public static string? GetMainModuleFileName()
    {
        // ReSharper disable once using
        using var processModule = Process.GetCurrentProcess().MainModule;
        var pathToExe = processModule?.FileName;
        return pathToExe != null ? Path.GetFileName(pathToExe) : null;
    }



}