using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.ViewModels;


namespace LedgerBook.ExceptionMiddleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ExceptionMiddleware(RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IServiceScopeFactory serviceScopeFactory
    )
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptions(context, ex);
        }
    }

    private async Task HandleExceptions(HttpContext context, Exception exception)
    {
        HttpStatusCode code;
        string message;

        code = context.Response.StatusCode switch
        {
            400 => HttpStatusCode.BadRequest,
            401 => HttpStatusCode.Unauthorized,
            404 => HttpStatusCode.NotFound,
            500 => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };
        message = Messages.ExceptionMessage;

        //add exception in db
        using IServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        IExceptionService exceptionserive = scope.ServiceProvider.GetRequiredService<IExceptionService>();
        await exceptionserive.AddExceptionLog(context, exception);

        context.Response.ContentType = "application/json";

        var jsonResponse = new ApiResponse<string>(false, message, null, code);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true 
        };
        string jsonMessage = JsonSerializer.Serialize(jsonResponse,options);
        await context.Response.WriteAsync(jsonMessage);
    }
}