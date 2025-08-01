
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        return Ok(new ApiResponse<UserProfileViewModel>(false, null, userProfileViewModel, HttpStatusCode.Forbidden));
    }
    #endregion

}
