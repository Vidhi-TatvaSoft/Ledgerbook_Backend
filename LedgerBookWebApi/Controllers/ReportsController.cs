using System.Net;
using System.Threading.Tasks;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
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
    private readonly ITransactionReportSevice _transactionReportService;

    public ReportsController(
        ILoginService loginService,
        IBusinessService businessService,
        ITransactionReportSevice transactionReportSevice
    ) : base(loginService, businessService)
    {
        _transactionReportService = transactionReportSevice;
    }

    #region get party counts for report page
    [HttpGet]
    [Route("GetPartyCounts")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetPartyCounts()
    {
        Businesses business = GetBusinessFromToken();
        return Ok(_transactionReportService.GetReportCounts(business.Id));
    }
    #endregion

    #region get transaction entries
    [HttpGet]
    [Route("GetReportTransactionEntries")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetReportTransactionEntries(string partyType, int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        Businesses business = GetBusinessFromToken();
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(_transactionReportService.GetReportTransactionEntries(business.Id, user.Id, partyType, searchPartyId, startDate, endDate));
    }
    #endregion

    #region get search party options 
    [HttpGet]
    [Route("GetSearchPartyOptions")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetSearchPartyOptions(string partytype, string searchText = "")
    {
        Businesses business = GetBusinessFromToken();
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(_transactionReportService.GetSearchPartyOptions(business.Id, user.Id, partytype));
    }
    #endregion

    #region generate pdf
    [HttpGet]
    [Route("GetReportPdfData")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetReportPdfData(string partytype, string timePeriod, int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        Businesses business = GetBusinessFromToken();
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(_transactionReportService.GetReportdata(partytype, timePeriod, business, user.Id, searchPartyId, startDate, endDate));
    }
    #endregion

    #region  generate excel
    [HttpGet]
    [Route("GetReportExcelData")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReportExcelData(string partytype, string timePeriod = "This Month", int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        Businesses business = GetBusinessFromToken();
        ApplicationUser user = GetCurrentUserIdentity();
        if (partytype.IsNullOrEmpty() || business == null || business.Id == 0 || user.Id == 0)
        {
            return Ok(new ApiResponse<FileContentResult>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest));
        }
        return await _transactionReportService.GetExcelData(partytype, timePeriod, business, user.Id, searchPartyId, startDate, endDate);
    }
    #endregion
}
