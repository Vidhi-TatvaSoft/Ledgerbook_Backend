using LedgerBook.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBook.Controllers;

public class LoginController : Controller
{

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult VerifyEmail(string verificationCode)
    {
        return View("VerifyEmail", verificationCode);
    }

    [HttpGet]
    public IActionResult ResetPassword(string resetPasswordToken)
    {
        return View("ResetPassword", resetPasswordToken);
    }

    // [HttpGet]
    // public IActionResult LoginForm()
    // {
    //     return View();
    // }

    // [HttpPost]
    // public IActionResult Login(LoginMain loginMain)
    // {
    //     return View();
    // }
}
