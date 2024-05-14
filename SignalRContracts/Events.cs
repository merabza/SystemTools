namespace SignalRContracts;

public static class Events
{
    public static string MessageReceived => nameof(IMessenger.SendMessage);
    public static string ProgressDataReceived => nameof(IProgressDataMessenger.SendProgressData);
}