using System;
using Serilog;

namespace SystemTools.SystemToolsShared.Errors;

public static class SystemToolsErrors
{
    public static readonly Error UnexpectedError = new()
    {
        Code = nameof(UnexpectedError), Name = "გაუთვალისწინებელი შეცდომა"
    };

    public static Error SuchARecordAlreadyExists =>
        new() { Code = nameof(SuchARecordAlreadyExists), Name = "ასეთი ჩანაწერი უკვე არსებობს" };

    public static Error TheEntryHasBeenUsedAndCannotBeDeleted =>
        new() { Code = nameof(TheEntryHasBeenUsedAndCannotBeDeleted), Name = "ჩანაწერი გამოყენებულია და ვერ წაიშლება" };

    public static Error ErrorCaught(string methodName, string errorMessage)
    {
        return new Error { Code = nameof(ErrorCaught), Name = $"Error in {methodName} {errorMessage}" };
    }

    public static Error VirtualMethodOverrideNotImplemented(string methodName)
    {
        return new Error
        {
            Code = nameof(VirtualMethodOverrideNotImplemented),
            Name = $"Virtual Method {methodName} Override did Not Implemented"
        };
    }

    public static Error MethodNotImplemented(string methodName)
    {
        return new Error { Code = nameof(MethodNotImplemented), Name = $"Method {methodName} did Not Implemented" };
    }

    public static Error HandlerNotImplemented(string methodName)
    {
        return new Error { Code = nameof(MethodNotImplemented), Name = $"Handler {methodName} did Not Implemented" };
    }

    public static Error ErrorWhenRunningMethod(string methodName, Guid errorGuid)
    {
        return new Error
        {
            Code = nameof(ErrorWhenRunningMethod),
            Name = $"{errorGuid} Error When Loading Data With Method {methodName}"
        };
    }

    public static Error UnexpectedApiException(Exception e)
    {
        var errorId = Guid.NewGuid();
        Log.Error("{ErrorId} - {EMessage}{NewLine}{EStackTrace}", errorId, e.Message, Environment.NewLine,
            e.StackTrace);
        return new Error { Code = nameof(UnexpectedApiException), Name = $"გაუთვალისწინებელი შეცდომა: {errorId}" };
    }

    public static Error RunProcessError(string errorMessage)
    {
        return new Error { Code = nameof(RunProcessError), Name = $"RunProcessError: {errorMessage}" };
    }

    public static Error UnexpectedDatabaseException(Exception e)
    {
        var errorId = Guid.NewGuid();
        Log.Error("{ErrorId} - {EMessage}{NewLine}{EStackTrace}", errorId, e.Message, Environment.NewLine,
            e.StackTrace);
        return new Error
        {
            Code = nameof(UnexpectedDatabaseException),
            Name = $"მონაცემთა ბაზასთან დაკავშირებული გაუთვალისწინებელი შეცდომა: {errorId}"
        };
    }
}
