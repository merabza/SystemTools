//using FilesProcessing;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
            $@"Time taken {(totalHours == 0 ? "" : $"{totalHours} hours, ")}{(totalMinutes == 0 ? "" : $"{taken.Minutes} minutes, ")}{taken.Seconds} seconds";
    }

    public static string RunProcessWithOutput(bool useConsole, ILogger? logger, string programFileName,
        string arguments)
    {
        var message = $"Running{Environment.NewLine}{programFileName} {arguments}";
        ConsoleWriteInformationLine(message, useConsole, logger);
        if (useConsole)
            Console.WriteLine(message);

        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = programFileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
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

        message = $"output for '{programFileName} {arguments}' is{Environment.NewLine}{sb}";
        ConsoleWriteInformationLine(message, useConsole, logger);
        return sb.ToString();
    }


    public static bool RunProcess(bool useConsole, ILogger? logger, string programFileName, string arguments,
        bool useErrorLine = true, int waitForExit = Timeout.Infinite)
    {
        ConsoleWriteInformationLine($"Running {programFileName} {arguments}...", useConsole, logger);

        var proc = Process.Start(programFileName, arguments);

        if (waitForExit == 0)
            return true;

        ConsoleWriteInformationLine($"Wait For Exit {programFileName}", useConsole, logger);

        proc.WaitForExit(waitForExit < 0 ? Timeout.Infinite : waitForExit);

        ConsoleWriteInformationLine($"{programFileName} finished", useConsole, logger);

        if (proc.ExitCode == 0)
            return true;

        var errorMessage = $"{programFileName} process finished with errors. ExitCode={proc.ExitCode}";
        if (useErrorLine && (useConsole || logger != null))
            WriteErrorLine(errorMessage, useConsole, logger);

        return false;
    }

    public static bool RunCmdProcess(string command, string? projectPath = null)
    {
        var psiNpmRunDist = new ProcessStartInfo
        {
            FileName = "cmd",
            RedirectStandardInput = true,
            WorkingDirectory = projectPath ?? Directory.GetCurrentDirectory()
        };
        var pNpmRunDist = Process.Start(psiNpmRunDist);
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

    public static void ConsoleWriteInformationLine(string text, bool useConsole, ILogger? logger = null)
    {
        logger?.LogInformation(text);
        if (!useConsole)
            return;
        Console.WriteLine(text);
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
        Console.WriteLine($"{ex.GetType().Name} thrown with message: {ex.Message}");
        Console.WriteLine($"Error message is: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
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

        if (serilogSettings == null)
        {
            Console.WriteLine("Serilog settings not set");
            return;
        }

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
}