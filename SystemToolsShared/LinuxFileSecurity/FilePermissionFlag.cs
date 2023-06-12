namespace SystemToolsShared.LinuxFileSecurity;

public struct FilePermissionFlag
{
    public FileAccess User { get; }
    public FileAccess Group { get; }
    public FileAccess Others { get; }

    internal FilePermissionFlag(FileAccess user, FileAccess group, FileAccess others)
    {
        User = ToValidAccess(user);
        Group = ToValidAccess(group);
        Others = ToValidAccess(others);
    }

    public static FileAccess ToValidAccess(FileAccess access)
    {
        //file access should be 0 to 7
        var num = (int)access;
        return (FileAccess)(num < 0 ? 0 : num > 7 ? 7 : num);
    }
}