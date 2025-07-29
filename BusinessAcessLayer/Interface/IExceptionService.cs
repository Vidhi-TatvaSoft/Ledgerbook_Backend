using Microsoft.AspNetCore.Http;

namespace BusinessAcessLayer.Interface;

public interface IExceptionService
{
    Task<bool> AddExceptionLog(HttpContext context, Exception exception);
    int? GetUserId(HttpContext context);
}
