
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Helper;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using LedgerBookWebApi.Authorization;
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
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProfile()
    {
        ApplicationUser user = GetCurrentUserIdentity();
        UserProfileViewModel userProfileViewModel = _userService.GetUserProfile(user.Id);
        return Ok(new ApiResponse<UserProfileViewModel>(true, null, userProfileViewModel, HttpStatusCode.OK));
    }

    [HttpPost]
    [Route("UpdateProfile")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromForm] UserProfileViewModel userProfileViewModel)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (!ModelState.IsValid)
        {
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        }
        return Ok(await _userService.UpdateUserProfile(userProfileViewModel));
    }
    #endregion

    [HttpPost]
    [Route("ChangePassword")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordViewModel changePasswordViewModel)
    {
        ApplicationUser user = GetCurrentUserIdentity();;
        if (!ModelState.IsValid)
        {
            return Ok(new ApiResponse<string>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest));
        }
        return Ok(await _userService.ChangePasswordAsync(user, changePasswordViewModel));
    }
}
