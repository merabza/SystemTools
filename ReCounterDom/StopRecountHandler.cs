using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MessagingAbstractions;
using SystemToolsShared;
using OneOf;

namespace ReCounterDom;

// ReSharper disable once ClassNeverInstantiated.Global
public class CancelCurrentProcessHandler : ICommandHandler<CancelCurrentProcessCommandRequest, bool>
{
    private readonly IServiceProvider _services;

    //IReCounterBackgroundTaskQueue backgroundTaskQueue, 
    // ReSharper disable once ConvertToPrimaryConstructor
    public CancelCurrentProcessHandler(IServiceProvider services)
    {
        _services = services;
    }

    public async Task<OneOf<bool, IEnumerable<Err>>> Handle(CancelCurrentProcessCommandRequest request,
        CancellationToken cancellationToken)
    {
        var service = _services.GetService(typeof(ReCounterQueuedHostedService));

        if (service is null)
            throw new ArgumentNullException(nameof(service));

        // ReSharper disable once using
        using var reCounterQueuedHostedService = (ReCounterQueuedHostedService)service;

        await reCounterQueuedHostedService.StopAsync(cancellationToken);
        await reCounterQueuedHostedService.StartAsync(cancellationToken);
        return await Task.FromResult(true);
    }
}