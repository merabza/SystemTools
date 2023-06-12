using System.IO;
using Newtonsoft.Json;

namespace SystemToolsShared;

//საჭიროა DbContextAnalyzer-ში
public sealed class FileLoader
{
    private readonly IFileStreamManager _fileManager;
    private readonly bool _useConsole;

    public FileLoader(IFileStreamManager fileManager, bool useConsole)
    {
        _fileManager = fileManager;
        _useConsole = useConsole;
    }

    public string Load(string filePath)
    {
        // Open the text file using a stream reader.
        using var reader = _fileManager.StreamReader(filePath);
        // Read the stream to a string, and return.
        return reader.ReadToEnd();
    }


    public T? DeserializeResolve<T>(string fileName)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(Load(fileName));
        }
        catch (IOException e)
        {
            StShared.WriteException(e, "The file could not be read:", _useConsole);
        }

        return default;
    }

    public static T? LoadDeserializeResolve<T>(string fileName, bool useConsole)
    {
        var fileStreamManager = new FileStreamManager();
        var fileLoader = new FileLoader(fileStreamManager, useConsole);
        return fileLoader.DeserializeResolve<T>(fileName);
    }
}