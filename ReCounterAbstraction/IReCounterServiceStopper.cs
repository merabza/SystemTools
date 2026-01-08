using System.Threading;
using System.Threading.Tasks;

namespace ReCounterAbstraction;

public interface IReCounterServiceStopper
{
    Task StopAsync(CancellationToken token);
}