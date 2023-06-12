namespace SystemToolsShared;

public struct Err
{
    //[JsonProperty(PropertyName = "errorCode")]
    public string ErrorCode { get; set; }

    //[JsonProperty(PropertyName = "errorMessage")]
    public string ErrorMessage { get; set; }

    //public Err(string errorCode, string errorMessage)
    //{
    //    ErrorCode = errorCode;
    //    ErrorMessage = errorMessage;
    //}
}