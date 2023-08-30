using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SignalRClient;

namespace SystemToolsShared;

public /*open*/ class ApiClient
{
    private readonly ILogger _logger;
    protected readonly string? ApiKey;
    private readonly HttpClient _client;
    private readonly string _server;

    protected ApiClient(ILogger logger, string server, string? apiKey)
    {
        _logger = logger;
        _server = server.AddNeedLastPart('/');
        ApiKey = apiKey;
        _client = new HttpClient();
    }

    private void LogResponseErrorMessage(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;
        var errorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //_messagesDataManager?.SendMessage(_userName, $"Returned error message from ApiClient: {errorMessage}").Wait();
        _logger.LogError("Returned error message from ApiClient: {errorMessage}", errorMessage);
    }

    protected async Task<string?> GetAsyncAsString(string afterServerAddress, bool withMessaging = true)
    {
        Uri uri = new($"{_server}{afterServerAddress}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(_server, ApiKey);
            await webAgentMessageHubClient.RunMessages();
        }

        var response = await _client.GetAsync(uri);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        LogResponseErrorMessage(response);
        return null;
    }

    protected async Task<bool> DeleteAsync(string afterServerAddress)
    {
        Uri uri = new($"{_server}{afterServerAddress}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, ApiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await _client.DeleteAsync(uri);

        await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return true;

        LogResponseErrorMessage(response);
        return false;
    }

    protected async Task<bool> PostAsync(string afterServerAddress, string? bodyJsonData = null)
    {
        Uri uri = new($"{_server}{afterServerAddress}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, ApiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await _client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"));

        await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return true;

        LogResponseErrorMessage(response);
        return false;
    }

    protected async Task<string?> PostAsyncReturnString(string afterServerAddress, string? bodyJsonData = null)
    {
        Uri uri = new($"{_server}{afterServerAddress}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(_server, ApiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await _client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"));

        await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return response.Content.ReadAsStringAsync().Result;

        LogResponseErrorMessage(response);
        return null;
    }
}