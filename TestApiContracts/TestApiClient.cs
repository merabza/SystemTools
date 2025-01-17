using ApiContracts;
using Microsoft.Extensions.Logging;
using OneOf;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SystemToolsShared.Errors;
using TestApiContracts.V1.Routes;

namespace TestApiContracts;

public class TestApiClient : ApiClient
{
    //public const string ApiName = "TestApi";

    // ReSharper disable once ConvertToPrimaryConstructor
    public TestApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server, bool useConsole) : base(
        logger, httpClientFactory, server, null, null, useConsole)
    {
    }

    public Task<OneOf<string, IEnumerable<Err>>> GetAppSettingsVersion(CancellationToken cancellationToken = default)
    {
        return GetAsyncAsString(TestApiRoutes.Test.TestBase + TestApiRoutes.Test.GetAppSettingsVersion,
            cancellationToken);
    }

    public Task<OneOf<string, IEnumerable<Err>>> GetVersion(CancellationToken cancellationToken = default)
    {
        return GetAsyncAsString(TestApiRoutes.Test.TestBase + TestApiRoutes.Test.GetVersion, cancellationToken);
    }
}