using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;


namespace LedgerBookWebApi.Controllers;

public class BaseController : ControllerBase
{
    protected readonly ILoginService _loginService;
    private readonly IActivityLogService _activityLogService;

    protected BaseController(ILoginService loginService, IActivityLogService activityLogService)
    {
        _loginService = loginService;
        _activityLogService = activityLogService;
    }

    #region get data from ajax header
    [HttpGet]
    public string? GetData(string tokenKey)
    {
        try
        {
            if (Request.Headers.TryGetValue(tokenKey, out StringValues _headerValues))
            {
                string customHeaderValue = _headerValues.FirstOrDefault()!;
                return customHeaderValue;
            }
            return null;
        }
        catch (Exception e)
        {
            return null;
        }

    }
    #endregion

    #region get current user from token
    protected ApplicationUser GetCurrentUserIdentity()
    {
        string token = GetData(TokenKey.UserToken)!;
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }
        ApplicationUser user = _loginService.GetUserFromTokenIdentity(token);
        if (user == null)
        {
            return null;
        }
        return user;
    }
    #endregion

    // #region render to login page if not authorized
    // protected IActionResult RedirectToLoginIfNotAuthenticated()
    // {
    //     string token = Request.Cookies[TokenKey.UserToken];
    //     if (Request.Cookies[TokenKey.UserToken] == null)
    //     {
    //         return RedirectToAction("Login", "Login");
    //     }
    //     return null;
    // }
    // #endregion

    // #region render to login page if not authorized
    // protected IActionResult RedirectToIndexIfBusinessNotFound()
    // {
    //     string token = Request.Cookies[TokenKey.BusinessToken];
    //     if (Request.Cookies[TokenKey.BusinessToken] == null)
    //     {
    //         return RedirectToAction("Index", "Business");
    //     }
    //     return null;
    // }
    // #endregion

    // // #region partial view response
    // // protected async Task<ViewResponseModel> PartialViewResponse(string viewName, object model, string message = null, ErrorType errorType = ErrorType.Success)
    // // {
    // //     ViewResponseModel partialViewResponseModel = new();
    // //     partialViewResponseModel.Message = message;
    // //     partialViewResponseModel.ErrorType = errorType;
    // //     partialViewResponseModel.HTML = await _viewRenderService.RenderToStringAsync($"Views/{viewName}", model);
    // //     return partialViewResponseModel;
    // // }
    // // #endregion

    // #region set activity log
    // protected async Task<ActivityLogs> SetActivityLog(string message, EnumHelper.Actiontype action, EnumHelper.ActivityEntityType entityType, int EntityTypeId, int? createdById = null, EnumHelper.ActivityEntityType? subEntityType = null, int? subEntityTypeId = null)
    // {
    //     return await _activityLogService.SetActivityLog(message, action, entityType, EntityTypeId, createdById, subEntityType, subEntityTypeId);
    // }
    // #endregion

    // #region fill view bag for category, type
    // // protected void SetViewBag(List<ReferenceDataValues>? categories = null, List<ReferenceDataValues>? Types = null)
    // // {
    // //     ViewBag.Categories = new SelectList(categories, "Id", "EntityValue");
    // //     ViewBag.Types = new SelectList(Types, "Id", "EntityValue");
    // //     return;
    // // }
    // #endregion
}