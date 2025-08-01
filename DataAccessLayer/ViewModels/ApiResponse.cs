using System.Net;

namespace DataAccessLayer.ViewModels;

public class ApiResponse<T>
{
    public HttpStatusCode? HttpStatusCode { get; set; }

    public bool? IsSuccess { get; set; } = false;

    public string? ToasterMessage { get; set; }

    public T? Result { get; set; }

    public ApiResponse(bool? isSuccess = false,  string? message = null, T? result = default,HttpStatusCode? statusCode = System.Net.HttpStatusCode.OK)
    {   
        IsSuccess = isSuccess;
        HttpStatusCode = statusCode;
        ToasterMessage = message;
        Result = result;
    }
}
