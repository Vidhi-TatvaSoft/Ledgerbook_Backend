using System.Security.Claims;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;

namespace BusinessAcessLayer.Services;

public class ExceptionService : IExceptionService
{
    private readonly IGenericRepo _genericRepository;
    public ExceptionService(IGenericRepo genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<bool> AddExceptionLog(HttpContext context, Exception exception)
    {
        ExceptionLogs exceptionLogs = new();
        exceptionLogs.ExceptionUrl = context.Request.Path;
        exceptionLogs.ExcceptionMessage = exception.Message;
        exceptionLogs.InnerException = exception.InnerException != null ? exception.InnerException.ToString() : exception.Message;
        exceptionLogs.UserId = GetUserId(context);
        exceptionLogs.ExceptionAt = DateTime.UtcNow;
        await _genericRepository.AddAsync<ExceptionLogs>(exceptionLogs);
        return true;
    }

    public int? GetUserId(HttpContext context)
    {
        Claim userClaim = context.User?.FindFirst("id");
        if (int.TryParse(userClaim?.Value, out int userId))
            return userId;
        return null;
    }
}
