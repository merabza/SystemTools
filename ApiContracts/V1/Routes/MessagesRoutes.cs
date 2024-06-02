namespace ApiContracts.V1.Routes;

public static class MessagesRoutes
{
    private const string Root = "/api";
    private const string Version = "v1";
    public const string ApiBase = Root + "/" + Version;

    public static class Messages
    {
        public const string MessagesRoute = "/messages";
    }
}