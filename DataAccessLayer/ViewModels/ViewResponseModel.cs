namespace DataAccessLayer.ViewModels;

public class ViewResponseModel
{

    public ViewResponseModel()
    {

    }
    public ViewResponseModel(string html, string message, ErrorType errorType = ErrorType.Success)
    {
        HTML = html;
        Message = message;
        ErrorType = errorType;
    }

    public string HTML { get; set; }
    public string Message { get; set; }
    public ErrorType ErrorType { get; set; }
    public string RedirectUrl { get; set; }
}

public enum ErrorType
{
    Error,
    Success,
    Warning,
    Info
}