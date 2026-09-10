//using System.Linq;
//using SystemTools.SharedKernel;

//namespace SystemTools.SystemToolsShared.Errors;

//public static class ErrorOmdExtensions
//{
//    public static Error ToError(this Error error)
//    {
//        return Error.Problem(error.Code, error.Name);
//    }

//    public static Error ToError(this Error[] errors)
//    {
//        return errors.Length == 1 ? errors[0].ToError() : new ValidationError([.. errors.Select(x => x.ToError())]);
//    }

//    // Reverse conversion for call sites that must keep producing the legacy
//    // Error[] wire format (BadRequest payloads, Result flows).
//    public static Error[] ToErrorOmdArray(this Error error)
//    {
//        return error is ValidationError validationError
//            ? [.. validationError.Errors.Select(x => new Error { Code = x.Code, Name = x.Description })]
//            : [new Error { Code = error.Code, Name = error.Description }];
//    }
//}


