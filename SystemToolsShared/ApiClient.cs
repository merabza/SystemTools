using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SignalRClient;

namespace SystemToolsShared;

public /*open*/ class ApiClient
{
    private readonly ILogger _logger;
    private readonly string? _apiKey;
    private readonly HttpClient _client;
    private readonly string _server;

    protected ApiClient(ILogger logger, string server, string? apiKey)
    {
        _logger = logger;
        _server = server.AddNeedLastPart('/');
        _apiKey = apiKey;
        _client = new HttpClient();
    }

    private async Task LogResponseErrorMessage(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var errors = JsonConvert.DeserializeObject<IEnumerable<Err>>(responseBody);
        if (errors is not null)
            foreach (var err in errors)
                StShared.WriteErrorLine($"Error from server: {err.ErrorMessage}", true);

        var errorMessage = response.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
        _logger.LogError("Returned error message from ApiClient: {errorMessage}", errorMessage);
    }

    protected async Task<bool> GetAsync(string afterServerAddress, CancellationToken cancellationToken,
        bool withMessaging = true)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages();
        }

        var response = await _client.GetAsync(uri, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return true;

        await LogResponseErrorMessage(response, cancellationToken);
        return false;
    }

    protected async Task<string?> GetAsyncAsString(string afterServerAddress, CancellationToken cancellationToken,
        bool withMessaging = true)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
            await webAgentMessageHubClient.RunMessages();
        }

        var response = await _client.GetAsync(uri, cancellationToken);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync(cancellationToken);

        await LogResponseErrorMessage(response, cancellationToken);
        return null;
    }

    protected async Task<bool> DeleteAsync(string afterServerAddress, CancellationToken cancellationToken)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await _client.DeleteAsync(uri, cancellationToken);

        await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return true;

        await LogResponseErrorMessage(response, cancellationToken);
        return false;
    }

    protected async Task<bool> PostAsync(string afterServerAddress, CancellationToken cancellationToken,
        string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await _client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"),
            cancellationToken);

        await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return true;

        await LogResponseErrorMessage(response, cancellationToken);
        return false;
    }

    protected async Task<string?> PostAsyncReturnString(string afterServerAddress, CancellationToken cancellationToken,
        string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await _client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"),
            cancellationToken);

        await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync(cancellationToken);

        await LogResponseErrorMessage(response, cancellationToken);
        return null;
    }


    protected async Task<T?> PostAsyncReturn<T>(string afterServerAddress, CancellationToken cancellationToken,
        string? bodyJsonData = null)
    {
        Uri uri = new(
            $"{_server}{afterServerAddress}{(string.IsNullOrWhiteSpace(_apiKey) ? "" : $"?apikey={_apiKey}")}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, _apiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await _client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"),
            cancellationToken);

        await webAgentMessageHubClient.StopMessages();

        if (!response.IsSuccessStatusCode)
        {
            await LogResponseErrorMessage(response, cancellationToken);
            return default;
        }

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        var desResult = JsonConvert.DeserializeObject<T>(result);
        return desResult;
    }
}