namespace SystemToolsShared.ErrorModels;

public static class ApiErrors
{
    public static string IsEmptyErrMessage(string propertyNameLocalized)
    {
        return $"{propertyNameLocalized} შევსებული უნდა იყოს";
    }
    
    public static string IsLongerThenErrMessage(string propertyNameLocalized)
    {
        return propertyNameLocalized + " სიგრძე არ შეიძლება იყოს {MaxLength} სიმბოლოზე მეტი";
    }


}