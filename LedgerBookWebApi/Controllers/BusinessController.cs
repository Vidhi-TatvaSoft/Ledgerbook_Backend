using System.Net;
using System.Threading.Tasks;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using LedgerBookWebApi.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class BusinessController : BaseController
{
    private readonly IBusinessService _businessService;
    private readonly IJWTTokenService _jwttokenService;
    public BusinessController(
        ILoginService loginService,
        IActivityLogService activityLogService,
        IBusinessService businessService,
        IJWTTokenService jWTTokenService
    ) : base(loginService, activityLogService)
    {
        _businessService = businessService;
        _jwttokenService = jWTTokenService;
    }

    #region get all businesses
    [HttpGet]
    [Route("GetBusinesses")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetBusinesses(string searchText = null)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(_businessService.GetRolewiseBusiness(user.Id, searchText));
    }
    #endregion

    [HttpGet]
    [Route("GetBusinessDetails")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    public IActionResult GetBusinessDetails(int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(_businessService.GetBusinessItemById(businessId));
    }

    [HttpPost]
    [Route("SaveBusiness")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SaveBusiness([FromForm] BusinessItem businessItem)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(await _businessService.SaveBusiness(businessItem, user.Id));
    }

    [HttpGet]
    [Route("GetAllUsersOfBusiness")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsersOfBusiness(int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(await _businessService.GetUsersOfBusiness(businessId, user.Id));
    }

    [HttpGet]
    [Route("GetuserDetailById")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetuserDetailById(int businessId, int? userId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(await _businessService.GetUserById(businessId, user.Id, userId));
    }

    [HttpPost]
    [Route("SaveUserDetails")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SaveUserDetails([FromBody] UserDetailsViewModel userDetailsViewModel)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (!ModelState.IsValid)
        {
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.BadRequest));
        }
        return Ok(await _businessService.SaveUserDetails(userDetailsViewModel.UserDetails, userDetailsViewModel.SelectedRoles, userDetailsViewModel.BusinessId, user.Id));
    }

    #region Delete user from business
    [HttpGet]
    [Route("DeleteUserFromBusiness")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUserFromBusiness(int userId, int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(await _businessService.DeleteUserFromBusiness(userId, businessId, user));
    }
    #endregion

    #region inactive / active use
    [HttpGet]
    [Route("ActiveInactiveUser")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ActiveInactiveUser(int userId, bool isActive, int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(await _businessService.ActiveInactiveUser(userId, isActive, businessId, user));
    }
    #endregion

    #region delete business
    [HttpPost]
    [Route("DeleteBusiness")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteBusiness([FromForm] int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(await _businessService.DeleteBusiness(businessId, user.Id));
    }
    #endregion

    #region getBusinessCookies
    [HttpGet]
    [Route("GetBusinessData")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetBusinessData(int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(_businessService.GetBusinessData(businessId, user));
    }
    #endregion
}
