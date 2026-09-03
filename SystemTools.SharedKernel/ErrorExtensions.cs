namespace SystemTools.SharedKernel;

public static class ErrorExtensions
{
    //BadRequest-ის პასუხში შეცდომები ყოველთვის მასივის სახით იგზავნება, რომ ApiClient-მა ერთნაირად წაიკითხოს
    public static Error[] ToErrorArray(this Error error)
    {
        return error is ValidationError validationError ? validationError.Errors : [error];
    }
}
