using Microsoft.AspNetCore.Mvc;

namespace LedgerBook.Controllers;

public class PartyController : Controller
{
    [HttpGet]
    public IActionResult ManageBusiness()
    {
        // ViewData["sidebar"] = "Dashboard";
        return View();
    }

    public IActionResult Customers()
    {
        ViewData["sidebar"] = "Customers";
        return View();
    }

    public IActionResult Suppliers()
    {
        ViewData["sidebar"] = "Suppliers";
        return View();
    }
}
