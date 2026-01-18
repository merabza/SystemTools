using System;

namespace SystemTools.SystemToolsShared.LinuxFileSecurity;

public readonly struct FilePermissionFlag : IEquatable<FilePermissionFlag>
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
        int num = (int)access;
        num = num switch
        {
            < 0 => 0,
            > 7 => 7,
            _ => num
        };
        return (LinuxFileAccess)num;
    }

    public override bool Equals(object? obj)
    {
        return obj is FilePermissionFlag other && User == other.User && Group == other.Group && Others == other.Others;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(User, Group, Others);
    }

    public static bool operator ==(FilePermissionFlag left, FilePermissionFlag right) => left.Equals(right);

    public static bool operator !=(FilePermissionFlag left, FilePermissionFlag right) => !(left == right);

    public bool Equals(FilePermissionFlag other)
    {
        return User.Equals(other.User) && Group.Equals(other.Group) && Others.Equals(other.Others);
    }
}
