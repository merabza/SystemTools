using ApiContracts;
using Microsoft.Extensions.Logging;
using OneOf;
using ReCounterContracts.V1.Routes;
using SystemToolsShared.Errors;

namespace ReCounterContracts;

public class RecounterApiClient : ApiClient
{
    private readonly ReCounterMessageHubClient _messageHubClient;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected RecounterApiClient(ILogger logger, IHttpClientFactory httpClientFactory,
        ReCounterMessageHubClient messageHubClient, string server, string? apiKey, bool useConsole) : base(logger,
        httpClientFactory, server, apiKey, messageHubClient, useConsole)
    {
        _messageHubClient = messageHubClient;
        //new ReCounterMessageHubClient(server, apiKey)
    }

    public bool IsProcessRunning()
    {
        return MessageHubClient is not null && _messageHubClient.IsProcessRunning;
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