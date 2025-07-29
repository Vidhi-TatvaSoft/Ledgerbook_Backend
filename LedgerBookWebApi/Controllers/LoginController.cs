using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Helper;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/Login/")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IUserService _userService;
    private readonly IJWTTokenService _jwtTokenService;
    private readonly ICookieService _cookieService;
    private readonly IAttachmentService _attachmentService;
    public LoginController(
        IUserService userService,
        IJWTTokenService jWTTokenService,
        ICookieService cookieService,
        ILoginService loginService,
        IAttachmentService attachmentService)

    {
        _userService = userService;
        _jwtTokenService = jWTTokenService;
        _cookieService = cookieService;
        _attachmentService = attachmentService;
        _loginService = loginService;
    }

    #region login get method
    [HttpGet]
    public async Task<IActionResult> Login()
    {
        string token = Request.Cookies[TokenKey.UserToken];
        if (token != null)
        {
            ApplicationUser user = _loginService.GetUserFromTokenIdentity(token);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                if (user.ProfileAttachmentId != null)
                {
                    AttachmentViewModel attachmentViewModel = _attachmentService.GetAttachmentById((int)user.ProfileAttachmentId);
                    _cookieService.SetCookie(Response, TokenKey.ProfilePhoto, attachmentViewModel.BusinesLogoPath);
                }
                _cookieService.SetCookie(Response, TokenKey.UserName, user.FirstName + " " + user.LastName);
                return Ok();
            }
        }
        return NotFound();
    }
    #endregion

}
