using System.Net;

namespace DataAccessLayer.ViewModels;

public class ApiResponse
{
    public HttpStatusCode httpStatusCode { get; set; }

    public bool IsSuccess { get; set; } = true;

    public List<string> ErrorMessage { get; set; }

    public Object Result { get; set; }
}
