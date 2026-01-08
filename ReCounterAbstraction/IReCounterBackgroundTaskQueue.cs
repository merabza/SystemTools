using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReCounterAbstraction;

public interface IReCounterBackgroundTaskQueue
{
    void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
    Task<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken = default);
    void ClearQueue();
}