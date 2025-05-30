﻿using SystemToolsShared.Errors;

namespace ApiContracts.Errors;

public static class ApiErrors
{
    public static readonly Err InvalidRemoteAddress = new()
    {
        ErrorCode = nameof(InvalidRemoteAddress), ErrorMessage = "Invalid Remote Address"
    };

    public static readonly Err ApiKeyIsInvalid = new()
    {
        ErrorCode = nameof(ApiKeyIsInvalid), ErrorMessage = "API Key is invalid"
    };

    public static readonly Err RequestIsEmpty = new()
    {
        ErrorCode = nameof(RequestIsEmpty), ErrorMessage = "Request is Empty"
    };

    public static readonly Err SomeRequestParametersAreNotValid = new()
    {
        ErrorCode = nameof(SomeRequestParametersAreNotValid), ErrorMessage = "Some request parameters are not valid"
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