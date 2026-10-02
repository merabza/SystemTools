using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace SystemTools.ApiKeysManagement.Tests.TestDoubles;

//ApiKeys:AppSettingsByApiKey სექცია მოცემული ჩანაწერებით. null მნიშვნელობა კონფიგურაციაში საერთოდ არ იწერება
internal static class ApiKeysConfiguration
{
    public static IConfigurationRoot Create(params (string? ApiKey, string? RemoteIpAddress)[] entries)
    {
        var values = new Dictionary<string, string?>();
        for (int i = 0; i < entries.Length; i++)
        {
            (string? apiKey, string? remoteIpAddress) = entries[i];
            if (apiKey is not null)
            {
                values[$"ApiKeys:AppSettingsByApiKey:{i}:ApiKey"] = apiKey;
            }

            if (remoteIpAddress is not null)
            {
                values[$"ApiKeys:AppSettingsByApiKey:{i}:RemoteIpAddress"] = remoteIpAddress;
            }
        }

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }
}
