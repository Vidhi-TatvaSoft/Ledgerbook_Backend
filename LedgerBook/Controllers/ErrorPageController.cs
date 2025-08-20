using Microsoft.AspNetCore.Mvc;

namespace LedgerBook.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

public class ErrorPageController : Controller
{
    [Route("ErrorPage/PageNotFoundError")]
    public IActionResult PageNotFoundError()
    {
        return View();
    }

    [Route("ErrorPage/InternalServerError")]
    public IActionResult InternalServerError()
    {
        return View();
    }

    [Route("ErrorPage/HandleError/{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        switch (statusCode)
        {
            case 400:
                return View("BadRequest");
            case 401:
                return View("Unauthorize"); //dn
            case 403:
                return View("Forbidden");  //dn
            case 404:
                return View("PageNotFoundError"); //dn
            case 500:
                return View("InternalServerError");
            default:
                return View("GenericError");
        }
    }
}
