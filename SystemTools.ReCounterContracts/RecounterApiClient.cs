using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SystemTools.ApiContracts;
using SystemTools.ReCounterContracts.V1.Routes;
using SystemTools.SharedKernel;

namespace SystemTools.ReCounterContracts;

public /*open*/ class ReCounterApiClient : ApiClient
{
    protected ReCounterApiClient(ILogger logger, IHttpClientFactory httpClientFactory,
        ReCounterMessageHubClient messageHubClient, string server, string? apiKey, bool useConsole) : base(logger,
        httpClientFactory, server, apiKey, messageHubClient, useConsole)
    {
    }

    public Task<Result<ProgressData>> GetCurrentProcessStatus(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<ProgressData>(
            RecountMessagesRoutes.ReCounterRoute.Recounter + RecountMessagesRoutes.ReCounterRoute.CurrentProcessStatus,
            false, cancellationToken);
    }

    public Task<Result<bool>> CancelCurrentProcess(CancellationToken cancellationToken = default)
    {
        //გაუქმების მოთხოვნისას პროგრესის შეტყობინებები არ უნდა მივიღოთ და დავბეჭდოთ
        return PostAsyncReturn<bool>(
            RecountMessagesRoutes.ReCounterRoute.Recounter + RecountMessagesRoutes.ReCounterRoute.CancelCurrentProcess,
            false, cancellationToken);
    }
}
