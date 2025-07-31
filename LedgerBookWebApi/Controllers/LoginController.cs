using System.Net;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Helper;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/Login")]
public class LoginController : BaseController
{
    // private readonly ILoginService _loginService;
    private readonly IUserService _userService;
    private readonly IJWTTokenService _jwtTokenService;
    private readonly ICookieService _cookieService;
    private readonly IAttachmentService _attachmentService;

    public LoginController(
        IUserService userService,
        IJWTTokenService jWTTokenService,
        ICookieService cookieService,
        ILoginService loginService,
        IActivityLogService activityLogService,
        IAttachmentService attachmentService) : base(loginService, activityLogService)

    {
        _userService = userService;
        _jwtTokenService = jWTTokenService;
        _cookieService = cookieService;
        _attachmentService = attachmentService;
        // _loginService = loginService;
    }

    #region login 
    [HttpGet]
    [Route("Login")]
    public async Task<IActionResult> Login()
    {
        // ApiResponse<CookiesViewModel> apiResponse = new();
        CookiesViewModel cookiesViewModel = new();
        string token = GetData(TokenKey.UserToken);
        if (token != null)
        {
            ApplicationUser user = _loginService.GetUserFromTokenIdentity(token);
            if (user == null)
            {
                return Ok(new ApiResponse<string>(false));
            }
            else
            {
                if (user.ProfileAttachmentId != null)
                {
                    AttachmentViewModel attachmentViewModel = _attachmentService.GetAttachmentById((int)user.ProfileAttachmentId);
                    cookiesViewModel.ProfilePhoto = attachmentViewModel.BusinesLogoPath;
                }
                cookiesViewModel.UserName = user.FirstName + " " + user.LastName;
                return Ok(new ApiResponse<CookiesViewModel>(true, result: cookiesViewModel));
            }
        }
        return Ok(new ApiResponse<CookiesViewModel>(false));
    }

    [HttpPost]
    [Route("LoginAsync")]
    public async Task<IActionResult> LoginAsync([FromForm] LoginViewModel loginViewModel)
    {
        ApiResponse<CookiesViewModel> apiResponse = new();
        CookiesViewModel cookiesViewModel = new();
        if (!ModelState.IsValid)
        {
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        }
        else
        {
            if (!_loginService.IsEmailExist(loginViewModel.Email))
            {
                return Ok(new ApiResponse<string>(false, Messages.EmailDoesNotExistMessage, null, HttpStatusCode.BadRequest));
            }
            else if (!_userService.IsUserRegistered(loginViewModel.Email))
            {
                return Ok(new ApiResponse<string>(false, Messages.EmailDoesNotExistMessage, null, HttpStatusCode.BadRequest));
            }
            else
            {
                if (!_loginService.IsEmailVerified(loginViewModel.Email))
                {
                    return Ok(new ApiResponse<string>(false, Messages.NotVerifiedEmailMessae, null, HttpStatusCode.BadRequest));
                }
                else
                {
                    string verificaitonToken = await _loginService.VerifyPassword(loginViewModel);
                    if (verificaitonToken != null)
                    {
                        cookiesViewModel.UserToken = verificaitonToken;
                        // _cookieService.SetCookie(Response, TokenKey.UserToken, verificaitonToken);
                        ApplicationUser user = _loginService.GetUserFromTokenIdentity(verificaitonToken);
                        if (user == null)
                        {
                            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
                        }
                        else
                        {
                            if (user.ProfileAttachmentId != null)
                            {
                                AttachmentViewModel attachmentViewModel = _attachmentService.GetAttachmentById((int)user.ProfileAttachmentId);
                                cookiesViewModel.ProfilePhoto = attachmentViewModel.BusinesLogoPath;
                            }
                            cookiesViewModel.UserName = user.FirstName + " " + user.LastName;
                            return Ok(new ApiResponse<CookiesViewModel>(true, null, cookiesViewModel, HttpStatusCode.OK));
                        }
                    }
                    else
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.ToasterMessage = Messages.InvalidCredentilMessage;
                        return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
                    }
                }
            }
        }
        return Ok(apiResponse);
    }
    #endregion

    #region register
    [Route("Register")]
    [HttpPost]
    public async Task<IActionResult> Registration([FromForm] RegistrationViewModel RegisterVM)
    {
        if (!ModelState.IsValid)
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        else
            return Ok(await _loginService.RegisterUser(RegisterVM));
    }
    #endregion

    #region verify email
    [Route("VerifyEmail")]
    [HttpPost]
    public async Task<IActionResult> VerifyEmail([FromForm] string verificationCode)
    {
        if (verificationCode == null)
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        else
            return Ok(await _loginService.EmailVerification(verificationCode));
    }
    #endregion


    #region send email for forgot password
    [HttpPost]
    [Route("ForgotPassword")]
    public IActionResult ForgotPassword([FromForm] string email)
    {
        if (email != null)
            return Ok(_loginService.ForgotPassword(email));
        return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
    }
    #endregion

    #region resetpassword 
    [HttpGet]
    [Route("VerifyResetPasswordToken")]
    public IActionResult VerifyResetPasswordToken(string resetPasswordToken)
    {
        if (resetPasswordToken == null)
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        else
            return Ok(_loginService.VerifyResetPasswordToken(resetPasswordToken));
    }

    [HttpPost]
    [Route("ResetPasswordAsync")]
    public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPasswordViewModel resetPasswordViewModel)
    {
        if (!ModelState.IsValid)
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        else
            return Ok(_loginService.ResetPassword(resetPasswordViewModel));
    }
    #endregion
}
