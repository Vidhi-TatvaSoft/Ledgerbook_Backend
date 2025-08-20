using BusinessAcessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using BusinessAcessLayer.Constant;
using System.Net;
using BusinessAcessLayer.CustomException;
namespace LedgerBookWebApi.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IJWTTokenService _jWTService;
    private readonly ILoginService _loginService;
    private readonly IBusinessService _businessService;
    private readonly IUserBusinessMappingService _userBusinessMappingService;

    public PermissionHandler(
    IJWTTokenService jWTService,
    ILoginService loginService,
    IBusinessService businessService,
    IUserBusinessMappingService userBusinessMappingService
    ) : base()
    {
        this._jWTService = jWTService;
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
            throw new UnauthorizedAccessException();

        ApplicationUser user = _loginService.GetUserFromTokenIdentity(usertoken);
        if (user == null)
            throw new UnauthorizedAccessException();

        string businessToken = httpContext.Request.Headers[TokenKey.BusinessToken];
        if (requirement.Permission != "User")
        {
            if (string.IsNullOrEmpty(businessToken))
                throw new BusinessNotFoundException("Business Token Not found.");
            Businesses business = _businessService.GetBusinessFromToken(businessToken);
            if (business == null)
                throw new BusinessNotFoundException("Business Token Not found.");
            List<RoleViewModel> rolesByUser = _userBusinessMappingService.GetRolesByBusinessId(business.Id, user.Id);
            switch (requirement.Permission)
            {
                case "Owner/Admin":
                    if (rolesByUser.Any(role => role.RoleName == ConstantVariables.OwnerRole))
                    {
                        context.Succeed(requirement);
                    }
                    break;
                case "PurchaseManager":
                    if (rolesByUser.Any(role => role.RoleName == ConstantVariables.PurchaseManagerRole || role.RoleName == ConstantVariables.OwnerRole))
                    {
                        context.Succeed(requirement);
                    }
                    break;
                case "SalesManager":
                    if (rolesByUser.Any(role => role.RoleName == ConstantVariables.SalesManagerRole || role.RoleName == ConstantVariables.OwnerRole))
                    {
                        context.Succeed(requirement);
                    }
                    break;
                case "AnyRole":
                    if (rolesByUser.Any(role => role.RoleName == ConstantVariables.SalesManagerRole || role.RoleName == ConstantVariables.OwnerRole || role.RoleName == ConstantVariables.PurchaseManagerRole))
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
            throw new UnauthorizedAccessException();
        }
        return Task.CompletedTask;
    }
}