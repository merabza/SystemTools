using System;
using Serilog;
using SystemTools.SharedKernel;

namespace SystemTools.SystemToolsShared.Errors;

public static class SystemToolsErrors
{
    public static Error UnexpectedError => Error.Problem(nameof(UnexpectedError), "გაუთვალისწინებელი შეცდომა");

    public static Error SuchARecordAlreadyExists =>
        Error.Problem(nameof(SuchARecordAlreadyExists), "ასეთი ჩანაწერი უკვე არსებობს");

    public static Error TheEntryHasBeenUsedAndCannotBeDeleted =>
        Error.Problem(nameof(TheEntryHasBeenUsedAndCannotBeDeleted), "ჩანაწერი გამოყენებულია და ვერ წაიშლება");

    public static Error ErrorCaught(string methodName, string errorMessage)
    {
        return Error.Problem(nameof(ErrorCaught), $"Error in {methodName} {errorMessage}");
    }

    public static Error VirtualMethodOverrideNotImplemented(string methodName)
    {
        return Error.Problem(nameof(VirtualMethodOverrideNotImplemented),
            $"Virtual Method {methodName} Override did Not Implemented");
    }

    public static Error MethodNotImplemented(string methodName)
    {
        return Error.Problem(nameof(MethodNotImplemented), $"Method {methodName} did Not Implemented");
    }

    public static Error HandlerNotImplemented(string methodName)
    {
        return Error.Problem(nameof(MethodNotImplemented), $"Handler {methodName} did Not Implemented");
    }

    public static Error ErrorWhenRunningMethod(string methodName, Guid errorGuid)
    {
        return Error.Problem(nameof(ErrorWhenRunningMethod),
            $"{errorGuid} Error When Loading Data With Method {methodName}");
    }

    public static Error UnexpectedApiException(Exception e)
    {
        var errorId = Guid.NewGuid();
        Log.Error("{ErrorId} - {EMessage}{NewLine}{EStackTrace}", errorId, e.Message, Environment.NewLine,
            e.StackTrace);
        return Error.Failure(nameof(UnexpectedApiException), $"გაუთვალისწინებელი შეცდომა: {errorId}");
    }

    public static Error RunProcessError(string errorMessage)
    {
        return Error.Problem(nameof(RunProcessError), $"RunProcessError: {errorMessage}");
    }

    public static Error UnexpectedDatabaseException(Exception e)
    {
        var errorId = Guid.NewGuid();
        Log.Error("{ErrorId} - {EMessage}{NewLine}{EStackTrace}", errorId, e.Message, Environment.NewLine,
            e.StackTrace);
        return Error.Problem(nameof(UnexpectedDatabaseException),
            $"მონაცემთა ბაზასთან დაკავშირებული გაუთვალისწინებელი შეცდომა: {errorId}");
    }
}
