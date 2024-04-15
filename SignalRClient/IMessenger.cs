using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient;

public interface IMessenger
{
    Task SendMessage(string message, CancellationToken cancellationToken);
}