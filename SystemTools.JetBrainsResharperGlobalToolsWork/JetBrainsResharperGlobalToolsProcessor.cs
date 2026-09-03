using Microsoft.Extensions.Logging;
using SystemTools.SharedKernel;
using SystemTools.SystemToolsShared;

namespace SystemTools.JetBrainsResharperGlobalToolsWork;

public sealed class JetBrainsResharperGlobalToolsProcessor
{
    private const string Jb = "jb";
    private readonly ILogger? _logger;
    private readonly bool _useConsole;

    // ReSharper disable once ConvertToPrimaryConstructor
    public JetBrainsResharperGlobalToolsProcessor(ILogger? logger, bool useConsole)
    {
        _logger = logger;
        _useConsole = useConsole;
    }

    public Result Cleanupcode(string path, bool includeJson = false)
    {
        return StShared.RunProcess(_useConsole, _logger, Jb,
            $"cleanupcode{(includeJson ? "" : " --exclude=\"**.json\"")} {path}");
    }
}
