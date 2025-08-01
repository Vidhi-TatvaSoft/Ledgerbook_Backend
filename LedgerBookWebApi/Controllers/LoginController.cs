using System.Net;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login()
    {
        string token = GetData(TokenKey.UserToken);
        if (token.IsNullOrEmpty())
        {
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.BadRequest));
        }
        return Ok(_loginService.Login(token));
    }

    [HttpPost]
    [Route("LoginAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync([FromForm] LoginViewModel loginViewModel)
    {

        if (!ModelState.IsValid)
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        else
            return Ok(await _loginService.LoginAsync(loginViewModel));
    }
    #endregion

    #region register
    [Route("Register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public IActionResult VerifyResetPasswordToken(string resetPasswordToken)
    {
        if (resetPasswordToken == null)
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        else
            return Ok(_loginService.VerifyResetPasswordToken(resetPasswordToken));
    }

    [HttpPost]
    [Route("ResetPasswordAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPasswordViewModel resetPasswordViewModel)
    {
        if (!ModelState.IsValid)
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        else
            return Ok(await _loginService.ResetPassword(resetPasswordViewModel));
    }
    #endregion
}
