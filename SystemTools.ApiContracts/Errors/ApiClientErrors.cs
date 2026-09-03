using SystemTools.SharedKernel;

namespace SystemTools.ApiContracts.Errors;

public static class ApiClientErrors
{
    public static readonly Error ApiDidNotReturnAnything =
        Error.Problem(nameof(ApiDidNotReturnAnything), "api did not return anything");

    public static Error ApiUnknownError => Error.Problem(nameof(ApiUnknownError), "Api returned an unknown error");

    public static Error UnexpectedServerError =>
        Error.Problem(nameof(UnexpectedServerError), "Unexpected Server Error");

    public static Error ApiReturnedAnError(string errorMessage)
    {
        return Error.Problem(nameof(ApiReturnedAnError), $"Api Returned an Error: {errorMessage}");
    }

    /*
            return new ErrorOmd[] { new() { Code = "ApiReturnNothing", Name = "Nothing returned Api" } };
     */
}
