using System.Collections.Generic;

namespace ApiKeysManagement.Models;

public sealed class ApiKeys
{
    //პარამეტრები რომლებიც მიბმულია აპის გასაღებზე
    // ReSharper disable once CollectionNeverUpdated.Global
    public ICollection<ApiKeyByRemoteIpAddressModel>? AppSettingsByApiKey { get; set; }
}
