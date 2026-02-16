using System.Threading.Tasks;
using SystemTools.ApiKeysManagement.Domain;

namespace SystemTools.ApiKeysManagement;

public interface IApiKeyFinder
{
    Task<ApiKeyAndRemoteIpAddressDomain?> GetApiKeyAndRemAddress(string apiKey, string remoteIpAddress);
}
