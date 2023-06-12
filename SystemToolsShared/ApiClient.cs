using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SystemToolsShared;

public /*open*/ class ApiClient
{
    private readonly ILogger _logger;
    protected readonly string? ApiKey;
    protected readonly HttpClient Client;
    protected readonly string Server;

    protected ApiClient(ILogger logger, string server, string? apiKey)
    {
        _logger = logger;
        Server = server.AddNeedLastPart('/');
        ApiKey = apiKey;
        Client = new HttpClient();
    }

    protected void LogResponseErrorMessage(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;
        var errorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        _logger.LogError("Returned error message from ApiClient: {errorMessage}", errorMessage);
    }

    //public virtual bool CheckApiLive()
    //{
    //    return false;
    //}

    protected async Task<string?> GetAsyncAsString(string afterServerAddress)
    {
        Uri uri = new($"{Server}{afterServerAddress}");

        var response = await Client.GetAsync(uri);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        LogResponseErrorMessage(response);
        return null;
    }

    //protected async Task<bool> PostAsyncWithApiKey(string afterServerAddress)
    //{
    //    Uri uri = new($"{Server}{afterServerAddress}");

    //    var body = new ApiKeyModel { ApiKey = ApiKey };
    //    var bodyApiKeyJsonData = JsonConvert.SerializeObject(body);

    //    var response = await Client.PostAsync(uri,
    //        new StringContent(bodyApiKeyJsonData, Encoding.UTF8, "application/text"));

    //    if (response.IsSuccessStatusCode)
    //        return true;

    //    LogResponseErrorMessage(response);
    //    return false;
    //}

    protected async Task<bool> PostAsync(string afterServerAddress)
    {
        Uri uri = new($"{Server}{afterServerAddress}");

        var response = await Client.PostAsync(uri, null);

        if (response.IsSuccessStatusCode)
            return true;

        LogResponseErrorMessage(response);
        return false;
    }

    public async Task<string?> GetVersion(bool useConsole = false)
    {
        try
        {
            return await GetAsyncAsString("test/getversion");
        }
        catch (Exception e)
        {
            StShared.WriteErrorLine(e.Message, useConsole);
            return null;
        }
    }

    public async Task<bool> CheckValidation()
    {
        Console.WriteLine("Try connect to Web Agent...");

        var version = await GetVersion();

        if (string.IsNullOrWhiteSpace(version))
            return false;

        Console.WriteLine($"Connected successfully, Web Agent version is {version}");

        return true;
    }
}