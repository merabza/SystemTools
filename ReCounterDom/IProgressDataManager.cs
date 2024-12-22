using System.Threading;
using System.Threading.Tasks;
using ReCounterContracts;

namespace ReCounterDom;

public interface IProgressDataManager
{
    ProgressData? AccumulatedProgressData { get; }

    Task SetProgressData(string? userName, string name, string message, bool instantly,
        CancellationToken cancellationToken = default);

    Task SetProgressData(string? userName, string name, bool value, bool instantly,
        CancellationToken cancellationToken = default);

    Task SetProgressData(string? userName, string name, int value, bool instantly, CancellationToken cancellationToken = default);
    void StopTimer();
    void UserConnected(string connectionId, string? userName);
    void UserDisconnected(string connectionId, string? userName);
}