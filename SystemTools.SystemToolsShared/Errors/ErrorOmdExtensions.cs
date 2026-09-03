//using System.Linq;
//using SystemTools.SharedKernel;

//namespace SystemTools.SystemToolsShared.Errors;

//public static class ErrorOmdExtensions
//{
//    public static Error ToError(this ErrorOmd error)
//    {
//        return Error.Problem(error.Code, error.Name);
//    }

//    public static Error ToError(this ErrorOmd[] errors)
//    {
//        return errors.Length == 1 ? errors[0].ToError() : new ValidationError([.. errors.Select(x => x.ToError())]);
//    }

//    // Reverse conversion for call sites that must keep producing the legacy
//    // ErrorOmd[] wire format (BadRequest payloads, Result flows).
//    public static ErrorOmd[] ToErrorOmdArray(this Error error)
//    {
//        return error is ValidationError validationError
//            ? [.. validationError.Errors.Select(x => new ErrorOmd { Code = x.Code, Name = x.Description })]
//            : [new ErrorOmd { Code = error.Code, Name = error.Description }];
//    }
//}


