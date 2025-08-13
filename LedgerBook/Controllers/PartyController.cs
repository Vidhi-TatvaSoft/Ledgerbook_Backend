using LedgerBook.Constant;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBook.Controllers;

public class PartyController : Controller
{
    [HttpGet]
    public IActionResult ManageBusiness()
    {
        string partyType = Request.Cookies[TokenKey.PartyType]!;
        ViewData["sidebar"] = partyType;
        return View();
    }

    [HttpGet]
    public IActionResult VerifyParty(string verificationCode)
    {
        return View("VerifyParty",verificationCode);
    }

}
