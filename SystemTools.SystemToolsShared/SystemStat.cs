using System.Runtime.InteropServices;

namespace SystemTools.SystemToolsShared;

public static class SystemStat
{
    public static bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
}