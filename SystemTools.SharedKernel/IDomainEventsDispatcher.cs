using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SystemTools.SharedKernel;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
