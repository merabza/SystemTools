using System.IO;
using System.Linq;
using SystemTools.SystemToolsShared.Domain;

namespace SystemTools.SystemToolsShared;

public sealed class JsonConfigurationProvider : Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider
{
    private readonly JsonConfigurationSource _source;

    // ReSharper disable once ConvertToPrimaryConstructor
    public JsonConfigurationProvider(JsonConfigurationSource source) : base(source)
    {
        _source = source;
    }

    public override void Load(Stream stream)
    {
        // Let the base sealed class do the heavy lifting.
        base.Load(stream);

        //string appSetEnKeysJsonString = File.ReadAllText(_source.AppSetEnKeysFileName);
        //KeysList? appSetEnKeysList = JsonConvert.DeserializeObject<KeysList>(appSetEnKeysJsonString);

        //if (appSetEnKeysList is null || appSetEnKeysList.Keys is null)
        //    return;

        var appSetEnKeysList = KeysListDomain.LoadFromFile(_source.AppSetEnKeysFileName);
        if (appSetEnKeysList?.Keys is null)
        {
            return;
        }

        var key = _source.Key;
        var appSetEnKeys = appSetEnKeysList.Keys.ToList();

        // Do decryption here, you can tap into the Data property like so:
        foreach (var s in Data.Keys)
        {
            foreach (var dataKey in appSetEnKeys)
            {
                if (dataKey == s)
                {
                    Data[s] = EncryptDecrypt.DecryptString(Data[s], key);
                    appSetEnKeys.Remove(dataKey);
                    break;
                }

                if (!IsRelevant(dataKey, s))
                {
                    continue;
                }

                Data[s] = EncryptDecrypt.DecryptString(Data[s], key);
                break;
            }
        }

        //Console.WriteLine($"Decrypted key={key}, Data[s]={Data[s]}");
        // But you have to make your own MyEncryptionLibrary, not included here
    }

    public static bool IsRelevant(string dataKey, string dk)
    {
        var keys = dataKey.Split(":");
        var dKeys = dk.Split(":");

        if (keys.Length > dKeys.Length)
        {
            return false;
        }

        for (var i = 0; i < keys.Length; i++)
        {
            switch (keys[i])
            {
                case "*":
                    continue;
                case "[]":
                    {
                        if (!int.TryParse(dKeys[i], out _))
                        {
                            return false;
                        }

                        break;
                    }
                default:
                    {
                        if (keys[i] != dKeys[i])
                        {
                            return false;
                        }

                        break;
                    }
            }
        }

        return true;
    }
}
