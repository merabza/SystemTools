using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OneOf;
using ReCounterContracts;
using ReCounterContracts.V1.Routes;
using SystemTools.ApiContracts;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.ReCounterContracts;

public /*open*/ class ReCounterApiClient : ApiClient
{
    // ReSharper disable once ConvertToPrimaryConstructor
    protected ReCounterApiClient(ILogger logger, IHttpClientFactory httpClientFactory,
        ReCounterMessageHubClient messageHubClient, string server, string? apiKey, bool useConsole) : base(logger,
        httpClientFactory, server, apiKey, messageHubClient, useConsole)
    {
    }

    public Task<OneOf<ProgressData, Err[]>> GetCurrentProcessStatus(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<ProgressData>(
            RecountMessagesRoutes.ReCounterRoute.Recounter + RecountMessagesRoutes.ReCounterRoute.CurrentProcessStatus,
            false, cancellationToken);
    }

    public Task<OneOf<bool, Err[]>> CancelCurrentProcess(CancellationToken cancellationToken = default)
    {
        return PostAsyncReturn<bool>(
            RecountMessagesRoutes.ReCounterRoute.Recounter + RecountMessagesRoutes.ReCounterRoute.CancelCurrentProcess,
            cancellationToken);
    }
}
