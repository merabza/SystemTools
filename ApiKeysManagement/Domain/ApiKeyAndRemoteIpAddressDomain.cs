namespace ApiKeysManagement.Domain;

public sealed class ApiKeyAndRemoteIpAddressDomain
{
    //აპის გასაღები
    public required string ApiKey { get; set; }

    //IP მისამართი, საიდანაც ამ აპის გასაღების საშუალებით შეძლებენ შემოსვლას
    public required string RemoteIpAddress { get; set;  }
}