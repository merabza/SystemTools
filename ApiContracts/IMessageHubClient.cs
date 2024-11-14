using System.Threading;
using System.Threading.Tasks;

namespace ApiContracts;

public interface IMessageHubClient
{
    Task<bool> RunMessages(CancellationToken cancellationToken);
    Task<bool> StopMessages(CancellationToken cancellationToken);
}