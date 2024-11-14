using System.Threading;
using System.Threading.Tasks;

namespace ReCounterContracts;

public interface IProgressDataMessenger
{
    Task SendProgressData(ProgressData progressData, CancellationToken cancellationToken);
}