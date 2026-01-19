using System;
using System.Collections.Generic;

namespace SystemTools.SystemToolsShared.Errors;

public struct Err : IEquatable<Err>
{
    //[JsonProperty(PropertyName = "errorCode")]
    public string ErrorCode { get; set; }

    //[JsonProperty(PropertyName = "errorMessage")]
    public string ErrorMessage { get; init; }

    //public Err(string errorCode, string errorMessage)
    //{
    //    ErrorCode = errorCode;
    //    ErrorMessage = errorMessage;
    //}
    public static Err[] Create(Err err)
    {
        return [err];
    }

    public static Err[] CreateArr(Err err)
    {
        return [err];
    }

    public static Err[] RecreateErrors(IEnumerable<Err> haveErrors, Err addError)
    {
        var errors = new List<Err>();
        errors.AddRange(haveErrors);
        errors.Add(addError);
        return errors.ToArray();
    }

    public static Err[] RecreateErrors(IEnumerable<Err> haveErrors, IEnumerable<Err> addError)
    {
        var errors = new List<Err>();
        errors.AddRange(haveErrors);
        errors.AddRange(addError);
        return errors.ToArray();
    }

    public static void PrintErrorsOnConsole(IEnumerable<Err> errors)
    {
        foreach (Err error in errors)
        {
            StShared.WriteErrorLine(error.ErrorMessage, true, null, false);
        }
    }

    public bool Equals(Err other)
    {
        return ErrorCode == other.ErrorCode && ErrorMessage == other.ErrorMessage;
    }

    public override bool Equals(object? obj)
    {
        return obj is Err other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ErrorCode, ErrorMessage);
    }

    public static bool operator ==(Err left, Err right) => left.Equals(right);

    public static bool operator !=(Err left, Err right) => !(left == right);
}
