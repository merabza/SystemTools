using System;

namespace SystemToolsShared.LinuxFileSecurity;

[Flags]
public enum LinuxFileAccess
{
    None = 0,
    Execute = 1,
    Write = 2,
    Read = 4
}