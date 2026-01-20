namespace SystemTools.TestApiContracts.V1.Routes;

public static class TestApiRoutes
{
    private const string Api = "api";
    private const string Version = "v1";
    public const string ApiBase = Api + "/" + Version;

    public static class Test
    {
        public const string TestBase = "/test";

        // GET api/v1/test/testconnection
        public const string TestConnection = "/testconnection";

        // GET api/v1/test/getip
        public const string GetIp = "/getip";

        // GET api/v1/test/getversion
        public const string GetVersion = "/getversion";

        // GET api/v1/test/getappsettingsversion
        public const string GetAppSettingsVersion = "/getappsettingsversion";

        // GET api/v1/test/getsettings        
        public const string GetSettings = "/getsettings";
    }
}