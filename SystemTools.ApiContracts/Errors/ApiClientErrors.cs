using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.ApiContracts.Errors;

public static class ApiClientErrors
{
    public static readonly Error UnexpectedServerError = new()
    {
        Code = nameof(UnexpectedServerError), Name = "Unexpected Server Error"
    };

    public static readonly Error ApiUnknownError = new()
    {
        Code = nameof(ApiUnknownError), Name = "Api returned an unknown error"
    };

    public static readonly Error ApiDidNotReturnAnything = new()
    {
        Code = nameof(ApiDidNotReturnAnything), Name = "api did not return anything"
    };

    public static Error ApiReturnedAnError(string errorMessage)
    {
        return new Error { Code = nameof(ApiReturnedAnError), Name = $"Api Returned an Error: {errorMessage}" };
    }

    /*
            return new Error[] { new() { ErrorCode = "ApiReturnNothing", ErrorMessage = "Nothing returned Api" } };
     */
}
