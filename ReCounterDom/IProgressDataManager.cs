using System.Threading;
using System.Threading.Tasks;
using ReCounterContracts;

namespace ReCounterDom;

public interface IProgressDataManager
{
    ProgressData? AccumulatedProgressData { get; }
    Task SetProgressData(string? userName, string name, string message, bool instantly, CancellationToken cancellationToken);
    Task SetProgressData(string? userName, string name, bool value, bool instantly, CancellationToken cancellationToken);
    Task SetProgressData(string? userName, string name, int value, bool instantly, CancellationToken cancellationToken);
    void StopTimer();
    void UserConnected(string connectionId, string? userName);
    void UserDisconnected(string connectionId, string? userName);
}