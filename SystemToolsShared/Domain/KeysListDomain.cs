using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        {
            return null;
        }

        string appSetEnKeysJsonString = File.ReadAllText(filename);
        var appSetEnKeysList = JsonConvert.DeserializeObject<KeysList>(appSetEnKeysJsonString);

        if (appSetEnKeysList?.Keys is null)
        {
            return null;
        }

        List<string> keys = [];
        keys.AddRange(appSetEnKeysList.Keys.OfType<string>());

        return new KeysListDomain(keys);
    }
}
