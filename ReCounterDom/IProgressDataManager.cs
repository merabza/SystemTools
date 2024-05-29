using System.Threading.Tasks;
using System.Threading;

namespace ReCounterDom;

public interface IProgressDataManager
{
    //void SetProgressData(string name, string message, bool instantly);
    //void SetProgressData(string name, int value, bool instantly = false);
    //void SetProgressData(string name, bool value, bool instantly = true);
    Task SetProgressData(string name, string message, bool instantly, CancellationToken cancellationToken);
    Task SetProgressData(string name, bool value, bool instantly, CancellationToken cancellationToken);
    Task SetProgressData(string name, int value, bool instantly, CancellationToken cancellationToken);
    void StopTimer();
    void UserConnected(string connectionId);
    void UserDisconnected(string connectionId);
}