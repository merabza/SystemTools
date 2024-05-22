namespace TestApiContracts.V1.Routes;

public static class TestApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Base = Root + "/" + Version;

    public static class Test
    {
        private const string TestBase = Base + "/test";

        // GET api/v1/test/testconnection
        public const string TestConnection = TestBase + "/testconnection";

        // GET api/v1/test/getip
        public const string GetIp = TestBase + "/getip";

        // GET api/v1/test/getversion
        public const string GetVersion = TestBase + "/getversion";

        // GET api/v1/test/getappsettingsversion
        public const string GetAppSettingsVersion = TestBase + "/getappsettingsversion";

        // GET api/v1/test/getsettings        
        public const string GetSettings = TestBase + "/getsettings";
    }
}