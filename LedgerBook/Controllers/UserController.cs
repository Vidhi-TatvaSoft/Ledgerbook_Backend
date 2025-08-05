using Microsoft.AspNetCore.Mvc;

namespace LedgerBook.Controllers;

public class UserController : Controller
{
    [HttpGet]
    public IActionResult Profile()
    {
        return View();
    }
}
