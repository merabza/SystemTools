using System.Threading;
using System.Threading.Tasks;

namespace ReCounterDom;

public interface IReCounterServiceStopper
{
    Task StopAsync(CancellationToken token);
}