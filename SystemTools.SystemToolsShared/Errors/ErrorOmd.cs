using System.Collections.Generic;

namespace SystemTools.SystemToolsShared.Errors;

public record ErrorOmd
{
    public static readonly ErrorOmd None = new() { Code = string.Empty, Name = string.Empty };
    public static readonly ErrorOmd NullValue = new() { Code = "ErrorOmd.NullValue", Name = "Null value was provided" };

    public required string Code { get; init; }
    public required string Name { get; init; }

    public static ErrorOmd[] Create(ErrorOmd err)
    {
        return [err];
    }

    public static ErrorOmd[] CreateArr(ErrorOmd err)
    {
        return [err];
    }

    public static ErrorOmd[] RecreateErrors(IEnumerable<ErrorOmd> haveErrors, ErrorOmd addError)
    {
        var errors = new List<ErrorOmd>();
        errors.AddRange(haveErrors);
        errors.Add(addError);
        return [.. errors];
    }

    public static ErrorOmd[] RecreateErrors(IEnumerable<ErrorOmd> haveErrors, IEnumerable<ErrorOmd> addError)
    {
        var errors = new List<ErrorOmd>();
        errors.AddRange(haveErrors);
        errors.AddRange(addError);
        return [.. errors];
    }

    public static void PrintErrorsOnConsole(IEnumerable<ErrorOmd> errors)
    {
        foreach (ErrorOmd error in errors)
        {
            StShared.WriteErrorLine(error.Name, true, null, false);
        }
    }
}
