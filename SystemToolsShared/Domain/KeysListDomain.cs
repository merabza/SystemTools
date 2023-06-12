using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SystemToolsShared.Models;

namespace SystemToolsShared.Domain;

public sealed class KeysListDomain
{
    private KeysListDomain(List<string> keys)
    {
        Keys = keys;
    }

    public List<string> Keys { get; set; }


    public static KeysListDomain? LoadFromFile(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return null;

        var appSetEnKeysJsonString = File.ReadAllText(filename);
        var appSetEnKeysList = JsonConvert.DeserializeObject<KeysList>(appSetEnKeysJsonString);

        if (appSetEnKeysList is null || appSetEnKeysList.Keys is null)
            return null;

        List<string> keys = new();
        foreach (var s in appSetEnKeysList.Keys)
            if (s is not null)
                keys.Add(s);


        return new KeysListDomain(keys);
    }
}