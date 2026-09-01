using SystemTools.SharedKernel;

namespace SystemTools.SystemToolsShared;

public static class ErrorExtend
{
    public static void PrintErrorsOnConsole(this Error error)
    {
        if (error is ValidationError validationError)
        {
            foreach (Error err in validationError.Errors)
            {
                StShared.WriteErrorLine(err.Description, true, null, false);
            }
        }
        else
        {
            StShared.WriteErrorLine(error.Description, true, null, false);
        }
    }
}
