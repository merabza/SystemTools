using System.Threading;

namespace SystemTools.SystemToolsShared;

public sealed class ProgramAttributes
{
    private static ProgramAttributes? _instance;

    private static readonly Lock SyncRoot = new();
    //private readonly Dictionary<string, object> _attributes = new();

    // ReSharper disable once MemberCanBePrivate.Global
    private ProgramAttributes()
    {
    }

    public static ProgramAttributes Instance
    {
        get
        {
            //ეს ატრიბუტები სესიაზე არ არის დამოკიდებული და იქმნება პროგრამის გაშვებისთანავე, 
            //შემდგომში მასში ცვლილებები არ შედის,
            //მაგრამ შეიძლება პროგრამამ თავისი მუშაობის განმავლობაში რამდენჯერმე გამოიყენოს აქ არსებული ინფორმაცია
            if (_instance is not null) return _instance;

            lock (SyncRoot) //thread safe singleton
            {
                _instance = new ProgramAttributes();
            }

            return _instance;
        }
    }

    public string AppName { get; set; } = null!;
    public string AppKey { get; set; } = null!;

    //public static void SetTestInstance(ProgramAttributes newInstance)
    //{
    //    _instance = newInstance;
    //}

    //public void SetAttribute<TC>(string attributeName, TC attributeValue) where TC : notnull
    //{
    //    _attributes[attributeName] = attributeValue;
    //}

    //public TC? GetAttribute<TC>(string attributeName)
    //{
    //    if (_attributes.TryGetValue(attributeName, out var attribute))
    //        return (TC)attribute;
    //    return default;
    //}

    //public override string ToString()
    //{
    //    var sb = new StringBuilder();
    //    var atLeastOneAdded = false;
    //    foreach (var kvp in _attributes)
    //    {
    //        if (atLeastOneAdded)
    //            sb.Append(", ");
    //        sb.Append(kvp.Key);
    //        sb.Append('=');
    //        sb.Append(kvp.Value);
    //        atLeastOneAdded = true;
    //    }

    //    return sb.ToString();
    //}
}