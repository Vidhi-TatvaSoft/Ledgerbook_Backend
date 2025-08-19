using Microsoft.AspNetCore.Mvc;

namespace LedgerBook.Controllers;

public class ActivityLogController : Controller
{
    public IActionResult ActivityLog()
    {
        return View();   
    }
}
