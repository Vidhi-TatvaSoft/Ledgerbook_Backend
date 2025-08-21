using System.Net;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class LoginController : BaseController
{
    public LoginController(
        ILoginService loginService
        ) : base(loginService)
    {
    }

    #region login 
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
    [Route("VerifyEmail/{verificationCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> VerifyEmail([FromRoute] string verificationCode)
    {
        if (verificationCode == null)
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        else
            return Ok(await _loginService.EmailVerification(verificationCode));
    }
    #endregion

    #region send email for forgot password
    [HttpPost]
    [Route("ForgotPassword/{email}")]
    public IActionResult ForgotPassword([FromRoute] string email)
    {
        if (email != null)
            return Ok(_loginService.ForgotPassword(email));
        return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
    }
    #endregion

    #region resetpassword - verify token and reset password
    [HttpGet]
    [Route("VerifyResetPasswordToken/{resetPasswordToken}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public IActionResult VerifyResetPasswordToken([FromRoute]string resetPasswordToken)
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
