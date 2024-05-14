using System.Threading;
using System.Threading.Tasks;
using SignalRContracts.Models;

namespace SignalRContracts;

public interface IProgressDataMessenger : IMessenger
{
    Task SendProgressData(ProgressData progressData, CancellationToken cancellationToken);
}