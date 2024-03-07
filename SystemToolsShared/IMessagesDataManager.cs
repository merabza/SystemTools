using System.Threading;
using System.Threading.Tasks;

namespace SystemToolsShared;

public interface IMessagesDataManager
{
    void Dispose();
    Task SendMessage(string? userName, string message, CancellationToken cancellationToken);
    void UserConnected(string connectionId, string userName);
    void UserDisconnected(string connectionId, string userName);
}