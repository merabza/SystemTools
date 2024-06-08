using SystemToolsShared.Errors;

namespace ApiContracts.Errors;

public static class ApiClientErrors
{
    public static readonly Err UnexpectedServerError = new()
        { ErrorCode = nameof(UnexpectedServerError), ErrorMessage = "Unexpected Server Error" };

    public static readonly Err ApiUnknownError = new()
        { ErrorCode = nameof(ApiUnknownError), ErrorMessage = "Api returned an unknown error" };

    public static readonly Err ApiDidNotReturnAnything = new()
        { ErrorCode = nameof(ApiDidNotReturnAnything), ErrorMessage = "api did not return anything" };

    public static Err ApiReturnedAnError(string errorMessage)
    {
        return new Err
            { ErrorCode = nameof(ApiReturnedAnError), ErrorMessage = $"Api Returned an Error: {errorMessage}" };
    }


    /*
            return new Err[] { new() { ErrorCode = "ApiReturnNothing", ErrorMessage = "Nothing returned Api" } };
     */
}