using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SystemTools.ApiKeysManagement.Models;

namespace SystemTools.ApiKeysManagement.Domain;

public sealed class ApiKeysDomain
{
    //public საჭიროა ApiKeysChecker-ში ლოგირებისას
    // ReSharper disable once MemberCanBePrivate.Global
    public HashSet<ApiKeyAndRemoteIpAddressDomain> ApiKeys { get; } = [];

    public static ApiKeysDomain Create(IConfiguration configuration, ILogger logger)
    {
        IConfigurationSection apiKeysSection = configuration.GetSection("ApiKeys");
        var apiKeys = apiKeysSection.Get<ApiKeys>();

        var apiKeysDomain = new ApiKeysDomain();
        //var count = apiKeys?.AppSettingsByApiKey?.Count ?? 0;
        //logger.LogInformation("ApiKeys count is {count}", count);
        if (apiKeys?.AppSettingsByApiKey is null)
        {
            return apiKeysDomain;
        }

        foreach (ApiKeyByRemoteIpAddressModel apiKeyByRemoteIpAddressModel in apiKeys.AppSettingsByApiKey)
        {
            string? apiKey = apiKeyByRemoteIpAddressModel.ApiKey;
            string? remoteIpAddress = apiKeyByRemoteIpAddressModel.RemoteIpAddress;
            //logger.LogInformation("ApiKey={ApiKey} for {RemoteIpAddress}", apiKey, remoteIpAddress);
            if (apiKeyByRemoteIpAddressModel.ApiKey is null || apiKeyByRemoteIpAddressModel.RemoteIpAddress is null)
            {
                logger.LogError("Invalid ApiKey or RemoteIpAddress ApiKey={ApiKey} for {RemoteIpAddress}", apiKey,
                    remoteIpAddress);
                continue;
            }

            apiKeysDomain.ApiKeys.Add(new ApiKeyAndRemoteIpAddressDomain
            {
                ApiKey = apiKeyByRemoteIpAddressModel.ApiKey,
                RemoteIpAddress = apiKeyByRemoteIpAddressModel.RemoteIpAddress
            });
        }

        return apiKeysDomain;
    }

    public ApiKeyAndRemoteIpAddressDomain? AppSettingsByApiKey(string apiKey, string remoteIpAddress)
    {
        return ApiKeys.SingleOrDefault(s => s.ApiKey == apiKey && s.RemoteIpAddress == remoteIpAddress);
    }
}
