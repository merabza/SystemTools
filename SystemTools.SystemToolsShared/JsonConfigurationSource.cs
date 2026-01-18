using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace SystemTools.SystemToolsShared;

public sealed class JsonConfigurationSource : Microsoft.Extensions.Configuration.Json.JsonConfigurationSource
{
    public JsonConfigurationSource(IFileProvider? fileProvider, string path, bool optional, bool reloadOnChange,
        string key, string appSetEnKeysFileName)
    {
        FileProvider = fileProvider;
        Path = path;
        Optional = optional;
        ReloadOnChange = reloadOnChange;
        Key = key;
        AppSetEnKeysFileName = appSetEnKeysFileName;
    }

    public string Key { get; }
    public string AppSetEnKeysFileName { get; }

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        // ReSharper disable once DisposableConstructor
        return new JsonConfigurationProvider(this);
    }
}