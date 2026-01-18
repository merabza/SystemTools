namespace SystemTools.StringMessagesApiContracts.V1.Routes;

public static class MessagesRoutes
{
    private const string Api = "api";
    private const string Version = "v1";
    public const string ApiBase = Api + "/" + Version;

    public static class Messages
    {
        public const string MessagesRoute = "/messages";
    }
}