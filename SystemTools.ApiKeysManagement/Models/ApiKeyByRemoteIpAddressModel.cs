namespace SystemTools.ApiKeysManagement.Models;

public sealed class ApiKeyByRemoteIpAddressModel
{
    //აპის გასაღები
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? ApiKey { get; set; }

    //IP მისამართი, საიდანაც ამ აპის გასაღების საშუალებით შეძლებენ შემოსვლას
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? RemoteIpAddress { get; set; }
}
