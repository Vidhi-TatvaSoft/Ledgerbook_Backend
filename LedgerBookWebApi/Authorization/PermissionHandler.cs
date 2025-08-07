using BusinessAcessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using BusinessAcessLayer.Constant;
namespace LedgerBook.Authorization;

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
        HttpContext httpContext = _httpContextAccessor.HttpContext;
        string cookieSavedToken = httpContext.Request.Headers[TokenKey.UserToken];

        if (string.IsNullOrEmpty(cookieSavedToken))
        {
            throw new UnauthorizedAccessException();
            // throw new Exception("User token not Found");
            // context.Result
        }
        ApplicationUser user = _loginService.GetUserFromTokenIdentity(cookieSavedToken);

        string businessToken = httpContext.Request.Headers[TokenKey.BusinessToken];
        if (string.IsNullOrEmpty(businessToken))
        {
            throw new UnauthorizedAccessException();
            // throw new Exception("Business token not Found");
        }
        Businesses business = _businessService.GetBusinessFromToken(businessToken);
        List<RoleViewModel> rolesByUser = _userBusinessMappingService.GetRolesByBusinessId(business.Id, user.Id);

        string email = _jWTService.GetClaimValue(cookieSavedToken, "email");
        if (string.IsNullOrEmpty(email))
        {
            httpContext.Response.Redirect("/Login/Login");
            return Task.CompletedTask;
        }
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
        return Task.CompletedTask;
    }
}