namespace SystemTools.StringMessagesApiContracts;

public static class StringEvents
{
    public static string MessageReceived => nameof(IMessenger.SendMessage);
    //public static string ProgressDataReceived => nameof(IProgressDataMessenger.SendProgressData);
}