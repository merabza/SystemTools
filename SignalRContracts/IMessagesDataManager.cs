using System.Threading;
using System.Threading.Tasks;

namespace SignalRContracts;

public interface IMessagesDataManager
{
    Task SendMessage(string? userName, string message, CancellationToken cancellationToken);
    void UserConnected(string connectionId, string userName);
    void UserDisconnected(string connectionId, string userName);
}