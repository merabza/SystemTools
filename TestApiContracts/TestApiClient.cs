using ApiContracts;
using Microsoft.Extensions.Logging;
using OneOf;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SystemToolsShared;
using TestApiContracts.V1.Routes;

namespace TestApiContracts;

public class TestApiClient : ApiClient
{
    //public const string ApiName = "TestApi";

    // ReSharper disable once ConvertToPrimaryConstructor
    public TestApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server) : base(logger,
        httpClientFactory, server, null, null, null)
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