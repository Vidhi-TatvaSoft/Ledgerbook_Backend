using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;


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

        // _logger.LogError(exception, Messages.UnhandledExceptionMessage);

        bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        //add exception in db
        using IServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        IExceptionService exceptionserive = scope.ServiceProvider.GetRequiredService<IExceptionService>();
        await exceptionserive.AddExceptionLog(context, exception);

        if (isAjax)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200; // Always OK (to avoid redirect issues)

            context.Response.Headers.Add("X-Error", "true");

            var jsonResponse = new
            {
                success = false,
                statusCode = (int)code,
                error = message
            };

            string jsonMessage = JsonSerializer.Serialize(jsonResponse);
            await context.Response.WriteAsync(jsonMessage);
        }
        else
        {
            // if (!context.Response.HasStarted)
            // {
            //     string redirectUrl = $"/ErrorPage/HandleError/{(int)code}";
            //     context.Response.StatusCode = (int)HttpStatusCode.Redirect;
            //     context.Response.Headers["Location"] = redirectUrl;
            //     await context.Response.CompleteAsync();
            // }
            // else
            // {
            //     _logger.LogWarning(Messages.ResponseStartedLogWarning);
            //     context.Response.StatusCode = (int)code;
            //     await context.Response.WriteAsync(message);
            // }
        }
    }
}