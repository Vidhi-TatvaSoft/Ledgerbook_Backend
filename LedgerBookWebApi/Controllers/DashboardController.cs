using System.Net;
using BusinessAcessLayer.Interface;
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
       IBusinessService businessService,
       IPartyService partyService
       , IDashboardService dashboardService
    ) : base(loginService, businessService)
    {
        _partyService = partyService;
        _dashboardService = dashboardService;
    }

    [HttpGet]
    [Route("GetDashboardData")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetDashboardData()
    {
        Businesses business = GetBusinessFromToken();
        return Ok(_dashboardService.GetDashboardData(business.Id));
    }

    [HttpGet]
    [Route("GetGraphDetails")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetGraphDetails(string year = null)
    {
        Businesses business = GetBusinessFromToken();
        return Ok(new ApiResponse<List<decimal>>(true, null, _partyService.GetPartyRevenue(business.Id, year), HttpStatusCode.OK));
    }

    [HttpGet]
    [Route("GetYearsForRevenue")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetYearsForRevenue()
    {
        Businesses business = GetBusinessFromToken();
        return Ok(_dashboardService.Getyears(business.Id));
    }

}
