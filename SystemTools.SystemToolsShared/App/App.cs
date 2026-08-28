using Microsoft.Extensions.Options;

namespace SystemTools.SystemToolsShared.App;

public class App(IOptions<AppOptions> options) : IApplication
{
    public string AppName { get; } = options.Value.AppName;
}
