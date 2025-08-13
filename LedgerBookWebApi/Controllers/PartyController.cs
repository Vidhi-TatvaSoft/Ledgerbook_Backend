using System.Net;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using LedgerBookWebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerBookWebApi.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class PartyController : BaseController
{
    private readonly IPartyService _partyService;
    public PartyController(
       ILoginService loginService,
       IActivityLogService activityLogService,
       IBusinessService businessService,
       IPartyService partyService
    ) : base(loginService, activityLogService, businessService)
    {
        _partyService = partyService;
    }

    [HttpGet]
    [Route("CheckRolePermission")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult CheckRolePermission()
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.CheckRolepermission(business.Id, user.Id));
    }

    [HttpGet]
    [Route("GetAllParties")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetAllParties(string partyType, string searchText = "", string filter = "-1", string sort = "-1")
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetParties(partyType, business.Id, user.Id, searchText, filter, sort));
    }

    #region save party post method
    [HttpPost]
    [Route("SaveParty")]
    [PermissionAuthorize("AnyRole")]
    public async Task<IActionResult> SaveParty([FromForm] SavePartyViewModel partyViewModel)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        if (!ModelState.IsValid)
        {
            return Ok(new ApiResponse<SavePartyViewModel>(false, Messages.ExceptionMessage, partyViewModel, HttpStatusCode.BadRequest));
        }
        return Ok(await _partyService.SaveParty(partyViewModel, user.Id, business));

    }
    #endregion

    #region verify party email
    [HttpGet]
    [Route("VerifyPartyEmail")]
    public async Task<IActionResult> VerifyPartyEmail(string verificationCode)
    {
        return Ok(await _partyService.PartyEmailVerification(verificationCode));
    }
    #endregion

    [HttpGet]
    [Route("GetPartyDetails")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetPartyDetails(int partyId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetpartyByIdResponse(partyId, business.Id, user.Id));
    }

    #region display transaction entries
    [HttpGet]
    [Route("GetTransationEntries")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetTransationEntries(int partyId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetTransactionsByPartyId(partyId, business.Id, user.Id));
    }
    #endregion

    #region save transaction modal display
    [HttpGet]
    [Route("GetTransactionDetailById")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetTransactionDetailById(int partyId, EnumHelper.TransactionType? transactionType, int? transactionId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetTransactionDetailById(business.Id, user.Id, partyId, transactionType, transactionId));
    }
    #endregion

    #region save transaction post method
    [HttpPost]
    [Route("SaveTransactionEntry")]
    [PermissionAuthorize("AnyRole")]
    public async Task<IActionResult> SaveTransactionEntry([FromForm] TransactionEntryViewModel transactionEntryVM)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(await _partyService.SaveTransactionEntryWithPermission(transactionEntryVM, user.Id, business));
    }
    #endregion

    #region delete transaction
    [HttpPost]
    [Route("DeleteTransaction")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult DeleteTransaction([FromForm] int transactionId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.DeleteTransaction(transactionId, user.Id, business.Id));
    }
    #endregion

    #region  total amount base on partytype
    [HttpGet]
    [Route("GetTotalAmount")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult GetTotalAmount(EnumHelper.PartyType partyType)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetTotalByPartyType(partyType, user.Id, business.Id));
    }
    #endregion

    #region settle up party balance
    [HttpPost]
    [Route("SettleUpParty")]
    [PermissionAuthorize("AnyRole")]
    public async Task<IActionResult> SettleUpParty([FromForm] decimal netBalance, [FromForm] int partyId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(await _partyService.SettleUp(netBalance, partyId, user.Id, business));
    }
    #endregion

    #region Send reminders to party for pending payment
    [HttpGet]
    [Route("SettleUpParty")]
    [PermissionAuthorize("AnyRole")]
    public IActionResult SendReminderToParty(decimal netBalance, int partyId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.SendReminder(netBalance, partyId, user.Id, business));
    }
    #endregion


}
