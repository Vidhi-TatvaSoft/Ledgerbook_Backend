using System.Net;
using System.Net.Http.Headers;
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
    private readonly ITransactionReportSevice _transactionReportService;
    private readonly IPartyService _partyService;
    private readonly IUserBusinessMappingService _userBusinessMappingService;

    public ReportsController(
        ILoginService loginService,
        IActivityLogService activityLogService,
        IBusinessService businessService,
        ITransactionReportSevice transactionReportSevice,
        IPartyService partyService,
        IUserBusinessMappingService userBusinessMappingService
    ) : base(loginService, activityLogService, businessService)
    {
        _transactionReportService = transactionReportSevice;
        _partyService = partyService;
        _userBusinessMappingService = userBusinessMappingService;
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
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(_transactionReportService.GetReportTransactionEntries(business.Id, user.Id, partyType, searchPartyId, startDate, endDate));
    }
    #endregion

    #region  search options 
    [HttpGet]
    [Route("GetSearchPartyOptions")]
    [PermissionAuthorize("AnyRole")]
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
    public IActionResult GetReportPdfData(string partytype, string timePeriod, int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        Businesses business = GetBusinessFromToken();
        ApplicationUser user = GetCurrentUserIdentity();
        return Ok(_transactionReportService.GetReportdata(partytype, timePeriod, business, user.Id, searchPartyId, startDate, endDate));
    }

    [HttpPost]
    [Route("GetReportPdfDatatest")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetReportPdfDatatest(string partytype, string htmlContent)
    {
        Businesses business = GetBusinessFromToken();
        ApplicationUser user = GetCurrentUserIdentity();
        var renderer = new IronPdf.ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf(htmlContent);
        byte[] pdfBytes = pdf.BinaryData;
        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = new ByteArrayContent(pdfBytes);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = "generated_document.pdf"
        };
        return (IActionResult)response;
        // return Ok(_transactionReportService.GetReportdata(partytype, timePeriod, business, user.Id, searchPartyId, startDate, endDate));
    }
    #endregion

    #region  generate excel
    [HttpGet]
    [Route("GetReportExcelData")]
    [PermissionAuthorize("AnyRole")]
    public async Task<IActionResult> GetReportExcelData(string partytype, string timePeriod = "This Month", int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        Businesses business = GetBusinessFromToken();
        ApplicationUser user = GetCurrentUserIdentity();
        if (partytype.IsNullOrEmpty() || business == null || business.Id == 0 || user.Id == 0)
        {
            return Ok(new ApiResponse<FileContentResult>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest));
        }
        if (!_userBusinessMappingService.HasPermission(business.Id, user.Id, partytype))
        {
            return Ok(new ApiResponse<ReportTransactionEntriesViewModel>(false, Messages.ForbiddenMessage, null, HttpStatusCode.BadRequest));
        }
        return await _transactionReportService.GetExcelData(partytype, timePeriod, business, user.Id, searchPartyId, startDate, endDate);
    }
    #endregion
}
