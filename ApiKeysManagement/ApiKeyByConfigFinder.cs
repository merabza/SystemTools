using ApiKeysManagement.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ApiKeysManagement;

public class ApiKeyByConfigFinder : IApiKeyFinder
{
    private readonly ILogger<ApiKeyByConfigFinder> _logger;
    private readonly IConfiguration _configuration;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ApiKeyByConfigFinder(ILogger<ApiKeyByConfigFinder> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<ApiKeyAndRemoteIpAddressDomain?> GetApiKeyAndRemAddress(string apiKey, string remoteIpAddress)
    {
        var apiKeys = ApiKeysDomain.Create(_configuration, _logger);

        //_logger.LogInformation($"View Api Keys. count is - {apiKeys.ApiKeys.Count}");
        //foreach (ApiKeyAndRemoteIpAddressDomain key in apiKeys.ApiKeys)
        //{
        //    _logger.LogInformation($"RemoteIpAddress is - {key.RemoteIpAddress}");
        //    _logger.LogInformation($"ApiKey is - {key.ApiKey}");
        //}
        //_logger.LogInformation("View Api Keys Finished");

        return await Task.FromResult(apiKeys.AppSettingsByApiKey(apiKey, remoteIpAddress));
    }
}