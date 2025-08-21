using System.Net;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;


namespace LedgerBookWebApi.Controllers;

public class BaseController : ControllerBase
{
    protected readonly ILoginService _loginService;
    protected readonly IBusinessService? _businessService;

    protected BaseController(ILoginService loginService)
    {
        _loginService = loginService;
    }
    protected BaseController(ILoginService loginService, IBusinessService businessService)
    {
        _loginService = loginService;
        _businessService = businessService;

    }

    #region get data from ajax header
    protected string? GetData(string tokenKey)
    {
        try
        {
            if (Request.Headers.TryGetValue(tokenKey, out StringValues _headerValues))
            {
                string customHeaderValue = _headerValues.FirstOrDefault()!.Replace("Bearer ", "");
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

    #region get current user and business from token
    protected ApplicationUser GetCurrentUserIdentity()
    {
        string token = GetData(TokenKey.Authorization)!;
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

    protected Businesses GetBusinessFromToken()
    {
        string token = GetData(TokenKey.BusinessToken)!;
        if (string.IsNullOrEmpty(token))
        {
            return null!;
        }
        Businesses business = _businessService.GetBusinessFromToken(token);
        if (business == null)
        {
            return null!;
        }
        return business;
    }
    #endregion

    #region render to login page if not authorized
    protected IActionResult RedirectToLoginIfNotAuthenticated()
    {
        ApplicationUser user = GetCurrentUserIdentity();
        if (user == null)
        {
            return Ok(new ApiResponse<ApplicationUser>(false, null, null, HttpStatusCode.Forbidden));
        }
        return Ok(new ApiResponse<ApplicationUser>(true, null, user, HttpStatusCode.OK));
    }
    #endregion
}