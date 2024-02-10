namespace SystemToolsShared.ErrorModels;

public static class ApiClientErrors
{

    public static Err ApiReturnedAnError(string errorMessage) => new()
        { ErrorCode = nameof(ApiReturnedAnError), ErrorMessage = $"Api Returned an Error: {errorMessage}" };

    public static readonly Err ApiUnknownError = new()
        { ErrorCode = nameof(ApiUnknownError), ErrorMessage = "Api returned an unknown error" };

    public static readonly Err ApiDidNotReturnAnything = new()
        { ErrorCode = nameof(ApiDidNotReturnAnything), ErrorMessage = "api did not return anything" };


    /*
            return new Err[] { new() { ErrorCode = "ApiReturnNothing", ErrorMessage = "Nothing returned Api" } };
     */
}