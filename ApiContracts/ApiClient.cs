using System.Net.Http.Headers;
using System.Text;
using ApiContracts.Errors;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneOf;
using SystemToolsShared;
using SystemToolsShared.Errors;

namespace ApiContracts;

public /*open*/ abstract class ApiClient : IApiClient
{
    private readonly string? _apiKey;
    private readonly HttpClient _client;
    private readonly ILogger _logger;
    private readonly IMessageHubClient? _messageHubClient;
    private readonly string _server;
    private readonly bool _useConsole;

    protected static string? AccessToken = null;


    // ReSharper disable once ConvertToPrimaryConstructor
    protected ApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server, string? apiKey,
        IMessageHubClient? messageHubClient, bool useConsole)
    {
        _logger = logger;
        _server = server.RemoveNotNeedLastPart('/');
        _apiKey = apiKey;
        _messageHubClient = messageHubClient;
        _useConsole = useConsole;
        _client = httpClientFactory.CreateClient();
    }

    private async Task<Option<Err[]>> LogResponseErrorMessage(HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return null;

        if (_useConsole)
            StShared.WriteErrorLine(
                $"answer after uri: {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}", true,
                null, false);

        if (_useConsole)
            StShared.WriteErrorLine($"Error from server: {response.StatusCode} {response.ReasonPhrase}", true, null,
                false);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(responseBody))
            return new[] { ApiClientErrors.UnexpectedServerError };

        var errors = JsonConvert.DeserializeObject<IEnumerable<Err>>(responseBody)?.ToArray();
        if (_useConsole && errors is not null)
            foreach (var err in errors)
                StShared.WriteErrorLine($"Error from server: {err.ErrorMessage}", true);

        var errorMessage = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
        _logger.LogError("Returned error message from ApiClient: {errorMessage}", errorMessage);

        return errors?.Length > 0 ? errors : [ApiClientErrors.ApiReturnedAnError(errorMessage)];
    }

    protected async Task<Option<Err[]>> GetAsync(string afterServerAddress, CancellationToken cancellationToken)
    {
        var uri = CreateUri(afterServerAddress);

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
        var uri = CreateUri(afterServerAddress);

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
        var uri = CreateUri(afterServerAddress);

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

    protected Task<Option<Err[]>> PostAsync(string afterServerAddress, CancellationToken cancellationToken)
    {
        return PostAsync(afterServerAddress, null, cancellationToken);
    }

    protected async Task<Option<Err[]>> PostAsync(string afterServerAddress, string? bodyJsonData,
        CancellationToken cancellationToken)
    {
        var uri = CreateUri(afterServerAddress);

        if (_messageHubClient is not null)
            await _messageHubClient.RunMessages(cancellationToken);

        if (AccessToken is not null && _client.DefaultRequestHeaders.Authorization is null)
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AccessToken);

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

    protected Task<Option<Err[]>> PutAsync(string afterServerAddress, CancellationToken cancellationToken)
    {
        return PutAsync(afterServerAddress, null, cancellationToken);
    }

    protected async Task<Option<Err[]>> PutAsync(string afterServerAddress, string? bodyJsonData,
        CancellationToken cancellationToken)
    {
        var uri = CreateUri(afterServerAddress);

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

    protected Task<OneOf<string, Err[]>> PostAsyncReturnString(string afterServerAddress,
        CancellationToken cancellationToken)
    {
        return PostAsyncReturnString(afterServerAddress, null, cancellationToken);
    }

    protected async Task<OneOf<string, Err[]>> PostAsyncReturnString(string afterServerAddress, string? bodyJsonData,
        CancellationToken cancellationToken)
    {
        var uri = CreateUri(afterServerAddress);

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


    protected Task<OneOf<T, Err[]>> PostAsyncReturn<T>(string afterServerAddress, CancellationToken cancellationToken)
    {
        return PostAsyncReturn<T>(afterServerAddress, null, cancellationToken);
    }


    protected async Task<OneOf<T, Err[]>> PostAsyncReturn<T>(string afterServerAddress, string? bodyJsonData,
        CancellationToken cancellationToken)
    {
        var uri = CreateUri(afterServerAddress);

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
        var uri = CreateUri(afterServerAddress);

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

    private Uri CreateUri(string afterServerAddress)
    {
        Uri uri = new($"{_server}{afterServerAddress}");
        if (!string.IsNullOrWhiteSpace(_apiKey))
            uri = string.IsNullOrEmpty(uri.Query)
                ? new Uri($"{uri}?apikey={_apiKey}")
                : new Uri($"{uri}&apikey={_apiKey}");
        return uri;
    }
}