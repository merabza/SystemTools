using System.Threading;
using System.Threading.Tasks;

namespace StringMessagesApiContracts;

public interface IMessenger
{
    Task SendMessage(string message, CancellationToken cancellationToken = default);
}