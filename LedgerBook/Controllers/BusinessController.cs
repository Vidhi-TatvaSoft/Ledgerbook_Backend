using Microsoft.AspNetCore.Mvc;

namespace LedgerBook.Controllers;

public class BusinessController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
