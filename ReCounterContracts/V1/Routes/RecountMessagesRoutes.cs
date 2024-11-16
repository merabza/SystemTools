namespace ReCounterContracts.V1.Routes;

public static class RecountMessagesRoutes
{
    private const string Api = "api";
    private const string Version = "v1";
    public const string ApiBase = Api + "/" + Version;

    public static class ReCounterRoute
    {
        //api/v1/recounter
        public const string Recounter = "/recounter";

        //HUB api/v1/recounter/messages
        public const string Messages = "/messages";

        // POST api/v1/recounter/cancelcurrentprocess
        public const string CancelCurrentProcess = "/cancelcurrentprocess";

        // POST api/v1/recounter/currentprocessstatus
        public const string CurrentProcessStatus = "/currentprocessstatus";

        // POST api/v1/recounter/isprocessrunning
        public const string IsProcessRunning = "/isprocessrunning";
    }
}