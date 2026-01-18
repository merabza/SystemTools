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
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.SystemToolsShared;

public static class StShared
{
    public static string TimeTakenMessage(DateTime startDateTime)
    {
        DateTime endDateTime = DateTime.Now; //პროცესის დასრულების დრო
        TimeSpan taken = endDateTime - startDateTime; //დავიანგარიშოთ რა დრო დასჭირდა მთლიანად პროცესს
        int totalHours = (int)taken.TotalHours; //საათები თუ არ დასჭირდა რომ არ გამოვიტანოთ
        int totalMinutes = (int)taken.TotalMinutes; //საათები თუ არ დასჭირდა რომ არ გამოვიტანოთ
        //დახარჯული დროის შესახებ ინფორმაციის გამოტანა ფორმაზე.
        return
            $"Time taken {(totalHours == 0 ? string.Empty : $"{totalHours} hours, ")}{(totalMinutes == 0 ? string.Empty : $"{taken.Minutes} minutes, ")}{taken.Seconds} seconds";
    }

    public static OneOf<(string, int), Err[]> RunProcessWithOutput(bool useConsole, ILogger? logger,
        string programFileName, string arguments, int[]? allowExitCodes = null)
    {
        //var option = CheckFileExists(programFileName);
        //if (option.IsSome) 
        //    return (Err[])option;

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

        var sb = new StringBuilder();
        proc.Start();
        while (!proc.StandardOutput.EndOfStream)
        {
            string? line = proc.StandardOutput.ReadLine();
            if (useConsole)
            {
                Console.WriteLine(line);
            }

            sb.AppendLine(line);
        }
        //message = "output for '{0} {1}' is{2}{3}";

        if (IsAllowExitCode(proc.ExitCode, allowExitCodes))
        {
            ConsoleWriteInformationLine(logger, useConsole, "output for '{0} {1}' is{2}{3}", programFileName, arguments,
                Environment.NewLine, sb);
            return (sb.ToString(), proc.ExitCode);
        }

        string errorMessage =
            $"{programFileName} {arguments} process was finished with errors. ExitCode={proc.ExitCode}";
        if (useConsole || logger is not null)
        {
            WriteErrorLine(errorMessage, useConsole, logger);
        }

        return new[] { SystemToolsErrors.RunProcessError(errorMessage) };
    }

    private static bool IsAllowExitCode(int exitCode, int[]? allowExitCodes)
    {
        if (exitCode == 0)
        {
            return true;
        }

        return allowExitCodes is not null && allowExitCodes.Contains(exitCode);
    }

    public static Option<Err[]> RunProcess(bool useConsole, ILogger? logger, string programFileName, string arguments,
        int[]? allowExitCodes = null, bool useErrorLine = true, int waitForExit = Timeout.Infinite)
    {
        ConsoleWriteInformationLine(logger, useConsole, "Running {0} {1}...", programFileName, arguments);

        //var option = CheckFileExists(programFileName);
        //if (option.IsSome) 
        //    return option;

        // ReSharper disable once using
        using Process proc = Process.Start(programFileName, arguments);

        if (waitForExit == 0)
        {
            return null;
        }

        ConsoleWriteInformationLine(logger, useConsole, "Wait For Exit {0}", programFileName);

        proc.WaitForExit(waitForExit < 0 ? Timeout.Infinite : waitForExit);

        ConsoleWriteInformationLine(logger, useConsole, "{0} finished", programFileName);

        if (IsAllowExitCode(proc.ExitCode, allowExitCodes))
        {
            return null;
        }

        string errorMessage =
            $"{programFileName} {arguments} process was finished with errors. ExitCode={proc.ExitCode}";
        if (useErrorLine && (useConsole || logger is not null))
        {
            WriteErrorLine(errorMessage, useConsole, logger);
        }

        return new[] { SystemToolsErrors.RunProcessError(errorMessage) };
    }

    //private static Option<Err[]> CheckFileExists(string programFileName)
    //{
    //    // Check if the program file exists before starting the process
    //    if (!File.Exists(programFileName))
    //    {
    //        var errorMsg = $"File not found: {programFileName}";
    //        return Err.CreateArr(new Err { ErrorCode = "FileNotFound", ErrorMessage = errorMsg });
    //    }

    //    //also check if the file exists in the current directory
    //    if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), programFileName)))
    //    {
    //        var errorMsg = $"File not found in current directory: {programFileName}";
    //        return Err.CreateArr(new Err { ErrorCode = "FileNotFound", ErrorMessage = errorMsg });
    //    }
    //    return null;
    //}

    public static bool RunCmdProcess(string command, string? projectPath = null)
    {
        var psiNpmRunDist = new ProcessStartInfo
        {
            FileName = "cmd",
            RedirectStandardInput = true,
            WorkingDirectory = projectPath ?? Directory.GetCurrentDirectory()
        };
        // ReSharper disable once using
        using Process? pNpmRunDist = Process.Start(psiNpmRunDist);
        if (pNpmRunDist is null)
        {
            return false;
        }

        pNpmRunDist.StandardInput.WriteLine($"{command} & exit");
        pNpmRunDist.WaitForExit();

        return true;
    }

    public static bool CreateFolder(string path, bool useConsole)
    {
        string? checkedPath = FileStat.CreateFolderIfNotExists(path, useConsole);

        if (checkedPath is not null)
        {
            return true;
        }

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
#pragma warning disable CA2254 // Template should be a static expression
        logger?.LogInformation(message, args);
#pragma warning restore CA2254 // Template should be a static expression
        if (!useConsole)
        {
            return;
        }

        ConsoleWriteFormattedLine(message, args);
    }

    private static void ConsoleWriteFormattedLine(string message, params object?[] args)
    {
        //var vsb = new StringBuilder(256);
        int scanIndex = 0;
        int endIndex = message.Length;
        int argIndex = 0;

        while (scanIndex < endIndex)
        {
            int openBraceIndex = FindBraceIndex(message, '{', scanIndex, endIndex);
            if (scanIndex == 0 && openBraceIndex == endIndex)
            {
                // No holes found.
                Console.WriteLine(message);
                return;
            }

            int closeBraceIndex = FindBraceIndex(message, '}', openBraceIndex, endIndex);

            if (closeBraceIndex == endIndex)
            {
                Console.Write(message[scanIndex..endIndex]);
                scanIndex = endIndex;
            }
            else
            {
                if (openBraceIndex > scanIndex)
                {
                    Console.Write(message[scanIndex..openBraceIndex]);
                }

                ConsoleColor existingColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Cyan;
                if (argIndex < args.Length)
                {
                    Console.Write(args[argIndex++]);
                }
                else
                {
                    Console.Write(message.AsSpan(openBraceIndex + 1, closeBraceIndex - 1)); //value
                }

                Console.ForegroundColor = existingColor;

                scanIndex = closeBraceIndex + 1;
            }
        }

        Console.WriteLine();
    }

    private static int FindBraceIndex(string format, char brace, int startIndex, int endIndex)
    {
        // Example: {{prefix{{{Argument}}}suffix}}.
        int braceIndex = endIndex;
        int scanIndex = startIndex;
        int braceOccurrenceCount = 0;

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
                    {
                        braceIndex = scanIndex;
                    }
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
#pragma warning disable CA2254 // Template should be a static expression
        logger?.LogWarning(warningText);
#pragma warning restore CA2254 // Template should be a static expression
        if (!useConsole)
        {
            return;
        }

        ConsoleColor existingColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[warning] ");
        Console.ForegroundColor = existingColor;
        Console.WriteLine(warningText);
        if (pauseAfter)
        {
            Pause();
        }
    }

    public static void WriteErrorLine(string errorText, bool useConsole, ILogger? logger = null, bool pauseAfter = true)
    {
#pragma warning disable CA2254
        logger?.LogError(errorText);
#pragma warning restore CA2254
        if (!useConsole)
        {
            return;
        }

        ConsoleColor existingColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR] ");
        Console.ForegroundColor = existingColor;
        Console.WriteLine(errorText);
        if (pauseAfter)
        {
            Pause();
        }
    }

    public static void WriteSuccessMessage(string messageText)
    {
        ConsoleColor currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(messageText);
        Console.ForegroundColor = currentColor;
    }

    public static void WriteException(Exception? ex, string? additionalMessage, bool useConsole, ILogger? logger = null,
        bool pauseAfter = true)
    {
#pragma warning disable CA2254
        logger?.LogError(ex, additionalMessage ?? string.Empty);
#pragma warning restore CA2254
        if (!useConsole)
        {
            return;
        }

        ConsoleColor existingColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR] ");
        Console.ForegroundColor = existingColor;
        if (!string.IsNullOrWhiteSpace(additionalMessage))
        {
            Console.WriteLine(additionalMessage);
        }

        Console.WriteLine($"{ex?.GetType().Name} thrown with message: {ex?.Message}");
        Console.WriteLine($"Error message is: {ex?.Message}");
        Console.WriteLine($"StackTrace: {ex?.StackTrace}");
        if (pauseAfter)
        {
            Pause();
        }
    }

    public static void WriteException(Exception? ex, bool useConsole, ILogger? logger = null, bool pauseAfter = true)
    {
        WriteException(ex, null, useConsole, logger, pauseAfter);
    }

    public static void LogSerilogFilePath(IConfigurationRoot config)
    {
        IConfigurationSection serilogSettings = config.GetSection("Serilog");

        //if (serilogSettings is null)
        //{
        //    Console.WriteLine("Serilog settings not set");
        //    return;
        //}

        IConfigurationSection? writeToSection = serilogSettings.GetChildren().SingleOrDefault(s => s.Key == "WriteTo");

        if (writeToSection is null)
        {
            Console.WriteLine("Serilog WriteTo Section not set");
            return;
        }

        IConfigurationSection? writeToWithNameFile =
            writeToSection.GetChildren().FirstOrDefault(child => child["Name"] == "File");
        if (writeToWithNameFile is null)
        {
            Console.WriteLine("Serilog WriteTo File Section not set");
            return;
        }

        IConfigurationSection? argsSection = writeToWithNameFile.GetChildren().SingleOrDefault(s => s.Key == "Args");
        if (argsSection is null)
        {
            Console.WriteLine("Serilog WriteTo File Args Section not set");
            return;
        }

        IConfigurationSection? path = argsSection.GetChildren().SingleOrDefault(s => s.Key == "path");
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
        using ProcessModule? processModule = Process.GetCurrentProcess().MainModule;
        string? pathToExe = processModule?.FileName;
        return pathToExe is not null ? Path.GetDirectoryName(pathToExe) : null;
    }

    public static string? GetMainModuleFileName()
    {
        // ReSharper disable once using
        using ProcessModule? processModule = Process.GetCurrentProcess().MainModule;
        string? pathToExe = processModule?.FileName;
        return pathToExe is not null ? Path.GetFileName(pathToExe) : null;
    }
}
