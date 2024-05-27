using System;
using System.IO;

namespace ReCounterDom;

public class ReCounterLogger
{
    private readonly LogFile _logFile;

    private int _errorLogId;

    private ReCounterLogger(LogFile logFile)
    {
        _logFile = logFile;
    }

    public static ReCounterLogger? Create(string? logFolderName, string reCounterName)
    {
        var strLogFolder = string.IsNullOrWhiteSpace(logFolderName) ? null : CreateFolder(logFolderName);
        if (string.IsNullOrWhiteSpace(strLogFolder))
            return null;
        //თუ ფოლდერი ვერ შეიქმნა, მაშინ ითვლება, რომ ფაილიც ვერ შეიქმნება
        var logFileName = Path.Combine(strLogFolder, reCounterName + "-logs.txt");
        if (string.IsNullOrWhiteSpace(logFileName))
            return null;
        LogFile logFile = new(logFileName);
        return new ReCounterLogger(logFile);
    }

    public void LogMessage(string message)
    {
        WriteMessage(++_errorLogId + ": " + DateTime.Now + "\t - " + message);
    }

    private void WriteMessage(string message)
    {
        Console.WriteLine(message);
        _logFile.SaveLogToFile(message);
    }


    private static string CreateFolder(string strLogFolderName)
    {
        if (string.IsNullOrWhiteSpace(strLogFolderName))
            return string.Empty;
        try
        {
            DirectoryInfo logFolderDir = new(strLogFolderName);
            //თუ ფოლდერი არ არსებობს შევქმნათ
            if (!logFolderDir.Exists)
                logFolderDir.Create();
            var strLogFolder = logFolderDir.FullName;
            while (strLogFolder.EndsWith(Path.AltDirectorySeparatorChar))
                strLogFolder = strLogFolder[..^1];
            return strLogFolder;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}