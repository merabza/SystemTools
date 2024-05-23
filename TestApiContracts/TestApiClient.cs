using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SystemToolsShared;
using OneOf;
using TestApiContracts.V1.Routes;

namespace TestApiContracts;

public class TestApiClient : ApiClient
{
    //public const string ApiName = "TestApi";

    // ReSharper disable once ConvertToPrimaryConstructor
    public TestApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server) : base(logger,
        httpClientFactory, server, null, null, false)
    {
    }

    public async Task<OneOf<string, Err[]>> GetAppSettingsVersion(CancellationToken cancellationToken)
    {
        return await GetAsyncAsString(TestApiRoutes.Test.TestBase + TestApiRoutes.Test.GetAppSettingsVersion,
            cancellationToken);
    }

    public async Task<OneOf<string, Err[]>> GetVersion(CancellationToken cancellationToken)
    {
        return await GetAsyncAsString(TestApiRoutes.Test.TestBase + TestApiRoutes.Test.GetVersion, cancellationToken);
    }
}