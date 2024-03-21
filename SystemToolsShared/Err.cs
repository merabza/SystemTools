using System.Collections.Generic;

namespace SystemToolsShared;

public struct Err
{
    //[JsonProperty(PropertyName = "errorCode")]
    public string ErrorCode { get; set; }

    //[JsonProperty(PropertyName = "errorMessage")]
    public string ErrorMessage { get; set; }

    //public Err(string errorCode, string errorMessage)
    //{
    //    ErrorCode = errorCode;
    //    ErrorMessage = errorMessage;
    //}

    public static Err[] RecreateErrors(IEnumerable<Err> haveErrors, Err addError)
    {
        var errors = new List<Err>();
        errors.AddRange(haveErrors);
        errors.Add(addError);
        return [.. errors];
    }

    public static void PrintErrorsOnConsole(IEnumerable<Err> errors)
    {
        foreach (var error in errors) StShared.WriteErrorLine(error.ErrorMessage, true, null, false);
    }
}