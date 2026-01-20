using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SystemTools.ReCounterAbstraction;

public sealed class ReCounterQueuedHostedService : BackgroundService, IReCounterServiceStopper
{
    private readonly ILogger _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public ReCounterQueuedHostedService(IReCounterBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
    {
        TaskQueue = taskQueue;
        _logger = loggerFactory.CreateLogger<ReCounterQueuedHostedService>();
    }

    private IReCounterBackgroundTaskQueue TaskQueue { get; }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ReCounter Queued Hosted Service is stopping.");

        return base.StopAsync(cancellationToken);
    }

    public bool IsProcessRunning()
    {
        return ExecuteTask is not null;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReCounter Queued Hosted Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            Func<CancellationToken, Task>? workItem = await TaskQueue.DequeueAsync(stoppingToken);

            try
            {
                if (workItem is not null)
                {
                    await workItem(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred executing {nameof(workItem)}.");
            }
        }

        _logger.LogInformation("ReCounter Queued Hosted Service is stopping.");
    }
}
