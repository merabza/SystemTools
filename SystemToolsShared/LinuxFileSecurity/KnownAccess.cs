namespace SystemToolsShared.LinuxFileSecurity;

public static class KnownAccess
{
    public static readonly FileAccess None = FileAccess.None;
    public static readonly FileAccess Read = FileAccess.Read;
    public static readonly FileAccess Write = FileAccess.Write;
    public static readonly FileAccess Execute = FileAccess.Execute;
    public static readonly FileAccess ReadWrite = FileAccess.Read | FileAccess.Write;
    public static readonly FileAccess ReadExecute = FileAccess.Read | FileAccess.Execute;
    public static readonly FileAccess WriteExecute = FileAccess.Write | FileAccess.Execute;
    public static readonly FileAccess ReadWriteExecute = FileAccess.Read | FileAccess.Write | FileAccess.Execute;
}