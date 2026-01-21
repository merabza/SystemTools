using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SystemTools.ReCounterAbstraction;

public sealed class ReCounterBackgroundTaskQueue : IReCounterBackgroundTaskQueue, IDisposable
{
    private readonly SemaphoreSlim _signal = new(0);
    private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();

    public void Dispose()
    {
        _signal.Dispose();
    }

    public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        _workItems.Enqueue(workItem);
        _signal.Release();
    }

    public async Task<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        await _signal.WaitAsync(cancellationToken);
        _workItems.TryDequeue(out Func<CancellationToken, Task>? workItem);

        return workItem;
    }

    public void ClearQueue()
    {
        _workItems.Clear();
    }
}
