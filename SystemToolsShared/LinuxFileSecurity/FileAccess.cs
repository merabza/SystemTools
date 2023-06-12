using System;

namespace SystemToolsShared.LinuxFileSecurity;

[Flags]
public enum FileAccess
{
    None = 0,
    Execute = 1,
    Write = 2,
    Read = 4
}