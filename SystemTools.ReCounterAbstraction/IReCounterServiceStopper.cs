using System.Threading;
using System.Threading.Tasks;

namespace SystemTools.ReCounterAbstraction;

public interface IReCounterServiceStopper
{
    Task StopAsync(CancellationToken token);
}