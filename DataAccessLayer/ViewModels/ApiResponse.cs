using System.Net;

namespace DataAccessLayer.ViewModels;

public class ApiResponse<T>
{
    public HttpStatusCode HttpStatusCode { get; set; }

    public bool IsSuccess { get; set; } = true;

    public List<string> ErrorMessages { get; set; }

    public string ToasterMessage { get; set; }

    public T Result { get; set; }
}
