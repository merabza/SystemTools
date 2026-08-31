using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SystemTools.ApiContracts.Errors;
using SystemTools.SharedKernel;
using SystemTools.SystemToolsShared;
using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.ApiContracts;

public /*open*/ abstract class ApiClient : IApiClient
{
    private readonly string? _apiKey;
    private readonly HttpClient _client;
    private readonly ILogger? _logger;
    private readonly string _server;

    private readonly bool _useConsole;

    //protected იყენებს SystemTools
    //readonly არ გამოდგება, JwtContractReCounterApiClient სეტავს
    // ReSharper disable once MemberCanBePrivate.Global
    protected string? AccessToken;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected ApiClient(ILogger? logger, IHttpClientFactory httpClientFactory, string server, string? apiKey,
        IMessageHubClient? messageHubClient, bool useConsole, string? accessToken = null)
    {
        _logger = logger;
        _server = server.RemoveNotNeedLastPart('/');
        _apiKey = apiKey;
        MessageHubClient = messageHubClient;
        _useConsole = useConsole;
        AccessToken = accessToken;
        _client = httpClientFactory.CreateClient();
    }

    //protected იყენებს SystemTools
    // ReSharper disable once MemberCanBePrivate.Global
    protected IMessageHubClient? MessageHubClient { get; }

    private async ValueTask<Result> LogResponseErrorMessage(HttpResponseMessage response, string? bodyJsonData,
        CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        if (_useConsole)
        {
            StShared.WriteErrorLine(
                $"answer after uri: {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}", true,
                null, false);

            if (!string.IsNullOrWhiteSpace(bodyJsonData))
            {
                StShared.WriteErrorLine($"request body was : {bodyJsonData}", true, null, false);
            }

            StShared.WriteErrorLine($"ErrorOmd from server: {response.StatusCode} {response.ReasonPhrase}", true, null,
                false);
        }

        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return Result.Failure(ApiClientErrors.UnexpectedServerError);
        }

        Error[]? errors = JsonConvert.DeserializeObject<Error[]>(responseBody)?.ToArray();
        if (_useConsole && errors is not null)
        {
            foreach (Error err in errors)
            {
                StShared.WriteErrorLine($"Error from server: {err.Code}", true);
            }
        }

        string errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger?.LogError("Returned error message from ApiClient: {Name}", errorMessage);

        //return errors?.Length > 0
        //    ? Result.Failure(errors)
           return Result.Failure(ApiClientErrors.ApiReturnedAnError(errorMessage));
    }

    protected Task<Result> GetAsync(string afterServerAddress, CancellationToken cancellationToken = default)
    {
        return GetAsync(afterServerAddress, true, cancellationToken);
    }

    public async ValueTask<bool> RunMessages(CancellationToken cancellationToken = default)
    {
        if (MessageHubClient is null)
        {
            return false;
        }

        return await MessageHubClient.RunMessages(cancellationToken);
    }

    public async ValueTask<bool> StopMessages(CancellationToken cancellationToken = default)
    {
        if (MessageHubClient is null)
        {
            return false;
        }

        return await MessageHubClient.StopMessages(cancellationToken);
    }

    private async Task<Result> GetAsync(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        SetAuthorizationAccessToken();

        // ReSharper disable once using
        using HttpResponseMessage response = await _client.GetAsync(uri, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        Result respResult = await LogResponseErrorMessage(response, null, cancellationToken);
        if (respResult.IsFailure)
        {
            return respResult;
        }

        return Result.Failure(ApiClientErrors.ApiUnknownError);
    }

    protected async Task<Result> GetWithTokenAsync(string token, string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // ReSharper disable once using
        using HttpResponseMessage response = await _client.GetAsync(uri, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        Result respResult = await LogResponseErrorMessage(response, null, cancellationToken);
        if (respResult.IsFailure)
        {
            return respResult;
        }

        return Result.Failure(ApiClientErrors.ApiUnknownError);
    }

    private void SetAuthorizationAccessToken()
    {
        if (AccessToken is not null && _client.DefaultRequestHeaders.Authorization is null ||
            _client.DefaultRequestHeaders.Authorization?.Parameter is null ||
            _client.DefaultRequestHeaders.Authorization.Parameter != AccessToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }
    }

    protected async Task<Result<string>> GetAsyncAsString(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using HttpResponseMessage response = await _client.GetAsync(uri, cancellationToken);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        Result respResult = await LogResponseErrorMessage(response, null, cancellationToken);
        if (respResult.IsFailure)
        {
            return Result.Failure<string>(respResult.Error);
        }

        return Result.Failure<string>(ApiClientErrors.ApiUnknownError);
    }

    protected async ValueTask<Result> DeleteAsync(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using HttpResponseMessage response = await _client.DeleteAsync(uri, cancellationToken);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        Result respResult = await LogResponseErrorMessage(response, null, cancellationToken);
        if (respResult.IsFailure)
        {
            return respResult;
        }

        return Result.Failure(ApiClientErrors.ApiUnknownError);
    }

    protected ValueTask<Result> PostAsync(string afterServerAddress, CancellationToken cancellationToken = default)
    {
        return PostAsync(afterServerAddress, true, null, cancellationToken);
    }

    protected ValueTask<Result> PostAsync(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        return PostAsync(afterServerAddress, useMessageHubClient, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში DatabaseApiClient-ის მიერ
    // ReSharper disable once MemberCanBePrivate.Global
    protected async ValueTask<Result> PostAsync(string afterServerAddress, bool useMessageHubClient,
        string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        SetAuthorizationAccessToken();

        // ReSharper disable once using
        using StringContent? content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, MediaTypeNames.Application.Json);

        // ReSharper disable once using
        using HttpResponseMessage response = await _client.PostAsync(uri, content, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        Result respResult = await LogResponseErrorMessage(response, bodyJsonData, cancellationToken);
        if (respResult.IsFailure)
        {
            return respResult;
        }

        return Result.Failure(ApiClientErrors.ApiUnknownError);
    }

    protected Task<Result> PutAsync(string afterServerAddress, CancellationToken cancellationToken = default)
    {
        return PutAsync(afterServerAddress, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში
    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task<Result> PutAsync(string afterServerAddress, string? bodyJsonData,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using StringContent? content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, MediaTypeNames.Application.Json);

        // ReSharper disable once using
        HttpResponseMessage response = await _client.PutAsync(uri, content, cancellationToken);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        Result respResult = await LogResponseErrorMessage(response, bodyJsonData, cancellationToken);
        if (respResult.IsFailure)
        {
            return respResult;
        }

        return Result.Failure(ApiClientErrors.ApiUnknownError);
    }

    protected ValueTask<Result<string>> PostAsyncReturnString(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturnString(afterServerAddress, true, null, cancellationToken);
    }

    protected ValueTask<Result<string>> PostAsyncReturnString(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturnString(afterServerAddress, useMessageHubClient, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში
    // ReSharper disable once MemberCanBePrivate.Global
    protected async ValueTask<Result<string>> PostAsyncReturnString(string afterServerAddress, bool useMessageHubClient,
        string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using StringContent? content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, MediaTypeNames.Application.Json);

        // ReSharper disable once using
        HttpResponseMessage response = await _client.PostAsync(uri, content, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        Result respResult = await LogResponseErrorMessage(response, bodyJsonData, cancellationToken);
        if (respResult.IsFailure)
        {
            return Result.Failure<string>(respResult.Error);
        }

        return Result.Failure<string>(ApiClientErrors.ApiUnknownError);
    }

    protected Task<Result<T>> PostAsyncReturn<T>(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturn<T>(afterServerAddress, true, null, cancellationToken);
    }

    protected Task<Result<T>> PostAsyncReturn<T>(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturn<T>(afterServerAddress, useMessageHubClient, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში
    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task<Result<T>> PostAsyncReturn<T>(string afterServerAddress, bool useMessageHubClient,
        string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using StringContent? content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, MediaTypeNames.Application.Json);

        // ReSharper disable once using
        using HttpResponseMessage response = await _client.PostAsync(uri, content, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        if (!response.IsSuccessStatusCode)
        {
            Result respResult = await LogResponseErrorMessage(response, bodyJsonData, cancellationToken);
            if (respResult.IsFailure)
            {
                return Result.Failure<T>(respResult.Error);
            }

            return Result.Failure<T>(ApiClientErrors.ApiUnknownError);
        }

        string result = await response.Content.ReadAsStringAsync(cancellationToken);
        var desResult = JsonConvert.DeserializeObject<T>(result);
        if (desResult is null)
        {
            return Result.Failure<T>(ApiClientErrors.ApiDidNotReturnAnything);
        }

        return desResult;
    }

    protected async Task<Result<T>> GetAsyncReturn<T>(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        Uri uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
        {
            await MessageHubClient.RunMessages(cancellationToken);
        }

        SetAuthorizationAccessToken();

        // ReSharper disable once using
        using HttpResponseMessage response = await _client.GetAsync(uri, cancellationToken);

        if (MessageHubClient is not null)
        {
            await MessageHubClient.StopMessages(cancellationToken);
        }

        if (!response.IsSuccessStatusCode)
        {
            Result respResult = await LogResponseErrorMessage(response, null, cancellationToken);
            if (respResult.IsFailure)
            {
                return Result.Failure<T>(respResult.Error);
            }

            return Result.Failure<T>(ApiClientErrors.ApiUnknownError);
        }

        string result = await response.Content.ReadAsStringAsync(cancellationToken);
        var desResult = JsonConvert.DeserializeObject<T>(result);
        if (desResult is null)
        {
            return Result.Failure<T>(ApiClientErrors.ApiDidNotReturnAnything);
        }

        return desResult;
    }

    private Uri CreateUri(string afterServerAddress)
    {
        var uri = new Uri($"{_server}{afterServerAddress}");
        if (!string.IsNullOrWhiteSpace(_apiKey))
        {
            uri = string.IsNullOrEmpty(uri.Query)
                ? new Uri($"{uri}?apikey={_apiKey}")
                : new Uri($"{uri}&apikey={_apiKey}");
        }

        return uri;
    }
}
