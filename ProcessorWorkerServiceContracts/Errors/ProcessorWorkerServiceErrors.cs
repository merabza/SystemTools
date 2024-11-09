using SystemToolsShared.Errors;

namespace ProcessorWorkerServiceContracts.Errors;

public static class ProcessorWorkerServiceErrors
{
    public static readonly Err UpdatingPronounsEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingPronounsEndedWithErrors),
        ErrorMessage = "ნაცვალსახელების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingGrammarCasesNamesEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingGrammarCasesNamesEndedWithErrors),
        ErrorMessage = "ბრუნვების სახელების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingNounNumbersNamesEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingNounNumbersNamesEndedWithErrors),
        ErrorMessage = "სახელების რიცხვების სახელების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingNounParadigmRowsEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingNounParadigmRowsEndedWithErrors),
        ErrorMessage = "სახელების პარადიგმების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingNounParadigmsEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingNounParadigmsEndedWithErrors),
        ErrorMessage = "სახელების პარადიგმების სახელების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingVerbParadigmRowsEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingVerbParadigmRowsEndedWithErrors),
        ErrorMessage = "ზმნების პარადიგმების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingVerbParadigmsEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingVerbParadigmsEndedWithErrors),
        ErrorMessage = "ზმნების პარადიგმების სახელების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingVerbRowsEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingVerbRowsEndedWithErrors),
        ErrorMessage = "ზმნების მწკრივების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingVerbSeriesEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingVerbSeriesEndedWithErrors),
        ErrorMessage = "ზმნების სერიების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingVerbTypesEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingVerbTypesEndedWithErrors),
        ErrorMessage = "ზმნების ტიპების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingInflectionTypesEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingInflectionTypesEndedWithErrors),
        ErrorMessage = "ფლექსიების ტიპების განახლება დასრულდა შეცდომებით"
    };

    public static readonly Err UpdatingSometimesAttachedParticleMorphemesByInflectionTypesEndedWithErrors = new()
    {
        ErrorCode = nameof(UpdatingSometimesAttachedParticleMorphemesByInflectionTypesEndedWithErrors),
        ErrorMessage = "სხვათა სიტყვის მორფემების განახლება დასრულდა შეცდომებით"
    };


    //public static readonly Err SameParametersAreEmpty = new()
    //    { ErrorCode = nameof(SameParametersAreEmpty), ErrorMessage = "Same Parameters are Empty" };

    //public static readonly Err FileStorageNameForExchangeDoesNotSpecified = new()
    //{
    //    ErrorCode = nameof(FileStorageNameForExchangeDoesNotSpecified),
    //    ErrorMessage = "File Storage Name For Exchange Does Not Specified"
    //};

    //public static readonly Err ExchangeFileStorageNotCreated = new()
    //    { ErrorCode = nameof(ExchangeFileStorageNotCreated), ErrorMessage = "Exchange File Storage Not Created" };

    //public static readonly Err LanguageModelFilesNotFoundOnExchangeStorage = new()
    //{
    //    ErrorCode = nameof(LanguageModelFilesNotFoundOnExchangeStorage),
    //    ErrorMessage = "Language Model files not found on exchange storage"
    //};

    //public static Err TheLanguageModelFileFolderNameCouldNotBeDetermined = new()
    //{
    //    ErrorCode = nameof(TheLanguageModelFileFolderNameCouldNotBeDetermined),
    //    ErrorMessage = "The Language Model file folder name could not be determined"
    //};

    //public static Err ParametersFileBodyIsEmpty = new()
    //    { ErrorCode = nameof(ParametersFileBodyIsEmpty), ErrorMessage = "Parameters File Body Is Empty" };

    //public static Err FileCanNotDeleted(string lModelDataFileFullPath)
    //{
    //    return new Err
    //        { ErrorCode = nameof(FileCanNotDeleted), ErrorMessage = $"File {lModelDataFileFullPath} can not Deleted" };
    //}

    //public static Err ParameterIsEmpty(string parameterName)
    //{
    //    return new Err { ErrorCode = nameof(ParameterIsEmpty), ErrorMessage = $"{parameterName} is empty" };
    //}

    //public static Err JsonDataIsEmpty(string parameterName)
    //{
    //    return new Err { ErrorCode = nameof(JsonDataIsEmpty), ErrorMessage = $"Json Data {parameterName} is empty" };
    //}

    //public static Err InstallerFolderDoesNotExists(string folderForDataFile)
    //{
    //    return new Err
    //    {
    //        ErrorCode = "InstallerFolderDoesNotExists",
    //        ErrorMessage = $"Installer install folder {folderForDataFile} does not exists"
    //    };
    //}
}