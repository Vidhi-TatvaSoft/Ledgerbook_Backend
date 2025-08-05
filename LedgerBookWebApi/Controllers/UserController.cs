
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Helper;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    public UserController(
       ILoginService loginService,
       IActivityLogService activityLogService,
       IUserService userService
    ) : base(loginService, activityLogService)
    {
        _userService = userService;
    }

    #region profile index page
    [HttpGet]
    [Route("GetProfile")]
    public IActionResult GetProfile()
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        UserProfileViewModel userProfileViewModel = _userService.GetUserProfile(user.Id);
        return Ok(new ApiResponse<UserProfileViewModel>(true, null, userProfileViewModel, HttpStatusCode.OK));
    }

    [HttpPost]
    [Route("UpdateProfile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UserProfileViewModel userProfileViewModel)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        if (!ModelState.IsValid)
        {
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        }
        return Ok(await _userService.UpdateUserProfile(userProfileViewModel));
    }
    #endregion

    [HttpGet]
    [Route("getusers")]
    public IActionResult getusers()
    {
        return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.OK));
    }

    [HttpPost]
    [Route("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordViewModel changePasswordViewModel)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        if (!ModelState.IsValid)
        {
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        }
        return Ok(await _userService.ChangePasswordAsync(user, changePasswordViewModel));
    }
}
