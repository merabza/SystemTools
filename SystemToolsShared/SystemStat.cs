using System.Runtime.InteropServices;

namespace SystemToolsShared;

public static class SystemStat
{
    public static bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
}