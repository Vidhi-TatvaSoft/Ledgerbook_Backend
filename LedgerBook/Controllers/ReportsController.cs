using Microsoft.AspNetCore.Mvc;

namespace LedgerBook.Controllers;

public class ReportsController : Controller
{
    [HttpGet]
    public IActionResult Reports()
    {
        ViewData["report"] = "transaction";
         ViewData["sidebar"] = "Reports";
        return View();
    }
}
