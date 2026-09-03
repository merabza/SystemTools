using SystemTools.SharedKernel;

namespace SystemTools.ApiContracts.Errors;

public static class ApiErrors
{
    public static Error InvalidRemoteAddress => Error.Problem(nameof(InvalidRemoteAddress), "Invalid Remote Address");

    public static Error ApiKeyIsInvalid => Error.Problem(nameof(ApiKeyIsInvalid), "API Key is invalid");

    public static Error RequestIsEmpty => Error.Problem(nameof(RequestIsEmpty), "Request is Empty");

    public static Error SomeRequestParametersAreNotValid =>
        Error.Problem(nameof(SomeRequestParametersAreNotValid), "Some request parameters are not valid");

    public static string IsEmptyErrMessage(string propertyNameLocalized)
    {
        return $"{propertyNameLocalized} შევსებული უნდა იყოს";
    }

    public static string IsLongerThenErrMessage(string propertyNameLocalized, int maxLength)
    {
        return $"{propertyNameLocalized} სიგრძე არ შეიძლება იყოს {maxLength} სიმბოლოზე მეტი";
    }
}
