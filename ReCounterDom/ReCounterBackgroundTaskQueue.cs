using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ReCounterDom;

public class ReCounterBackgroundTaskQueue : IReCounterBackgroundTaskQueue
{
    private readonly SemaphoreSlim _signal = new(0);
    private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();

    public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        _workItems.Enqueue(workItem);
        _signal.Release();
    }

    public async Task<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);
        _workItems.TryDequeue(out var workItem);

        return workItem;
    }

    public void ClearQueue()
    {
        _workItems.Clear();
    }
}