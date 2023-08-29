namespace SignalRClient;

public static class Events
{
    public static string MessageSent => nameof(IMessenger.SendMessage);
}