using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    private readonly string _server;

    private readonly bool _useConsole;

    //protected იყენებს SystemTools
    protected string? AccessToken = null;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected ApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server, string? apiKey,
        IMessageHubClient? messageHubClient, bool useConsole)
    {
        _logger = logger;
        _server = server.RemoveNotNeedLastPart('/');
        _apiKey = apiKey;
        MessageHubClient = messageHubClient;
        _useConsole = useConsole;
        _client = httpClientFactory.CreateClient();
    }

    //protected იყენებს SystemTools
    protected IMessageHubClient? MessageHubClient { get; }


    private async ValueTask<Option<IEnumerable<Err>>> LogResponseErrorMessage(HttpResponseMessage response,
        string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode)
            return null;

        if (_useConsole)
        {
            StShared.WriteErrorLine(
                $"answer after uri: {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}", true,
                null, false);

            if (!string.IsNullOrWhiteSpace(bodyJsonData))
                StShared.WriteErrorLine($"request body was : {bodyJsonData}", true, null, false);

            StShared.WriteErrorLine($"Error from server: {response.StatusCode} {response.ReasonPhrase}", true, null,
                false);
        }

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

    protected Task<Option<IEnumerable<Err>>> GetAsync(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return GetAsync(afterServerAddress, true, cancellationToken);
    }

    public async ValueTask<bool> RunMessages(CancellationToken cancellationToken = default)
    {
        if (MessageHubClient is null)
            return false;
        return await MessageHubClient.RunMessages(cancellationToken);
    }

    public async ValueTask<bool> StopMessages(CancellationToken cancellationToken = default)
    {
        if (MessageHubClient is null)
            return false;
        return await MessageHubClient.StopMessages(cancellationToken);
    }

    private async Task<Option<IEnumerable<Err>>> GetAsync(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
            await MessageHubClient.RunMessages(cancellationToken);

        SetAuthorizationAccessToken();

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
            await MessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, null, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected async Task<Option<IEnumerable<Err>>> GetWithTokenAsync(string token, string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(afterServerAddress);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, null, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    private void SetAuthorizationAccessToken()
    {
        if ((AccessToken is not null && _client.DefaultRequestHeaders.Authorization is null) ||
            _client.DefaultRequestHeaders.Authorization?.Parameter is null ||
            _client.DefaultRequestHeaders.Authorization.Parameter != AccessToken)
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
    }

    protected async Task<OneOf<string, IEnumerable<Err>>> GetAsyncAsString(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(afterServerAddress);

        if (MessageHubClient is not null)
            await MessageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (MessageHubClient is not null)
            await MessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync(cancellationToken);

        var respResult = await LogResponseErrorMessage(response, null, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected async ValueTask<Option<IEnumerable<Err>>> DeleteAsync(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(afterServerAddress);

        if (MessageHubClient is not null)
            await MessageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var response = await _client.DeleteAsync(uri, cancellationToken);

        if (MessageHubClient is not null)
            await MessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, null, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected ValueTask<Option<IEnumerable<Err>>> PostAsync(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return PostAsync(afterServerAddress, true, null, cancellationToken);
    }

    protected ValueTask<Option<IEnumerable<Err>>> PostAsync(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        return PostAsync(afterServerAddress, useMessageHubClient, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში DatabaseApiClient-ის მიერ
    // ReSharper disable once MemberCanBePrivate.Global
    protected async ValueTask<Option<IEnumerable<Err>>> PostAsync(string afterServerAddress, bool useMessageHubClient,
        string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
            await MessageHubClient.RunMessages(cancellationToken);

        SetAuthorizationAccessToken();

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, MediaTypeNames.Application.Json);

        // ReSharper disable once using
        using var response = await _client.PostAsync(uri, content, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
            await MessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, bodyJsonData, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected Task<Option<IEnumerable<Err>>> PutAsync(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return PutAsync(afterServerAddress, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში
    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task<Option<IEnumerable<Err>>> PutAsync(string afterServerAddress, string? bodyJsonData,
        CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(afterServerAddress);

        if (MessageHubClient is not null)
            await MessageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, MediaTypeNames.Application.Json);

        // ReSharper disable once using
        var response = await _client.PutAsync(uri, content, cancellationToken);

        if (MessageHubClient is not null)
            await MessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, bodyJsonData, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected ValueTask<OneOf<string, IEnumerable<Err>>> PostAsyncReturnString(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturnString(afterServerAddress, true, null, cancellationToken);
    }

    protected ValueTask<OneOf<string, IEnumerable<Err>>> PostAsyncReturnString(string afterServerAddress,
        bool useMessageHubClient, CancellationToken cancellationToken = default)
    {
        return PostAsyncReturnString(afterServerAddress, useMessageHubClient, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში
    // ReSharper disable once MemberCanBePrivate.Global
    protected async ValueTask<OneOf<string, IEnumerable<Err>>> PostAsyncReturnString(string afterServerAddress,
        bool useMessageHubClient, string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
            await MessageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, MediaTypeNames.Application.Json);

        // ReSharper disable once using
        var response = await _client.PostAsync(uri, content, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
            await MessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync(cancellationToken);

        var respResult = await LogResponseErrorMessage(response, bodyJsonData, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError };
    }

    protected Task<OneOf<T, IEnumerable<Err>>> PostAsyncReturn<T>(string afterServerAddress,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturn<T>(afterServerAddress, true, null, cancellationToken);
    }

    protected Task<OneOf<T, IEnumerable<Err>>> PostAsyncReturn<T>(string afterServerAddress, bool useMessageHubClient,
        CancellationToken cancellationToken = default)
    {
        return PostAsyncReturn<T>(afterServerAddress, useMessageHubClient, null, cancellationToken);
    }

    //გამოიყენება SupportTools პროექტში
    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task<OneOf<T, IEnumerable<Err>>> PostAsyncReturn<T>(string afterServerAddress,
        bool useMessageHubClient, string? bodyJsonData, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
            await MessageHubClient.RunMessages(cancellationToken);

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, MediaTypeNames.Application.Json);

        // ReSharper disable once using
        using var response = await _client.PostAsync(uri, content, cancellationToken);

        if (useMessageHubClient && MessageHubClient is not null)
            await MessageHubClient.StopMessages(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var respResult = await LogResponseErrorMessage(response, bodyJsonData, cancellationToken);
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

    protected async Task<OneOf<T, IEnumerable<Err>>> GetAsyncReturn<T>(string afterServerAddress,
        bool useMessageHubClient, CancellationToken cancellationToken = default)
    {
        var uri = CreateUri(afterServerAddress);

        if (useMessageHubClient && MessageHubClient is not null)
            await MessageHubClient.RunMessages(cancellationToken);

        SetAuthorizationAccessToken();

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (MessageHubClient is not null)
            await MessageHubClient.StopMessages(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var respResult = await LogResponseErrorMessage(response, null, cancellationToken);
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