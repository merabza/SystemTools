using ApiContracts;
using Microsoft.Extensions.Logging;
using OneOf;
using ReCounterContracts.V1.Routes;
using SystemToolsShared.Errors;

namespace ReCounterContracts;

public class RecounterApiClient : ApiClient
{
    // ReSharper disable once ConvertToPrimaryConstructor
    protected RecounterApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server, string? apiKey,
        bool useConsole) : base(logger, httpClientFactory, server, apiKey,
        new ReCounterMessageHubClient(server, apiKey, string.Empty), useConsole)
    {
    }

    public async Task<OneOf<ProgressData, Err[]>> GetCurrentProcessStatus(CancellationToken cancellationToken)
    {
        return await GetAsyncReturn<ProgressData>(
            RecountMessagesRoutes.ReCounterRoute.Recounter + RecountMessagesRoutes.ReCounterRoute.CurrentProcessStatus,
            false, cancellationToken);
    }

    public async Task<OneOf<bool, Err[]>> CancelCurrentProcess(CancellationToken cancellationToken)
    {
        return await PostAsyncReturn<bool>(
            RecountMessagesRoutes.ReCounterRoute.Recounter + RecountMessagesRoutes.ReCounterRoute.CancelCurrentProcess,
            cancellationToken);
    }
}