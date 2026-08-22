using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.ApiContracts.Errors;

public static class ApiClientErrors
{
    public static readonly ErrorOmd UnexpectedServerError = new()
    {
        Code = nameof(UnexpectedServerError), Name = "Unexpected Server ErrorOmd"
    };

    public static readonly ErrorOmd ApiUnknownError = new()
    {
        Code = nameof(ApiUnknownError), Name = "Api returned an unknown error"
    };

    public static readonly ErrorOmd ApiDidNotReturnAnything = new()
    {
        Code = nameof(ApiDidNotReturnAnything), Name = "api did not return anything"
    };

    public static ErrorOmd ApiReturnedAnError(string errorMessage)
    {
        return new ErrorOmd { Code = nameof(ApiReturnedAnError), Name = $"Api Returned an ErrorOmd: {errorMessage}" };
    }

    /*
            return new ErrorOmd[] { new() { Code = "ApiReturnNothing", Name = "Nothing returned Api" } };
     */
}
