using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OneOf;
using ProcessorWorkerServiceContracts.V1.Routes;
using ReCounterContracts;
using SystemToolsShared.Errors;

namespace ProcessorWorkerServiceContracts;

public class ProcessorWorkerServiceApiClient : RecounterApiClient
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public ProcessorWorkerServiceApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server,
        string? apiKey, bool useConsole) : base(logger, httpClientFactory, server, apiKey, useConsole)
    {
    }

    public async Task<OneOf<bool, Err[]>> IsWordsByLemmasClear(CancellationToken cancellationToken)
    {
        return await GetAsyncReturn<bool>(
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ReCounterBase +
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.IsWordsByLemmasClear, cancellationToken);
    }

    public async Task<Option<Err[]>> GenerateWordsByLemmasList(ECountType countType,
        CancellationToken cancellationToken)
    {
        return await PostAsync(
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ReCounterBase +
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.GenerateWordsByLemmasList + "/?countType=" + countType,
            cancellationToken);
    }

    public async Task<Option<Err[]>> ClearRedundantLemmas(CancellationToken cancellationToken)
    {
        return await PostAsync(
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ReCounterBase +
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ClearRedundantLemmas, cancellationToken);
    }

    public async Task<Option<Err[]>> ClearRedundantWords(CancellationToken cancellationToken)
    {
        return await PostAsync(
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ReCounterBase +
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ClearRedundantWords, cancellationToken);
    }

    public async Task<Option<Err[]>> ClearWordsByLemmas(CancellationToken cancellationToken)
    {
        return await PostAsync(
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ReCounterBase +
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ClearWordsByLemmas, cancellationToken);
    }

    public async Task<Option<Err[]>> ClearRawWordsByLemmas(CancellationToken cancellationToken)
    {
        return await PostAsync(
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ReCounterBase +
            ProcessorWorkerServiceApiRoutes.ReCounterRoute.ClearRawWordsByLemmas, cancellationToken);
    }
}