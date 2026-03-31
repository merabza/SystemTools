using System.Collections.Generic;

namespace SystemTools.SystemToolsShared.Errors;

public record Error
{
    public static readonly Error None = new() { Code = string.Empty, Name = string.Empty };
    public static readonly Error NullValue = new() { Code = "Error.NullValue", Name = "Null value was provided" };

    public required string Code { get; init; }
    public required string Name { get; init; }

    public static Error[] Create(Error err)
    {
        return [err];
    }

    public static Error[] CreateArr(Error err)
    {
        return [err];
    }

    public static Error[] RecreateErrors(IEnumerable<Error> haveErrors, Error addError)
    {
        var errors = new List<Error>();
        errors.AddRange(haveErrors);
        errors.Add(addError);
        return errors.ToArray();
    }

    public static Error[] RecreateErrors(IEnumerable<Error> haveErrors, IEnumerable<Error> addError)
    {
        var errors = new List<Error>();
        errors.AddRange(haveErrors);
        errors.AddRange(addError);
        return errors.ToArray();
    }

    public static void PrintErrorsOnConsole(IEnumerable<Error> errors)
    {
        foreach (Error error in errors)
        {
            StShared.WriteErrorLine(error.Name, true, null, false);
        }
    }
}
