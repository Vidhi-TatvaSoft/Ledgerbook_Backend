using System.Net;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using LedgerBook.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class PartyController : BaseController
{
    private readonly IPartyService _partyService;
    public PartyController(
       ILoginService loginService,
       IActivityLogService activityLogService,
       IBusinessService businessService,
       IPartyService partyService
    ) : base(loginService, activityLogService, businessService)
    {
        _partyService = partyService;
    }

    [HttpGet]
    [Route("CheckRolePermission")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult CheckRolePermission()
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.CheckRolepermission(business.Id,user.Id));
    }

    [HttpGet]
    [Route("GetAllParties")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetAllParties(string partyType, string searchText = "", string filter = "-1", string sort = "-1")
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetParties(partyType, business.Id,user.Id, searchText, filter, sort));
    }
}
