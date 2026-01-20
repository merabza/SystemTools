using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OneOf;
using SystemTools.ApiContracts;
using SystemTools.SystemToolsShared.Errors;
using SystemTools.TestApiContracts.V1.Routes;

namespace SystemTools.TestApiContracts;

public sealed class TestApiClient : ApiClient
{
    //public const string ApiName = "TestApi";

    // ReSharper disable once ConvertToPrimaryConstructor
    public TestApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server, bool useConsole) : base(
        logger, httpClientFactory, server, null, null, useConsole)
    {
    }

    public Task<OneOf<string, Err[]>> GetAppSettingsVersion(CancellationToken cancellationToken = default)
    {
        return GetAsyncAsString(TestApiRoutes.Test.TestBase + TestApiRoutes.Test.GetAppSettingsVersion,
            cancellationToken);
    }

    public Task<OneOf<string, Err[]>> GetVersion(CancellationToken cancellationToken = default)
    {
        return GetAsyncAsString(TestApiRoutes.Test.TestBase + TestApiRoutes.Test.GetVersion, cancellationToken);
    }
}