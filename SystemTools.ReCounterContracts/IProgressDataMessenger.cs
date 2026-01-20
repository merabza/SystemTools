using System.Threading;
using System.Threading.Tasks;

namespace SystemTools.ReCounterContracts;

public interface IProgressDataMessenger
{
    Task SendProgressData(ProgressData progressData, CancellationToken cancellationToken = default);
}