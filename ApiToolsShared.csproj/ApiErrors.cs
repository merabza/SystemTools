using SystemToolsShared;

namespace ApiToolsShared;

public static class ApiErrors
{
    public static readonly Err InvalidRemoteAddress = new()
        { ErrorCode = nameof(InvalidRemoteAddress), ErrorMessage = "Invalid Remote Address" };

    public static readonly Err ApiKeyIsInvalid = new()
        { ErrorCode = nameof(ApiKeyIsInvalid), ErrorMessage = "API Key is invalid" };

    public static readonly Err RequestIsEmpty = new()
        { ErrorCode = nameof(RequestIsEmpty), ErrorMessage = "Request is Empty" };

    public static readonly Err SomeRequestParametersAreNotValid = new()
    {
        ErrorCode = nameof(SomeRequestParametersAreNotValid), ErrorMessage = "Some request parameters are not valid"
    };
}