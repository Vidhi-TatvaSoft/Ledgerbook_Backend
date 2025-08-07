using Microsoft.AspNetCore.Mvc;

namespace LedgerBook.Controllers;

public class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Dashboard()
    {
        ViewData["sidebar"] = "Dashboard";
        return View();
    }
}
