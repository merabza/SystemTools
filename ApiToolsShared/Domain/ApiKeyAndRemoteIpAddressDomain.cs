namespace ApiToolsShared.Domain;

public sealed class ApiKeyAndRemoteIpAddressDomain
{
    public ApiKeyAndRemoteIpAddressDomain(string apiKey, string remoteIpAddress)
    {
        ApiKey = apiKey;
        RemoteIpAddress = remoteIpAddress;
    }

    //აპის გასაღები
    public string ApiKey { get; }

    //IP მისამართი, საიდანაც ამ აპის გასაღების საშუალებით შეძლებენ შემოსვლას
    public string RemoteIpAddress { get; }
}