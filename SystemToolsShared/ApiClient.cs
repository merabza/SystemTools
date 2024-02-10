using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

public /*open*/ class ApiClient
{
    private readonly string? _apiKey;
    private readonly HttpClient _client;
    private readonly ILogger _logger;
    private readonly string _server;

    protected ApiClient(ILogger logger, string server, string? apiKey)
    {
        _logger = logger;
        _server = server.AddNeedLastPart('/');
        _apiKey = apiKey;
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

    protected async Task<Option<Err[]>> GetAsync(string afterServerAddress, CancellationToken cancellationToken,
        bool withMessaging = true)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

        var response = await _client.GetAsync(uri, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError};
    }

    protected async Task<OneOf<string, Err[]>> GetAsyncAsString(string afterServerAddress,
        CancellationToken cancellationToken, bool withMessaging = true)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages(cancellationToken);
        }

        var response = await _client.GetAsync(uri, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync(cancellationToken);

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError};
    }

    protected async Task<Option<Err[]>> DeleteAsync(string afterServerAddress, CancellationToken cancellationToken)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages(cancellationToken);

        var response = await _client.DeleteAsync(uri, cancellationToken);

        await webAgentMessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError};
    }

    protected async Task<Option<Err[]>> PostAsync(string afterServerAddress, CancellationToken cancellationToken,
        string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages(cancellationToken);

        var response = await _client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"),
            cancellationToken);

        await webAgentMessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError};
    }

    protected async Task<Option<Err[]>> PutAsync(string afterServerAddress, CancellationToken cancellationToken,
        string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages(cancellationToken);

        var response = await _client.PutAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"),
            cancellationToken);

        await webAgentMessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return null;

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError};
    }

    protected async Task<OneOf<string, Err[]>> PostAsyncReturnString(string afterServerAddress,
        CancellationToken cancellationToken, string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages(cancellationToken);

        var response = await _client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"),
            cancellationToken);

        await webAgentMessageHubClient.StopMessages(cancellationToken);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync(cancellationToken);

        var respResult = await LogResponseErrorMessage(response, cancellationToken);
        if (respResult.IsSome)
            return (Err[])respResult;
        return new[] { ApiClientErrors.ApiUnknownError};
    }


    protected async Task<OneOf<T, Err[]>> PostAsyncReturn<T>(string afterServerAddress,
        CancellationToken cancellationToken, string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages(cancellationToken);

        var response = await _client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"),
            cancellationToken);

        await webAgentMessageHubClient.StopMessages(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var respResult = await LogResponseErrorMessage(response, cancellationToken);
            if (respResult.IsSome)
                return (Err[])respResult;
            return new[] { ApiClientErrors.ApiUnknownError};
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

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages(cancellationToken);

        var response = await _client.GetAsync(uri, cancellationToken);

        await webAgentMessageHubClient.StopMessages(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var respResult = await LogResponseErrorMessage(response, cancellationToken);
            if (respResult.IsSome)
                return (Err[])respResult;
            return new[] { ApiClientErrors.ApiUnknownError};
        }

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        var desResult = JsonConvert.DeserializeObject<T>(result);
        if (desResult is null)
            return new[] { ApiClientErrors.ApiDidNotReturnAnything };
        return desResult;
    }
}