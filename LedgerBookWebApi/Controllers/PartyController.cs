using System.Net;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class PartyController : BaseController
{
    private readonly IUserBusinessMappingService _userBusinessMappingService;
    public PartyController(
       ILoginService loginService,
       IActivityLogService activityLogService,
       IBusinessService businessService,
       IUserBusinessMappingService userBusinessMappingService
    ) : base(loginService, activityLogService)
    {
        _userBusinessMappingService = userBusinessMappingService;
    }

    [HttpGet]
    [Route("CheckRolePermission")]
    public IActionResult CheckRolePermission()
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.Unauthorized));

        Businesses business = GetBusinessFromToken();
        if (business == null)
            return Ok(new ApiResponse<string>(false, null, null, HttpStatusCode.ServiceUnavailable));

        List<RoleViewModel> rolesByUser = _userBusinessMappingService.GetRolesByBusinessId(business.Id, user.Id);
        List<string> layout = new();
        if (rolesByUser.Any(role => role.RoleName == "Owner/Admin"))
        {
            layout.Add("Customers");
            layout.Add("Suppliers");
        }
        else if (rolesByUser.Any(role => role.RoleName == "Purchase Manager"))
            layout.Add("Suppliers");
        else if (rolesByUser.Any(role => role.RoleName == "Sales Manager"))
            layout.Add("Customers");
        else
            layout = new();
        return Ok();
    }
}
