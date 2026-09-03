using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SystemTools.SharedKernel;

namespace SystemTools.ApiContracts.Tests.TestDoubles;

//ApiClient-ის protected მეთოდების ტესტებიდან გამოსაძახებლად
internal sealed class TestableApiClient : ApiClient
{
    public TestableApiClient(IHttpClientFactory httpClientFactory, string server, string? apiKey,
        IMessageHubClient? messageHubClient = null, ILogger? logger = null) : base(logger, httpClientFactory, server,
        apiKey, messageHubClient, false)
    {
    }

    public Task<Result> Get(string afterServerAddress, CancellationToken cancellationToken = default)
    {
        return GetAsync(afterServerAddress, cancellationToken);
    }

    public Task<Result> GetWithToken(string token, string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return GetWithTokenAsync(token, afterServerAddress, cancellationToken);
    }

    public Task<Result<string>> GetString(string afterServerAddress, CancellationToken cancellationToken = default)
    {
        return GetAsyncAsString(afterServerAddress, cancellationToken);
    }

    public Task<Result<T>> GetReturn<T>(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<T>(afterServerAddress, useMessageHubClient, cancellationToken);
    }

    public ValueTask<Result> Delete(string afterServerAddress, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(afterServerAddress, cancellationToken);
    }

    public ValueTask<Result> Post(string afterServerAddress, bool useMessageHubClient, string? bodyJsonData,
        CancellationToken cancellationToken = default)
    {
        return PostAsync(afterServerAddress, useMessageHubClient, bodyJsonData, cancellationToken);
    }

    public Task<Result> Put(string afterServerAddress, string? bodyJsonData,
        CancellationToken cancellationToken = default)
    {
        return PutAsync(afterServerAddress, bodyJsonData, cancellationToken);
    }

    public ValueTask<Result<string>> PostReturnString(string afterServerAddress, bool useMessageHubClient,
        string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        return PostAsyncReturnString(afterServerAddress, useMessageHubClient, bodyJsonData, cancellationToken);
    }

    public Task<Result<T>> PostReturn<T>(string afterServerAddress, bool useMessageHubClient, string? bodyJsonData,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturn<T>(afterServerAddress, useMessageHubClient, bodyJsonData, cancellationToken);
    }
}
