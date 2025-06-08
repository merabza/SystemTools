using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace SystemToolsShared;

public static class FileStat
{
    public static bool IsFileSchema(string fileStoragePath)
    {
        var uriCreated = Uri.TryCreate(fileStoragePath, UriKind.Absolute, out var uri);
        return !uriCreated || uri is null || uri.Scheme.ToLower() == "file";
    }

    public static string NormalizePath(string path)
    {
        if (Uri.TryCreate(path, UriKind.Absolute, out var result))
            if (result.Scheme != "file")
                return result.AbsoluteUri.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        var fullPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? fullPath.ToUpperInvariant() : fullPath;
    }

    public static bool CreatePrevFolderIfNotExists(string fileName, bool useConsole, ILogger? logger = null)
    {
        try
        {
            var sf = new FileInfo(fileName);
            if (sf.DirectoryName is null)
                return false;

            DirectoryInfo destDir = new(sf.DirectoryName);

            if (destDir.Exists)
                return true;
            //ფოლდერი არ არსებობს, უნდა შეიქმნას
            destDir.Create();
            destDir.Refresh();

            return destDir.Exists;
        }
        catch (Exception e)
        {
            StShared.WriteException(e, useConsole, logger);
            return false;
        }
    }

    public static string? CreateFolderIfNotExists(string folderName, bool useConsole, ILogger? logger = null)
    {
        try
        {
            DirectoryInfo destDir = new(folderName);

            if (destDir.Exists)
                return destDir.FullName;
            //ფოლდერი არ არსებობს, უნდა შეიქმნას
            destDir.Create();
            destDir.Refresh();

            return destDir.Exists ? destDir.FullName : null;
        }
        catch (Exception e)
        {
            StShared.WriteException(e, useConsole, logger);
            return null;
        }
    }

    public static string RemoveNotNeedLeadPart(this string dest, char removeLead)
    {
        return dest.StartsWith(removeLead) ? dest[1..] : dest;
    }

    public static string RemoveNotNeedLastPart(this string dest, char removeLast)
    {
        return dest.EndsWith(removeLast) ? dest[..^1] : dest;
    }

    public static string AddNeedLeadPart(this string dest, string mustLead)
    {
        if (dest.StartsWith(mustLead))
            return dest;
        return mustLead + dest;
    }

    public static string AddNeedLastPart(this string dest, string mustLast)
    {
        if (dest.EndsWith(mustLast))
            return dest;
        return dest + mustLast;
    }

    public static string AddNeedLastPart(this string dest, char mustLast)
    {
        if (dest.EndsWith(mustLast))
            return dest;
        return dest + mustLast;
    }

    public static bool FitsMask(this string sFileName, string sFileMask)
    {
        if (string.IsNullOrWhiteSpace(sFileMask))
            return true;
        var regexFileMask = sFileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", ".").Replace("\\", @"\\");
        if (!sFileMask.EndsWith('*'))
            regexFileMask += '$';
        if (!sFileMask.StartsWith('*'))
            regexFileMask = '^' + regexFileMask;
        Regex mask = new(regexFileMask);
        var toRet = mask.IsMatch(sFileName);
        return toRet;
    }

    //string maskFirstVersion = "yyyyMMddHHmmssfffffff";
    public static (DateTime, string?) GetDateTimeAndPatternByDigits(this string fileName, string maskFirstVersion)
    {
        StringBuilder sbMask = new();
        var position = 0;
        var maskPosition = 0;
        var maskPositionInName = 0;
        foreach (var c in fileName)
        {
            if (char.IsDigit(c))
            {
                if (maskPosition == 0)
                    maskPositionInName = position;
                sbMask.Append(maskFirstVersion[maskPosition]);
                maskPosition++;
                if (maskPosition == maskFirstVersion.Length)
                    break;
            }
            else if (c is '-' or '_' && maskPosition > 0 &&
                     maskFirstVersion[maskPosition] != maskFirstVersion[maskPosition - 1])
            {
                sbMask.Append(c);
            }
            else if (maskPosition > 7)
            {
                break;
            }
            else if (maskPosition > 0)
            {
                maskPosition = 0;
                sbMask.Clear();
            }

            position++;
        }

        //მინიმუმ 8 პოზიცია არის წელიწადი, თვე და დღე
        if (maskPosition < 8)
            return (DateTime.MinValue, null);
        var mask = sbMask.ToString();
        var strDate = fileName.Substring(maskPositionInName, mask.Length);
        var pattern = fileName[..maskPositionInName] + mask + fileName[(maskPositionInName + mask.Length)..];

        var dt = TryGetDate(strDate, mask);
        return dt == DateTime.MinValue ? (DateTime.MinValue, null) : (dt, pattern);
    }

    private static DateTime TryGetDate(string strDate, string mask)
    {
        try
        {
            return DateTime.ParseExact(strDate, mask, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            //Console.WriteLine(e);
        }

        return DateTime.MinValue;
    }

    // This method accepts two strings the represent two files to
    // compare. A return value of true indicates that the contents of the files
    // are the same. A return value of any other value indicates that the
    // files are not the same.
    public static bool FileCompare(string file1, string file2)
    {
        //_consoleFormatter.WriteInSameLine($"Compare Files file1={file1} file2={file2}");
        Console.WriteLine($"Compare Files file1={file1} file2={file2}");

        int file1Byte;
        int file2Byte;

        // Determine if the same file was referenced two times.
        if (file1 == file2)
            // Return true to indicate that the files are the same.
            return true;

        // Open the two files.
        // ReSharper disable once using
        using FileStream fs1 = new(file1, FileMode.Open);
        // ReSharper disable once using
        using FileStream fs2 = new(file2, FileMode.Open);

        // Check the file sizes. If they are not the same, the files
        // are not the same.
        if (fs1.Length != fs2.Length)
        {
            // Close the file
            fs1.Close();
            fs2.Close();

            // Return false to indicate files are different
            return false;
        }

        // Read and compare a byte from each file until either a
        // non-matching set of bytes is found or until the end of
        // file1 is reached.
        do
        {
            // Read one byte from each file.
            file1Byte = fs1.ReadByte();
            file2Byte = fs2.ReadByte();
        } while (file1Byte == file2Byte && file1Byte != -1);

        // Close the files.
        fs1.Close();
        fs2.Close();

        // Return the success of the comparison. "file1Byte" is
        // equal to "file2Byte" at this point only if the files are
        // the same.
        return file1Byte - file2Byte == 0;
    }

    public static void DeleteDirectoryWithNormaliseAttributes(string targetDir)
    {
        Console.WriteLine($"Deleting {targetDir} ...");
        var files = Directory.GetFiles(targetDir);
        var dirs = Directory.GetDirectories(targetDir);

        foreach (var file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (var dir in dirs) 
            DeleteDirectoryWithNormaliseAttributes(dir);

        Directory.Delete(targetDir, false);
        Console.WriteLine($"Deleted {targetDir}");
    }

    
    public static void DeleteDirectoryIfExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) 
            return;

        Console.WriteLine($"Deleting {directoryPath} ...");
        Directory.Delete(directoryPath, true);
        Console.WriteLine($"Deleted {directoryPath}");
    }



    public static void ClearFolder(string targetFolder, string[] excludes)
    {
        Console.WriteLine($"Clearing {targetFolder} ...");
        var files = Directory.GetFiles(targetFolder);
        var dirs = Directory.GetDirectories(targetFolder);

        foreach (var file in files)
        {
            if (excludes.Any(exclude => file.Contains(exclude)))
                continue;
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (var dir in dirs)
        {
            if (excludes.Any(exclude => dir.Contains(exclude)))
                continue;
            DeleteDirectoryWithNormaliseAttributes(dir);
        }

        Console.WriteLine($"Cleared {targetFolder}");
    }

    public static bool CopyFilesAndFolders(string sourceFolderPath, string destinationFolderPath, string[] excludes,
        bool useConsole, ILogger? logger = null)
    {
        Console.WriteLine($"Copying files from {sourceFolderPath} to {destinationFolderPath}...");

        if (!Directory.Exists(sourceFolderPath))
        {
            StShared.WriteErrorLine($"source folder {destinationFolderPath} does not exists", useConsole, logger);
            return false;
        }

        var appDiffPath = CreateFolderIfNotExists(destinationFolderPath, true);
        if (appDiffPath is null)
        {
            StShared.WriteErrorLine($"does not exists and cannot be creates destination folder {destinationFolderPath}",
                useConsole, logger);
            return false;
        }

        var sourceDirInfo = new DirectoryInfo(sourceFolderPath);
        var files = sourceDirInfo.GetFiles();
        var dirs = sourceDirInfo.GetDirectories();

        foreach (var file in files)
        {
            if (excludes.Any(exclude => file.FullName.Contains(exclude)))
                continue;
            File.Copy(file.FullName, Path.Combine(destinationFolderPath, file.Name));
        }

        foreach (var dir in dirs)
        {
            if (excludes.Any(exclude => dir.FullName.Contains(exclude)))
                continue;
            CopyFilesAndFolders(Path.Combine(sourceFolderPath, dir.Name), Path.Combine(destinationFolderPath, dir.Name),
                excludes, useConsole, logger);
        }

        Console.WriteLine("Coping files Finished");

        return true;
    }
}