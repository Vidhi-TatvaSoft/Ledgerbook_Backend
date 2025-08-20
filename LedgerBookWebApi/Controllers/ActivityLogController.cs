using System.Net;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using LedgerBookWebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class ActivityLogController : BaseController
{
    private readonly IActivityLogService _activityLogService;
    private readonly IBusinessService _businessService;
    private readonly IPartyService _partyService;

    public ActivityLogController(
        ILoginService loginService,
        IActivityLogService activityLogService,
        IBusinessService businessService,
        IPartyService partyService
    ) : base(loginService, activityLogService)
    {
        _activityLogService = activityLogService;
        _businessService = businessService;
        _partyService = partyService;
    }

    #region get activities
    [HttpPost]
    [Route("GetActivities")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetActivities([FromBody] ActivityDataViewModel activityData)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(_activityLogService.GetActivities(activityData, user.Id));
    }
    #endregion

    #region get all bsuiness of user
    [HttpGet]
    [Route("GetAllBusiness")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllBusiness()
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(new ApiResponse<List<BusinessViewModel>>(true, null, _businessService.GetAllBusinesses(user.Id), HttpStatusCode.OK));
    }
    #endregion

    #region get all parties by business
    [HttpGet]
    [Route("GetAllParties")]
    [PermissionAuthorize("User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllParties(int businessId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(new ApiResponse<List<Parties>>(true, null, _partyService.GetAllPartiesByBusiness(businessId, user.Id), HttpStatusCode.OK));
    }
    #endregion
}
