using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using IronPdf.Rendering;
using LedgerBookWebApi.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TheArtOfDev.HtmlRenderer.PdfSharp;

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

    [HttpGet]
    [Route("GetReportPdfDatatest")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetReportPdfDatatest(string partytype)
    {
        var renderer = new ChromePdfRenderer();

        // Remove all margins
        renderer.RenderingOptions.MarginTop = 0;
        renderer.RenderingOptions.MarginBottom = 0;
        renderer.RenderingOptions.MarginLeft = 0;
        renderer.RenderingOptions.MarginRight = 0;

        // Ensure no automatic headers/footers
        // renderer.RenderingOptions.CreatePdfFormsFromHtml = false;
        // renderer.RenderingOptions.CssMediaType = PdfCssMediaType.Screen;
        // renderer.RenderingOptions.PrintHtmlBackgrounds = true;

        // // Paper settings
        // renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;

        // // Disable Chrome’s default print margins
        // renderer.RenderingOptions.SetCustomPaperSizeInInches(8.00, 11.00); // exact A4
        //                                                                    // renderer.RenderingOptions.EnableCustomPaperSize = true;
        // renderer.RenderingOptions.Zoom = 0;

        string htmlContent = "<div style='background-color: green;width:100%;height:100%;margin:0;padding:0;'>hiiiii</div>";

        var pdfDocument = renderer.RenderHtmlAsPdf(htmlContent);

        return File(pdfDocument.BinaryData, "application/pdf", "generated.pdf");
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
        return await _transactionReportService.GetExcelData(partytype, timePeriod, business, user.Id, searchPartyId, startDate, endDate);
    }
    #endregion
}
