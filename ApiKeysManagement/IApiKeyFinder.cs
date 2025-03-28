using ApiKeysManagement.Domain;
using System.Threading.Tasks;

namespace ApiKeysManagement;

public interface IApiKeyFinder
{
    Task<ApiKeyAndRemoteIpAddressDomain?> GetApiKeyAndRemAddress(string apiKey, string remoteIpAddress);
}