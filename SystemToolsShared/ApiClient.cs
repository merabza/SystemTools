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
    private readonly IMessagesDataManager? _messagesDataManager;
    private readonly string? _userName;
    protected readonly HttpClient Client;
    protected readonly string Server;

    protected ApiClient(ILogger logger, string server, string? apiKey, IMessagesDataManager? messagesDataManager,
        string? userName)
    {
        _logger = logger;
        Server = server.AddNeedLastPart('/');
        ApiKey = apiKey;
        _messagesDataManager = messagesDataManager;
        _userName = userName;
        Client = new HttpClient();
    }

    protected void LogResponseErrorMessage(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;
        var errorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        _messagesDataManager?.SendMessage(_userName, $"Returned error message from ApiClient: {errorMessage}").Wait();
        _logger.LogError("Returned error message from ApiClient: {errorMessage}", errorMessage);
    }

    protected async Task<string?> GetAsyncAsString(string afterServerAddress, bool withMessaging = true)
    {
        Uri uri = new($"{Server}{afterServerAddress}");

        WebAgentMessageHubClient? webAgentMessageHubClient = null;
        if (withMessaging)
        {
            webAgentMessageHubClient = new WebAgentMessageHubClient(Server, ApiKey);
            await webAgentMessageHubClient.RunMessages();
        }

        var response = await Client.GetAsync(uri);

        if (webAgentMessageHubClient is not null)
            await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        LogResponseErrorMessage(response);
        return null;
    }

    protected async Task<bool> DeleteAsync(string afterServerAddress)
    {
        Uri uri = new($"{Server}{afterServerAddress}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(Server, ApiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await Client.DeleteAsync(uri);

        await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return true;

        LogResponseErrorMessage(response);
        return false;
    }

    protected async Task<bool> PostAsync(string afterServerAddress, string? bodyJsonData = null)
    {
        Uri uri = new($"{Server}{afterServerAddress}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(Server, ApiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await Client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"));

        await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return true;

        LogResponseErrorMessage(response);
        return false;
    }

    protected async Task<string?> PostAsyncReturnString(string afterServerAddress, string? bodyJsonData = null)
    {
        Uri uri = new($"{Server}{afterServerAddress}");

        var webAgentMessageHubClient = new WebAgentMessageHubClient(Server, ApiKey);
        await webAgentMessageHubClient.RunMessages();

        var response = await Client.PostAsync(uri,
            bodyJsonData is null ? null : new StringContent(bodyJsonData, Encoding.UTF8, "application/json"));

        await webAgentMessageHubClient.StopMessages();

        if (response.IsSuccessStatusCode)
            return response.Content.ReadAsStringAsync().Result;

        LogResponseErrorMessage(response);
        return null;
    }
}