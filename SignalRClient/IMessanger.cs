using System.Threading.Tasks;

namespace SignalRClient;

public interface IMessenger
{
    Task SendMessage(string message);
}