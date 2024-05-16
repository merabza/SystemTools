using System;
using System.Reflection;
using Serilog;

namespace SystemToolsShared.ErrorModels;

public static class SystemToolsErrors
{
    public static readonly Err UnexpectedError = new()
    {
        ErrorCode = nameof(UnexpectedError),
        ErrorMessage = "გაუთვალისწინებელი შეცდომა"
    };

    public static Err SuchARecordAlreadyExists => new()
        { ErrorCode = nameof(SuchARecordAlreadyExists), ErrorMessage = "ასეთი ჩანაწერი უკვე არსებობს" };

    public static Err TheEntryHasBeenUsedAndCannotBeDeleted => new()
    {
        ErrorCode = nameof(TheEntryHasBeenUsedAndCannotBeDeleted),
        ErrorMessage = "ჩანაწერი გამოყენებულია და ვერ წაიშლება"
    };

    public static Err ErrorCaught(string methodName, string errorMessage)
    {
        return new Err
        {
            ErrorCode = nameof(ErrorCaught),
            ErrorMessage = $"Error in {methodName} {errorMessage}"
        };
    }

    public static Err VirtualMethodOverrideNotImplemented(string methodName)
    {
        return new Err
        {
            ErrorCode = nameof(VirtualMethodOverrideNotImplemented),
            ErrorMessage = $"Virtual Method {methodName} Override did Not Implemented"
        };
    }

    public static Err MethodNotImplemented(string methodName)
    {
        return new Err
            { ErrorCode = nameof(MethodNotImplemented), ErrorMessage = $"Method {methodName} did Not Implemented" };
    }

    public static Err HandlerNotImplemented(string methodName)
    {
        return new Err
            { ErrorCode = nameof(MethodNotImplemented), ErrorMessage = $"Handler {methodName} did Not Implemented" };
    }

    public static Err ErrorWhenRunningMethod(string methodName, Guid errorGuid)
    {
        return new Err
        {
            ErrorCode = nameof(ErrorWhenRunningMethod),
            ErrorMessage = $"{errorGuid} Error When Loading Data With Method {methodName}"
        };
    }

    public static Err UnexpectedApiException(Exception e)
    {
        var errorId = Guid.NewGuid();
        Log.Error($"{errorId} - {e.Message}{Environment.NewLine}{e.StackTrace}");
        return new Err
            { ErrorCode = nameof(UnexpectedApiException), ErrorMessage = $"გაუთვალისწინებელი შეცდომა: {errorId}" };
    }

    public static Err RunProcessError(string errorMessage)
    {
        return new Err { ErrorCode = nameof(RunProcessError), ErrorMessage = $"RunProcessError: {errorMessage}" };
    }
}