using System.Net;

namespace BusinessAcessLayer.CustomException;

public class BusinessNotFoundException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public BusinessNotFoundException(string message, HttpStatusCode statusCode = HttpStatusCode.ServiceUnavailable) : base(message)
    {
        StatusCode = statusCode;
    }
}
