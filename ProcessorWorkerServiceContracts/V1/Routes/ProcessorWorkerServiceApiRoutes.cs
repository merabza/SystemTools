namespace ProcessorWorkerServiceContracts.V1.Routes;

public static class ProcessorWorkerServiceApiRoutes
{
    private const string Api = "api";
    private const string Version = "v1";
    public const string ApiBase = Api + "/" + Version;

    public static class ReCounterRoute
    {
        public const string ReCounterBase = "/recounter";

        // POST api/v1/recounter/clearredundantlemmas
        public const string ClearRedundantLemmas = "/clearredundantlemmas";

        // POST api/v1/recounter/clearredundantwords
        public const string ClearRedundantWords = "/clearredundantwords";

        // POST api/v1/recounter/clearwordsbylemmas
        public const string ClearWordsByLemmas = "/clearwordsbylemmas";

        // POST api/v1/recounter/clearrawwordsbylemmas
        public const string ClearRawWordsByLemmas = "/clearrawwordsbylemmas";

        // POST api/v1/recounter/generatewordsbylemmaslist
        public const string GenerateWordsByLemmasList = "/generatewordsbylemmaslist";

        // GET api/v1/recounter/iswordsbylemmasclear
        public const string IsWordsByLemmasClear = "/iswordsbylemmasclear";
    }
}