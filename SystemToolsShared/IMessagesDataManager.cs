using System.Threading;
using System.Threading.Tasks;

namespace SystemToolsShared;

public interface IMessagesDataManager
{
    Task SendMessage(string? userName, string message, CancellationToken cancellationToken = default);
    void UserConnected(string connectionId, string userName);
    void UserDisconnected(string connectionId, string userName);
}