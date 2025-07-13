using System.IO;
using Newtonsoft.Json;

namespace SystemToolsShared;

//საჭიროა DbContextAnalyzer-ში
public sealed class FileLoader
{
    private readonly bool _useConsole;

    // ReSharper disable once ConvertToPrimaryConstructor
    private FileLoader(bool useConsole)
    {
        _useConsole = useConsole;
    }

    private static string Load(string filePath)
    {
        // Open the text file using a stream reader.
        // ReSharper disable once using
        // ReSharper disable once DisposableConstructor
        using var reader = new StreamReader(filePath);
        // Read the stream to a string, and return.
        return reader.ReadToEnd();
    }

    private T? DeserializeResolve<T>(string fileName)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(Load(fileName));
        }
        catch(JsonReaderException jre)
        {
            StShared.WriteException(jre, "The file could not be deserialized:", _useConsole);
        }
        catch (IOException e)
        {
            StShared.WriteException(e, "The file could not be read:", _useConsole);
        }

        return default;
    }

    public static T? LoadDeserializeResolve<T>(string fileName, bool useConsole)
    {
        //var fileStreamManager = new FileStreamManager();
        var fileLoader = new FileLoader(useConsole);
        return fileLoader.DeserializeResolve<T>(fileName);
    }
}