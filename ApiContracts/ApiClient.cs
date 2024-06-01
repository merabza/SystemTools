using System.Net.Http.Headers;
using System.Text;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SystemToolsShared;
using SystemToolsShared.ErrorModels;
using OneOf;

namespace ApiContracts;

public /*open*/ abstract class ApiClient : IApiClient //IDisposable, IAsyncDisposable, 
{
    private readonly string? _accessToken;
    private readonly string? _apiKey;
    private readonly HttpClient _client;
    private readonly ILogger _logger;
    private readonly string _server;
    //private readonly bool _withMessaging;
    private readonly IMessageHubClient? _messageHubClient;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected ApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server,
        string? apiKey, string? accessToken, IMessageHubClient? messageHubClient)
    {
        _logger = logger;
        _server = server.RemoveNotNeedLastPart('/');
        _apiKey = apiKey;
        _messageHubClient = messageHubClient;
        _accessToken = accessToken;
        _client = httpClientFactory.CreateClient();
    }

    //public async ValueTask DisposeAsync()
    //{
    //    // ReSharper disable once SuspiciousTypeConversion.Global
    //    if (_client is IAsyncDisposable clientAsyncDisposable)
    //        await clientAsyncDisposable.DisposeAsync();
    //    else
    //        _client.Dispose();
    //}

    //public void Dispose()
    //{
    //    _client.Dispose();
    //}

    private async Task<Option<Err[]>> LogResponseErrorMessage(HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return null;

        StShared.WriteErrorLine(
            $"answer after uri: {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}", true, null,
            false);

        StShared.WriteErrorLine($"Error from server: {response.StatusCode} {response.ReasonPhrase}", true, null, false);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(responseBody))
            return new Err[]
                { new() { ErrorCode = "UnexpectedServerError", ErrorMessage = "Unexpected Server Error" } };

        var errors = JsonConvert.DeserializeObject<IEnumerable<Err>>(responseBody)?.ToArray();
        if (errors is not null)
            foreach (var err in errors)
                StShared.WriteErrorLine($"Error from server: {err.ErrorMessage}", true);

        var errorMessage = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
        _logger.LogError("Returned error message from ApiClient: {errorMessage}", errorMessage);

        return errors?.Length > 0 ? errors : [ApiClientErrors.ApiReturnedAnError(errorMessage)];
    }

    protected async Task<Option<Err[]>> GetAsync(string afterServerAddress, CancellationToken cancellationToken)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?apikey={_apiKey}")}");

        if (_messageHubClient is not null)
            await _messageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (_messageHubClient is not null)
            await _messageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected async Task<OneOf<string, Err[]>> GetAsyncAsString(string afterServerAddress,
        CancellationToken cancellationToken)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?apikey={_apiKey}")}");

        if (_messageHubClient is not null)
            await _messageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (_messageHubClient is not null)
            await _messageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync(cancellationToken);

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected async Task<Option<Err[]>> DeleteAsync(string afterServerAddress, CancellationToken cancellationToken)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?apikey={_apiKey}")}");

        if (_messageHubClient is not null)
            await _messageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var response = await _client.DeleteAsync(uri, cancellationToken);

        if (_messageHubClient is not null)
            await _messageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected async Task<Option<Err[]>> PostAsync(string afterServerAddress, CancellationToken cancellationToken,
        string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?apikey={_apiKey}")}");

        if (_messageHubClient is not null)
            await _messageHubClient.RunMessages(cancellationToken);

        if (_accessToken is not null && _client.DefaultRequestHeaders.Authorization is null)
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Authorization", $"Bearer {_accessToken}");

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, "application/json");

        // ReSharper disable once using
        using var response = await _client.PostAsync(uri, content, cancellationToken);

        if (_messageHubClient is not null)
            await _messageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected async Task<Option<Err[]>> PutAsync(string afterServerAddress, CancellationToken cancellationToken,
        string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?apikey={_apiKey}")}");

        if (_messageHubClient is not null)
            await _messageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, "application/json");

        // ReSharper disable once using
        var response = await _client.PutAsync(uri, content, cancellationToken);

        if (_messageHubClient is not null)
            await _messageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected async Task<OneOf<string, Err[]>> PostAsyncReturnString(string afterServerAddress,
        CancellationToken cancellationToken, string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?apikey={_apiKey}")}");

        if (_messageHubClient is not null)
            await _messageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, "application/json");

        // ReSharper disable once using
        var response = await _client.PostAsync(uri, content, cancellationToken);

        if (_messageHubClient is not null)
            await _messageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync(cancellationToken);

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }


    protected async Task<OneOf<T, Err[]>> PostAsyncReturn<T>(string afterServerAddress,
        CancellationToken cancellationToken, string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?apikey={_apiKey}")}");

        if (_messageHubClient is not null)
            await _messageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, "application/json");

        // ReSharper disable once using
        using var response = await _client.PostAsync(uri, content, cancellationToken);

        if (_messageHubClient is not null)
            await _messageHubClient.StopMessages(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var respResult = await LogResponseErrorMessage(response, cancellationToken);
            if (respResult.IsSome)
                return (Err[])respResult;
            return new[] { ApiClientErrors.ApiUnknownError };
        }

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        var desResult = JsonConvert.DeserializeObject<T>(result);
        if (desResult is null)
            return new[] { ApiClientErrors.ApiDidNotReturnAnything };
        return desResult;
    }

    protected async Task<OneOf<T, Err[]>> GetAsyncReturn<T>(string afterServerAddress,
        CancellationToken cancellationToken)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? string.Empty : $"?apikey={_apiKey}")}");

        if (_messageHubClient is not null)
            await _messageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (_messageHubClient is not null)
            await _messageHubClient.StopMessages(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var respResult = await LogResponseErrorMessage(response, cancellationToken);
            if (respResult.IsSome)
                return (Err[])respResult;
            return new[] { ApiClientErrors.ApiUnknownError };
        }

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        var desResult = JsonConvert.DeserializeObject<T>(result);
        if (desResult is null)
            return new[] { ApiClientErrors.ApiDidNotReturnAnything };
        return desResult;
    }

    //private MessageHubClient? _messageHubClient;

    //public async Task<Option<Err[]>> StartMessageHubMonitoring(CancellationToken cancellationToken)
    //{
    //    if (!_withMessaging) 
    //        return null;

    //    _messageHubClient = new MessageHubClient(_server, _apiKey);
    //    await _messageHubClient.RunMessages(cancellationToken);

    //    return null;
    //}

    //public async Task<Option<Err[]>> StopMessageHubMonitoring(CancellationToken cancellationToken)
    //{

    //    if (_messageHubClient is not null)
    //        await _messageHubClient.StopMessages(cancellationToken);

    //    return null;
    //}
}
