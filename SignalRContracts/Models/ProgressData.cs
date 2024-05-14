using System.Collections.Generic;
using Newtonsoft.Json;

namespace SignalRContracts.Models;

public class ProgressData
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Dictionary<string, bool> BoolData { get; set; } = [];

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Dictionary<string, int> IntData { get; set; } = [];

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Dictionary<string, string> StrData { get; set; } = [];

    public void Add(string name, string message)
    {
        StrData[name] = message;
    }

    public void Add(string name, int value)
    {
        IntData[name] = value;
    }

    public void Add(string name, bool value)
    {
        BoolData[name] = value;
    }
}