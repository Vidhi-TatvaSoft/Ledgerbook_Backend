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

    #region login get method
    [HttpGet]
    [Route("Login")]
    public async Task<IActionResult> Login()
    {
        ApiResponse<CookiesViewModel> apiResponse = new();
        CookiesViewModel cookiesViewModel = new();
        string token = GetData(TokenKey.UserToken);
        if (token != null)
        {
            ApplicationUser user = _loginService.GetUserFromTokenIdentity(token);
            if (user == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
                return Ok(apiResponse);
            }
            else
            {
                if (user.ProfileAttachmentId != null)
                {
                    AttachmentViewModel attachmentViewModel = _attachmentService.GetAttachmentById((int)user.ProfileAttachmentId);
                    cookiesViewModel.ProfilePhoto = attachmentViewModel.BusinesLogoPath;
                }
                cookiesViewModel.UserName = user.FirstName + " " + user.LastName;
                apiResponse.IsSuccess = true;
                apiResponse.Result = cookiesViewModel;
                return Ok(apiResponse);
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
        ApiResponse<string> apiResponse = new();
        if (RegisterVM.Email == null || RegisterVM.Password == null)
        {
            apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
            apiResponse.ToasterMessage = Messages.InvalidCredentilMessage;
            apiResponse.IsSuccess = false;
            return Ok(apiResponse);
        }
        else
        {
            if (_loginService.IsEmailExist(RegisterVM.Email) && _userService.IsUserRegistered(RegisterVM.Email))
            {
                apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                apiResponse.ToasterMessage = Messages.EmailExistMessage;
                apiResponse.IsSuccess = false;
                return Ok(apiResponse);
            }
            else
            {
                if (await _loginService.SaveUser(RegisterVM))
                {
                    string verificationToken = _loginService.GetEmailVerifiactionToken(RegisterVM.Email);
                    string verificationCode = _jwtTokenService.GenerateTokenEmailVerificationToken(RegisterVM.Email, verificationToken);
                    string verificationLink = "http://localhost:5189/Login/VerifyEmail?verificationCode=" + verificationCode;
                    _ = CommonMethods.RegisterEmail(RegisterVM.FirstName + " " + RegisterVM.LastName, RegisterVM.Email, verificationLink, ConstantVariables.LoginLink);
                    apiResponse.HttpStatusCode = HttpStatusCode.Created;
                    apiResponse.ToasterMessage = Messages.RegistrationSuccessMessage;
                    apiResponse.Result = verificationToken;
                    apiResponse.IsSuccess = true;
                    return Ok(apiResponse);
                }
                else
                {
                    apiResponse.HttpStatusCode = HttpStatusCode.BadRequest;
                    apiResponse.ToasterMessage = Messages.ExceptionMessage;
                    apiResponse.IsSuccess = false;
                    return Ok(apiResponse);
                }
            }
        }
    }
    #endregion

    #region verify email
    [Route("VerifyEmail")]
    [HttpPost]
    public async Task<IActionResult> VerifyEmail([FromForm] string verificationCode)
    {
        string email = _jwtTokenService.GetClaimValue(verificationCode, "email")!;
        string emailToken = _jwtTokenService.GetClaimValue(verificationCode, "token")!;
        bool isEmailVerified = await _loginService.EmailVerification(email, emailToken);
        if (isEmailVerified)
        {
            ApiResponse<string> apiResponse = new()
            {
                IsSuccess = true,
                ToasterMessage = Messages.VerificationSuccessMessage
            };
            return Ok(apiResponse);
        }
        else
        {
            ApiResponse<string> apiResponse = new()
            {
                IsSuccess = false,
                ToasterMessage = Messages.VerificationErrorMessage
            };
            return Ok(apiResponse);
        }
    }
    #endregion

    #region Login
    [HttpPost]
    [Route("LoginAsync")]
    public async Task<IActionResult> LoginAsync([FromForm] LoginViewModel loginViewModel)
    {
        ApiResponse<CookiesViewModel> apiResponse = new();
        CookiesViewModel cookiesViewModel = new();
        if (!ModelState.IsValid)
        {
            apiResponse.IsSuccess = false;
            apiResponse.ToasterMessage = Messages.InvalidCredentilMessage;
        }
        else
        {
            if (!_loginService.IsEmailExist(loginViewModel.Email))
            {
                apiResponse.IsSuccess = false;
                apiResponse.ToasterMessage = Messages.EmailDoesNotExistMessage;
            }
            else if (!_userService.IsUserRegistered(loginViewModel.Email))
            {
                apiResponse.IsSuccess = false;
                apiResponse.ToasterMessage = Messages.EmailDoesNotExistMessage;
            }
            else
            {
                if (!_loginService.IsEmailVerified(loginViewModel.Email))
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.ToasterMessage = Messages.NotVerifiedEmailMessae;
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
                            apiResponse.IsSuccess = false;
                            apiResponse.ToasterMessage = Messages.InvalidCredentilMessage;
                            return Ok(apiResponse);
                        }
                        else
                        {
                            if (user.ProfileAttachmentId != null)
                            {
                                AttachmentViewModel attachmentViewModel = _attachmentService.GetAttachmentById((int)user.ProfileAttachmentId);
                                cookiesViewModel.ProfilePhoto = attachmentViewModel.BusinesLogoPath;
                                // _cookieService.SetCookie(Response, TokenKey.ProfilePhoto, attachmentViewModel.BusinesLogoPath);
                            }
                            cookiesViewModel.UserName = user.FirstName + " " + user.LastName;
                            // _cookieService.SetCookie(Response, TokenKey.UserName, user.FirstName + " " + user.LastName);
                            apiResponse.IsSuccess = true;
                            apiResponse.Result = cookiesViewModel;
                        }
                    }
                    else
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.ToasterMessage = Messages.InvalidCredentilMessage;
                    }
                }
            }
        }
        return Ok(apiResponse);
    }
    #endregion 

}
