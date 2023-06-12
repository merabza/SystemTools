using System.IO;
using System.Linq;
using SystemToolsShared.Domain;

namespace SystemToolsShared;

public sealed class JsonConfigurationProvider : Microsoft.Extensions.Configuration.Json.JsonConfigurationProvider
{
    private readonly JsonConfigurationSource _source;

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
            return;
        var key = _source.Key;


        // Do decryption here, you can tap into the Data property like so:
        foreach (var s in appSetEnKeysList.Keys.SelectMany(dataKey =>
                     Data.Keys.Where(w => IsRelevant(dataKey, w)).ToList()))
            //Console.WriteLine($"start Decript key={key}, Data[s]={Data[s]}");
            Data[s] = EncryptDecrypt.DecryptString(Data[s], key);
        //Console.WriteLine($"Decripted key={key}, Data[s]={Data[s]}");
        // But you have to make your own MyEncryptionLibrary, not included here
    }

    private static bool IsRelevant(string dataKey, string dk)
    {
        var keys = dataKey.Split(":");
        var dKeys = dk.Split(":");

        if (keys.Length > dKeys.Length)
            return false;

        for (var i = 0; i < keys.Length; i++)
        {
            if (keys[i] == "[]")
                if (!int.TryParse(dKeys[i], out _))
                    return false;

            if (keys[i] != "*" && keys[i] != dKeys[i])
                return false;
        }

        return true;
    }
}