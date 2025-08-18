using System.Net;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using LedgerBookWebApi.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class ReportsController : BaseController
{
    public ITransactionReportSevice _transactionReportService;
    public IPartyService _partyService;
    public ReportsController(
        ILoginService loginService,
        IActivityLogService activityLogService,
        IBusinessService businessService,
        ITransactionReportSevice transactionReportSevice,
        IPartyService partyService
    ) : base(loginService, activityLogService, businessService)
    {
        _transactionReportService = transactionReportSevice;
        _partyService = partyService;
    }

    [HttpGet]
    [Route("GetPartyCounts")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetPartyCounts()
    {
        Businesses business = GetBusinessFromToken();
        return Ok(_transactionReportService.GetReportCounts(business.Id));
    }


    #region display transaction entries
    [HttpGet]
    [Route("GetReportTransactionEntries")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetReportTransactionEntries(string partyType, int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        Businesses business = GetBusinessFromToken();
        return Ok(_transactionReportService.GetReportTransactionEntries(business.Id, partyType, searchPartyId, startDate, endDate));
    }
    #endregion

    #region  search options 
    [HttpGet]
    [Route("GetSearchPartyOptions")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetSearchPartyOptions(string partytype, string searchText = "")
    {
        Businesses business = GetBusinessFromToken();
        if (partytype.IsNullOrEmpty() || business.Id == 0)
        {
            return Ok(new ApiResponse<string>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest));
        }
        List<PartyViewModel> parties = _partyService.GetPartiesByType(partytype, business.Id, searchText, "-1", "-1");
        return Ok(new ApiResponse<List<PartyViewModel>>(true, null, parties, HttpStatusCode.OK));
    }
    #endregion

    #region generate pdf
    [HttpGet]
    [Route("GetReportPdfData")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetReportPdfData(string partytype, string timePeriod, int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        Businesses business = GetBusinessFromToken();
        return Ok(_transactionReportService.GetReportdata(partytype, timePeriod, business, searchPartyId, startDate, endDate));
    }
    #endregion

    #region  generate excel
    [HttpGet]
    [Route("GenerateExcel")]
    [PermissionAuthorize("AnyRole")]
    public async Task<IActionResult> GenerateExcel(string partytype, string timePeriod, int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        Businesses business = GetBusinessFromToken();
        if (partytype.IsNullOrEmpty() || business == null || business.Id == 0)
        {
            return Ok(new ApiResponse<FileContentResult >(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest));
        }
        return await _transactionReportService.GetExcelData(partytype, timePeriod, business, searchPartyId, startDate, endDate);
    }
    #endregion
}
