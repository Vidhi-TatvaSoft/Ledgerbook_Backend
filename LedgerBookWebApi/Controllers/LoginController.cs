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
    #endregion

    #region register
    [Route("Register")]
    [HttpPost]
    public async Task<IActionResult> Registration([FromForm] RegistrationViewModel RegisterVM)
    {
        if (RegisterVM.Email == null || RegisterVM.Password == null)
        {
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        }
        else
        {
            if (_loginService.IsEmailExist(RegisterVM.Email) && _userService.IsUserRegistered(RegisterVM.Email))
            {
                return Ok(new ApiResponse<string>(false, Messages.EmailExistMessage, null, HttpStatusCode.BadRequest));
            }
            else
            {
                if (await _loginService.SaveUser(RegisterVM))
                {
                    string verificationToken = _loginService.GetEmailVerifiactionToken(RegisterVM.Email);
                    string verificationCode = _jwtTokenService.GenerateTokenEmailVerificationToken(RegisterVM.Email, verificationToken);
                    string verificationLink = "http://localhost:5189/Login/VerifyEmail?verificationCode=" + verificationCode;
                    _ = CommonMethods.RegisterEmail(RegisterVM.FirstName + " " + RegisterVM.LastName, RegisterVM.Email, verificationLink, ConstantVariables.LoginLink);
                    return Ok(new ApiResponse<string>(true, Messages.RegistrationSuccessMessage, verificationToken, HttpStatusCode.Created));
                }
                else
                {
                    return Ok(new ApiResponse<string>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest));
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
            return Ok(new ApiResponse<string>(true, Messages.VerificationSuccessMessage, null, HttpStatusCode.OK));
        }
        else
        {
            ApiResponse<string> apiResponse = new()
            {
                IsSuccess = false,
                ToasterMessage = Messages.VerificationErrorMessage
            };
            return Ok(new ApiResponse<string>(false, Messages.VerificationErrorMessage, null, HttpStatusCode.BadRequest));
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

    #region forgot password post
    [HttpPost]
    [Route("ForgotPassword")]
    public IActionResult ForgotPassword([FromForm] string email)
    {
        if (email != null)
        {
            if (_loginService.IsEmailExist(email))
            {
                if (_userService.IsUserRegistered(email))
                {
                    //send email
                    ApplicationUser user = _userService.GetuserByEmail(email);
                    string username = user.FirstName + " " + user.LastName;
                    string resetPasswordToken = _jwtTokenService.GenerateTokenEmailPassword(email, user.PasswordHash);
                    string resetLink = ConstantVariables.LoginLink + "/Login/ResetPassword?resetPasswordToken=" + resetPasswordToken;
                    _ = CommonMethods.ResetPasswordEmail(email, username, resetLink, ConstantVariables.LoginLink);
                    return Ok(new ApiResponse<string>(true, Messages.SendResetPasswordMailSuccess, null, HttpStatusCode.OK));
                }
                else
                {
                    return Ok(new ApiResponse<string>(false, Messages.EmailDoesNotExistMessage, null, HttpStatusCode.BadRequest));
                }
            }
            else
            {
                return Ok(new ApiResponse<string>(false, Messages.EmailDoesNotExistMessage, null, HttpStatusCode.BadRequest));
            }
        }
        return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
    }
    #endregion

    #region resetpassword get
    [HttpGet]
    [Route("ResetPassword")]
    public IActionResult ResetPassword(string resetPasswordToken)
    {
        // ApiResponse<string> apiResponse = new();
        try
        {
            string email = _jwtTokenService.GetClaimValue(resetPasswordToken, "email")!;
            string newpassword = _jwtTokenService.GetClaimValue(resetPasswordToken, "password")!;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newpassword) || !_loginService.IsEmailExist(email))
            {
                return Ok(new ApiResponse<string>(false, Messages.InvalidResetPasswordLink, null, HttpStatusCode.BadRequest));
            }
            ApplicationUser user = _userService.GetuserByEmail(email);
            string savedPassword = user.PasswordHash!;

            if (savedPassword == newpassword)
            {
                return Ok(new ApiResponse<string>(true, null, email, HttpStatusCode.BadRequest));
            }
            return Ok(new ApiResponse<string>(false, Messages.LinkAlreadyUsedMessage, null, HttpStatusCode.BadRequest));
        }
        catch (Exception e)
        {
            return Ok(new ApiResponse<string>(false, Messages.InvalidResetPasswordLink, null, HttpStatusCode.BadRequest));

        }
    }
    #endregion

    #region reset password post
    [HttpPost]
    [Route("ResetPasswordAsync")]
    public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPasswordViewModel resetPasswordViewModel)
    {
        if (!ModelState.IsValid)
        {
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        }
        else
        {
            ApplicationUser user = _userService.GetuserByEmail(resetPasswordViewModel.Email);
            if (user != null)
            {
                PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
                PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, resetPasswordViewModel.Password);
                if (result != PasswordVerificationResult.Failed)
                {
                    // apiResponse.IsSuccess = false;
                    // TempData["ErrorMessage"] = Messages.SamePasswordsErrorMessage;
                    return Ok(new ApiResponse<string>(false, Messages.SamePasswordsErrorMessage, null, HttpStatusCode.BadRequest));
                }
                else
                {
                    bool IsPasswordUpdated = await _userService.UpdatePassword(resetPasswordViewModel);
                    if (IsPasswordUpdated)
                    {
                        return Ok(new ApiResponse<string>(true, string.Format(Messages.GlobalAddUpdateMesage, "Password", "updated"), null, HttpStatusCode.OK));
                    }
                    else
                    {
                        return Ok(new ApiResponse<string>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "update", "Password"), null, HttpStatusCode.BadRequest));
                    }
                }
            }
            else
            {
                return Ok(new ApiResponse<string>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest));
            }
        }
    }
    #endregion
}
