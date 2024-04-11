﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneOf;
using SignalRClient;
using SystemToolsShared.ErrorModels;

namespace SystemToolsShared;

public /*open*/ class ApiClient: IDisposable, IAsyncDisposable
{
    private readonly string? _apiKey;
    private readonly string? _accessToken;
    private readonly bool _withMessaging;
    private readonly HttpClient _client;
    private readonly ILogger _logger;
    private readonly string _server;

    protected ApiClient(ILogger logger, string server, string? apiKey, string? accessToken, bool withMessaging)
    {
        _logger = logger;
        _server = server.AddNeedLastPart('/');
        _apiKey = apiKey;
        _withMessaging = withMessaging;
        _accessToken = accessToken;
        // ReSharper disable once DisposableConstructor
        _client = new HttpClient();
    }

    private async Task<Option<Err[]>> LogResponseErrorMessage(HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return null;

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
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
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (_withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

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
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (_withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

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
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (_withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using var response = await _client.DeleteAsync(uri, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

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
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (_withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

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

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

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
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (_withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, "application/json");

        // ReSharper disable once using
        var response = await _client.PutAsync(uri, content, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

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
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (_withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, "application/json");

        // ReSharper disable once using
        var response = await _client.PostAsync(uri, content, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

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
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (_withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using var content = bodyJsonData is null
            ? null
            // ReSharper disable once DisposableConstructor
            : new StringContent(bodyJsonData, Encoding.UTF8, "application/json");

        // ReSharper disable once using
        using var response = await _client.PostAsync(uri, content, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

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
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (_withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

        // ReSharper disable once using
        using var response = await _client.GetAsync(uri, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

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

    public void Dispose()
    {
        _client.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is IAsyncDisposable clientAsyncDisposable)
            await clientAsyncDisposable.DisposeAsync();
        else
            _client.Dispose();
    }
}