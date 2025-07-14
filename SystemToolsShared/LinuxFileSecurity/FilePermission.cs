using System;
using System.Diagnostics;
using System.IO;

namespace SystemToolsShared.LinuxFileSecurity;

public sealed class FilePermission
{
    private const string Command = "chmod";
    public string FilePath { get; }
    public FilePermissionFlag Flags { get; private set; }

    // ReSharper disable once ConvertToPrimaryConstructor
    public FilePermission(string filePath)
    {
        FilePath = filePath;
    }

    public static FilePermission Create(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileLoadException("error loading " + filePath, filePath);
        return new FilePermission(filePath);
    }

    public override string ToString()
    {
        return
            $"{(int)FilePermissionFlag.ToValidAccess(Flags.User)}{(int)FilePermissionFlag.ToValidAccess(Flags.Group)}{(int)FilePermissionFlag.ToValidAccess(Flags.Others)} {FilePath}";
    }

    public static FilePermission? SetPermission(string filePath, LinuxFileAccess user, LinuxFileAccess group,
        LinuxFileAccess others)
    {
        var permission = new FilePermission(filePath) { Flags = new FilePermissionFlag(user, group, others) };
        return permission.Apply();
    }

    public void Apply(LinuxFileAccess user, LinuxFileAccess group, LinuxFileAccess others)
    {
        Flags = new FilePermissionFlag(user, group, others);
        Apply();
    }

    public FilePermission? Apply()
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
            var filePermission = new ProcessStartInfo(Command, permission);
            Process.Start(filePermission);
            return this;
        }
        catch
        {
            return null;
        }
    }
}