using System.Threading.Tasks;
using ApiKeysManagement.Domain;

namespace ApiKeysManagement;

public interface IApiKeyFinder
{
    Task<ApiKeyAndRemoteIpAddressDomain?> GetApiKeyAndRemAddress(string apiKey, string remoteIpAddress);
}
