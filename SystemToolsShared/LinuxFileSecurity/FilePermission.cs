using System;
using System.Diagnostics;
using System.IO;

namespace SystemToolsShared.LinuxFileSecurity;

public sealed class FilePermission
{
    private const string Command = "chmod";

    public FilePermission(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileLoadException("error loading " + filePath, filePath);
        FilePath = filePath;
    }

    public string FilePath { get; }

    public FilePermissionFlag Flags { get; private set; }

    public override string ToString()
    {
        return
            $"{(int)FilePermissionFlag.ToValidAccess(Flags.User)}{(int)FilePermissionFlag.ToValidAccess(Flags.Group)}{(int)FilePermissionFlag.ToValidAccess(Flags.Others)} {FilePath}";
    }

    //public FilePermission(FilePermission permission, FileAccess user, FileAccess group, FileAccess others)
    //    : this(permission.Process, user, group, others)
    //{
    //}

    public static FilePermission? SetPermission(string filePath, FileAccess user, FileAccess group,
        FileAccess others)
    {
        var permission = new FilePermission(filePath)
        { Flags = new FilePermissionFlag(user, group, others) };
        return permission.Apply();
    }

    public void Apply(FileAccess user, FileAccess group, FileAccess others)
    {
        Flags = new FilePermissionFlag(user, group, others);
        Apply();
    }

    private FilePermission? Apply()
    {
        if (string.IsNullOrWhiteSpace(FilePath) || string.IsNullOrWhiteSpace(FilePath))
        {
            Console.WriteLine("File name cannot be empty.");
            return null;
        }

        if (!File.Exists(FilePath))
        {
            Console.WriteLine("Error finding file " + FilePath + ".");
            return null;
        }

        try
        {
            var permission = ToString();
            var filePermission =
                new ProcessStartInfo(Command, permission);
            Process.Start(filePermission);
            return this;
        }
        catch
        {
            return null;
        }
    }
}