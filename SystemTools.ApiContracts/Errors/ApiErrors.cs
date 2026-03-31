using SystemTools.SystemToolsShared.Errors;

namespace SystemTools.ApiContracts.Errors;

public static class ApiErrors
{
    public static readonly Error InvalidRemoteAddress = new()
    {
        Code = nameof(InvalidRemoteAddress), Name = "Invalid Remote Address"
    };

    public static readonly Error ApiKeyIsInvalid = new()
    {
        Code = nameof(ApiKeyIsInvalid), Name = "API Key is invalid"
    };

    public static readonly Error RequestIsEmpty = new() { Code = nameof(RequestIsEmpty), Name = "Request is Empty" };

    public static readonly Error SomeRequestParametersAreNotValid = new()
    {
        Code = nameof(SomeRequestParametersAreNotValid), Name = "Some request parameters are not valid"
    };

    public static string IsEmptyErrMessage(string propertyNameLocalized)
    {
        return $"{propertyNameLocalized} შევსებული უნდა იყოს";
    }

    public static string IsLongerThenErrMessage(string propertyNameLocalized, int maxLength)
    {
        return $"{propertyNameLocalized} სიგრძე არ შეიძლება იყოს {maxLength} სიმბოლოზე მეტი";
    }
}
