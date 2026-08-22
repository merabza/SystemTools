using System;
using Serilog;

namespace SystemTools.SystemToolsShared.Errors;

public static class SystemToolsErrors
{
    public static readonly ErrorOmd UnexpectedError = new()
    {
        Code = nameof(UnexpectedError), Name = "გაუთვალისწინებელი შეცდომა"
    };

    public static ErrorOmd SuchARecordAlreadyExists =>
        new() { Code = nameof(SuchARecordAlreadyExists), Name = "ასეთი ჩანაწერი უკვე არსებობს" };

    public static ErrorOmd TheEntryHasBeenUsedAndCannotBeDeleted =>
        new() { Code = nameof(TheEntryHasBeenUsedAndCannotBeDeleted), Name = "ჩანაწერი გამოყენებულია და ვერ წაიშლება" };

    public static ErrorOmd ErrorCaught(string methodName, string errorMessage)
    {
        return new ErrorOmd { Code = nameof(ErrorCaught), Name = $"ErrorOmd in {methodName} {errorMessage}" };
    }

    public static ErrorOmd VirtualMethodOverrideNotImplemented(string methodName)
    {
        return new ErrorOmd
        {
            Code = nameof(VirtualMethodOverrideNotImplemented),
            Name = $"Virtual Method {methodName} Override did Not Implemented"
        };
    }

    public static ErrorOmd MethodNotImplemented(string methodName)
    {
        return new ErrorOmd { Code = nameof(MethodNotImplemented), Name = $"Method {methodName} did Not Implemented" };
    }

    public static ErrorOmd HandlerNotImplemented(string methodName)
    {
        return new ErrorOmd { Code = nameof(MethodNotImplemented), Name = $"Handler {methodName} did Not Implemented" };
    }

    public static ErrorOmd ErrorWhenRunningMethod(string methodName, Guid errorGuid)
    {
        return new ErrorOmd
        {
            Code = nameof(ErrorWhenRunningMethod),
            Name = $"{errorGuid} ErrorOmd When Loading Data With Method {methodName}"
        };
    }

    public static ErrorOmd UnexpectedApiException(Exception e)
    {
        var errorId = Guid.NewGuid();
        Log.Error("{ErrorId} - {EMessage}{NewLine}{EStackTrace}", errorId, e.Message, Environment.NewLine,
            e.StackTrace);
        return new ErrorOmd { Code = nameof(UnexpectedApiException), Name = $"გაუთვალისწინებელი შეცდომა: {errorId}" };
    }

    public static ErrorOmd RunProcessError(string errorMessage)
    {
        return new ErrorOmd { Code = nameof(RunProcessError), Name = $"RunProcessError: {errorMessage}" };
    }

    public static ErrorOmd UnexpectedDatabaseException(Exception e)
    {
        var errorId = Guid.NewGuid();
        Log.Error("{ErrorId} - {EMessage}{NewLine}{EStackTrace}", errorId, e.Message, Environment.NewLine,
            e.StackTrace);
        return new ErrorOmd
        {
            Code = nameof(UnexpectedDatabaseException),
            Name = $"მონაცემთა ბაზასთან დაკავშირებული გაუთვალისწინებელი შეცდომა: {errorId}"
        };
    }
}
