using Microsoft.Extensions.Options;

namespace SystemTools.SystemToolsShared.App;

public class Application(IOptions<ApplicationOptions> options) : IApplication
{
    public string AppName { get; } = options.Value.AppName;
}
