using System.Threading;
using System.Threading.Tasks;
using SystemTools.ReCounterContracts;

namespace SystemTools.ReCounterAbstraction;

public interface IProgressDataManager
{
    ProgressData? AccumulatedProgressData { get; }

    ValueTask SetProgressData(string? userName, string name, string message, bool instantly,
        CancellationToken cancellationToken = default);

    ValueTask SetProgressData(string? userName, string name, bool value, bool instantly,
        CancellationToken cancellationToken = default);

    ValueTask SetProgressData(string? userName, string name, int value, bool instantly,
        CancellationToken cancellationToken = default);

    void StopTimer();
    void UserConnected(string connectionId, string? userName);
    void UserDisconnected(string connectionId, string? userName);
}
