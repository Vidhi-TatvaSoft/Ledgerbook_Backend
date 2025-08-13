using BusinessAcessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using BusinessAcessLayer.Constant;
using System.Net;
using BusinessAcessLayer.CustomException;
namespace LedgerBookWebApi.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{

    private readonly IJWTTokenService _jWTService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILoginService _loginService;
    private readonly IBusinessService _businessService;
    private readonly IUserBusinessMappingService _userBusinessMappingService;

    public PermissionHandler(IJWTTokenService jWTService,
     IHttpContextAccessor httpContextAccessor, ILoginService loginService,
      IBusinessService businessService, IUserBusinessMappingService userBusinessMappingService)
        : base()
    {
        this._jWTService = jWTService;
        this._httpContextAccessor = httpContextAccessor;
        this._loginService = loginService;
        this._businessService = businessService;
        this._userBusinessMappingService = userBusinessMappingService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var usertoken = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(usertoken))
        {
            throw new UnauthorizedAccessException();
        }
        ApplicationUser user = _loginService.GetUserFromTokenIdentity(usertoken);
        if (user == null)
        {
            throw new UnauthorizedAccessException();
        }
        string businessToken = httpContext.Request.Headers[TokenKey.BusinessToken];
        if (requirement.Permission != "User")
        {
            if (string.IsNullOrEmpty(businessToken))
            {
                throw new BusinessNotFoundException("Business Token Not found.");
            }
            Businesses business = _businessService.GetBusinessFromToken(businessToken);
            if (business == null)
            {
                throw new BusinessNotFoundException("Business Token Not found.");
            }
            List<RoleViewModel> rolesByUser = _userBusinessMappingService.GetRolesByBusinessId(business.Id, user.Id);
            switch (requirement.Permission)
            {
                case "Owner/Admin":
                    if (rolesByUser.Any(role => role.RoleName == "Owner/Admin"))
                    {
                        context.Succeed(requirement);
                    }
                    break;
                case "PurchaseManager":
                    if (rolesByUser.Any(role => role.RoleName == "Purchase Manager" || role.RoleName == "Owner/Admin"))
                    {
                        context.Succeed(requirement);
                    }
                    break;
                case "SalesManager":
                    if (rolesByUser.Any(role => role.RoleName == "Sales Manager" || role.RoleName == "Owner/Admin"))
                    {
                        context.Succeed(requirement);
                    }
                    break;
                case "AnyRole":
                    if (rolesByUser.Any(role => role.RoleName == "Sales Manager" || role.RoleName == "Owner/Admin" || role.RoleName == "Purchase Manager"))
                    {
                        context.Succeed(requirement);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (requirement.Permission)
            {
                case "User":
                    context.Succeed(requirement);
                    break;

            }
        }

        string email = _jWTService.GetClaimValue(usertoken, "email");
        if (string.IsNullOrEmpty(email))
        {
            httpContext.Response.Redirect("/Login/Login");
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}