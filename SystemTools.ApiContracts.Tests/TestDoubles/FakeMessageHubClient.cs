using System.Threading;
using System.Threading.Tasks;

namespace SystemTools.ApiContracts.Tests.TestDoubles;

internal sealed class FakeMessageHubClient : IMessageHubClient
{
    public int RunCount { get; private set; }
    public int StopCount { get; private set; }

    public Task<bool> RunMessages(CancellationToken cancellationToken = default)
    {
        RunCount++;
        return Task.FromResult(true);
    }

    public ValueTask<bool> StopMessages(CancellationToken cancellationToken = default)
    {
        StopCount++;
        return ValueTask.FromResult(true);
    }
}
