using System.Threading;
using System.Threading.Tasks;

namespace SignalRContracts;

public interface IMessenger
{
    Task SendMessage(string message, CancellationToken cancellationToken);
}