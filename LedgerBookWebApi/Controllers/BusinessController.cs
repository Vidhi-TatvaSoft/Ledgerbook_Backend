using System.Net;
using System.Threading.Tasks;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class BusinessController : BaseController
{
    private readonly IBusinessService _businessService;
    public BusinessController(
        ILoginService loginService,
        IActivityLogService activityLogService,
        IBusinessService businessService
    ) : base(loginService, activityLogService)
    {
        _businessService = businessService;
    }

    #region get all businesses
    [HttpGet]
    [Route("GetBusinesses")]
    public IActionResult GetBusinesses(string searchText = null)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        return Ok(_businessService.GetRolewiseBusiness(user.Id, searchText));
    }
    #endregion

    [HttpGet]
    [Route("GetBusinessDetails")]
    public IActionResult GetBusinessDetails(int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        return Ok(_businessService.GetBusinessItemById(businessId));
    }

    [HttpPost]
    [Route("SaveBusiness")]
    public async Task<IActionResult> SaveBusiness([FromForm] BusinessItem businessItem)
    {
        // return Ok(false);
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        return Ok(await _businessService.SaveBusiness(businessItem, user.Id));
    }

    [HttpGet]
    [Route("GetAllUsersOfBusiness")]
    public async Task<IActionResult> GetAllUsersOfBusiness(int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        return Ok(await _businessService.GetUsersOfBusiness(businessId, user.Id));
    }

    [HttpGet]
    [Route("GetuserDetailById")]
    public async Task<IActionResult> GetuserDetailById(int businessId, int? userId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        return Ok(await _businessService.GetUserById(businessId, user.Id, userId));
    }

    [HttpPost]
    [Route("SaveUserDetails")]
    public async Task<IActionResult> SaveUserDetails([FromBody] UserDetailsViewModel userDetailsViewModel)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        if (!ModelState.IsValid)
        {
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.BadRequest));
        }
        return Ok(await _businessService.SaveUserDetails(userDetailsViewModel.UserDetails, userDetailsViewModel.SelectedRoles, userDetailsViewModel.BusinessId, user.Id));
    }

    #region Delete user from business
    [HttpGet]
    [Route("DeleteUserFromBusiness")]
    public async Task<IActionResult> DeleteUserFromBusiness(int userId, int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        return Ok(await _businessService.DeleteUserFromBusiness(userId, businessId, user));
    }
    #endregion

    #region inactive / active use
    [HttpGet]
    [Route("ActiveInactiveUser")]
    public async Task<IActionResult> ActiveInactiveUser(int userId, bool isActive, int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        return Ok(await _businessService.ActiveInactiveUser(userId, isActive, businessId, user));
    }
    #endregion

    #region delete business
    [HttpPost]
    [Route("DeleteBusiness")]
    public async Task<IActionResult> DeleteBusiness([FromForm]int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Forbidden));
        return Ok(await _businessService.DeleteBusiness(businessId, user.Id));
    }
    #endregion
}
