namespace SystemToolsShared.LinuxFileSecurity;

public static class KnownAccess
{
    public static readonly LinuxFileAccess None = LinuxFileAccess.None;
    public static readonly LinuxFileAccess Read = LinuxFileAccess.Read;
    public static readonly LinuxFileAccess Write = LinuxFileAccess.Write;
    public static readonly LinuxFileAccess Execute = LinuxFileAccess.Execute;
    public static readonly LinuxFileAccess ReadWrite = LinuxFileAccess.Read | LinuxFileAccess.Write;
    public static readonly LinuxFileAccess ReadExecute = LinuxFileAccess.Read | LinuxFileAccess.Execute;
    public static readonly LinuxFileAccess WriteExecute = LinuxFileAccess.Write | LinuxFileAccess.Execute;

    public static readonly LinuxFileAccess ReadWriteExecute =
        LinuxFileAccess.Read | LinuxFileAccess.Write | LinuxFileAccess.Execute;
}