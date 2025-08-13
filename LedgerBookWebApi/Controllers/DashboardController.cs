using System.Net;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using LedgerBookWebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class DashboardController : BaseController
{
    private readonly IPartyService _partyService;
    private readonly IDashboardService _dashboardService;
    public DashboardController(
       ILoginService loginService,
       IActivityLogService activityLogService,
       IBusinessService businessService,
       IPartyService partyService
       , IDashboardService dashboardService
    ) : base(loginService, activityLogService, businessService)
    {
        _partyService = partyService;
        _dashboardService = dashboardService;
    }

    [HttpGet]
    [Route("GetDashboardData")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetDashboardData()
    {
        Businesses business = GetBusinessFromToken();
        return Ok(_dashboardService.GetDashboardData(business.Id));
    }
    
    [HttpGet]
    [Route("GetGraphDetails")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetGraphDetails(string year = null)
    {
        Businesses business = GetBusinessFromToken();
        return Ok(new ApiResponse<List<decimal>>(true, null, _partyService.GetPartyRevenue(business.Id, year), HttpStatusCode.OK));
    }

    [HttpGet]
    [Route("GetYearsForRevenue")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetYearsForRevenue()
    {
        Businesses business = GetBusinessFromToken();
        return Ok(_dashboardService.Getyears(business.Id));
    }


}
