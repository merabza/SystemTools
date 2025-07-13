namespace SystemToolsShared.LinuxFileSecurity;

public struct FilePermissionFlag
{
    public LinuxFileAccess User { get; }
    public LinuxFileAccess Group { get; }
    public LinuxFileAccess Others { get; }

    internal FilePermissionFlag(LinuxFileAccess user, LinuxFileAccess group, LinuxFileAccess others)
    {
        User = ToValidAccess(user);
        Group = ToValidAccess(group);
        Others = ToValidAccess(others);
    }

    public static LinuxFileAccess ToValidAccess(LinuxFileAccess access)
    {
        //file access should be 0 to 7
        var num = (int)access;
        return (LinuxFileAccess)(num < 0 ? 0 : num > 7 ? 7 : num);
    }
}