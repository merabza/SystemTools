namespace SignalRContracts.V1.Routes;

public static class MessagesRoutes
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Base = Root + "/" + Version;

    public static class Messages
    {
        public const string MessagesRoute = Base + "/messages";
    }
}