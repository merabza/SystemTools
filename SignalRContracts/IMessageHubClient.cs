using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRContracts.Models;
using SignalRContracts.V1.Routes;

namespace SignalRContracts;

public interface IMessageHubClient
{
    Task RunMessages(CancellationToken cancellationToken);
    Task StopMessages(CancellationToken cancellationToken);
}