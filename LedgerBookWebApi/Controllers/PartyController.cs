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
       IBusinessService businessService,
       IPartyService partyService
    ) : base(loginService, businessService)
    {
        _partyService = partyService;
    }

    #region check roles and permissions
    [HttpGet]
    [Route("CheckRolePermission")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult CheckRolePermission()
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.CheckRolepermission(business.Id, user.Id));
    }
    #endregion

    #region get all parties
    [HttpGet]
    [Route("GetAllParties")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetAllParties(string partyType, string searchText = "", string filter = "-1", string sort = "-1")
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetParties(partyType, business.Id, user.Id, searchText, filter, sort));
    }
    #endregion

    #region get party and save party method
    [HttpGet]
    [Route("GetPartyDetails/{partyId}")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetPartyDetails([FromRoute] int partyId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetpartyByIdResponse(partyId, business.Id, user.Id));
    }

    [HttpPost]
    [Route("SaveParty")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [Route("VerifyPartyEmail/{verificationCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> VerifyPartyEmail([FromRoute] string verificationCode)
    {
        return Ok(await _partyService.PartyEmailVerification(verificationCode));
    }
    #endregion

    #region display transaction entries
    [HttpGet]
    [Route("GetTransationEntries/{partyId}")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetTransationEntries([FromRoute] int partyId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetTransactionsByPartyId(partyId, business.Id, user.Id));
    }
    #endregion

    #region get transaction entry, save transaction entry and delete transaction entry
    [HttpGet]
    [Route("GetTransactionDetailById")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetTransactionDetailById(int partyId, EnumHelper.TransactionType? transactionType, int? transactionId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.GetTransactionDetailById(business.Id, user.Id, partyId, transactionType, transactionId));
    }

    [HttpPost]
    [Route("SaveTransactionEntry")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SaveTransactionEntry([FromForm] TransactionEntryViewModel transactionEntryVM)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(await _partyService.SaveTransactionEntryWithPermission(transactionEntryVM, user.Id, business));
    }

    [HttpPost]
    [Route("DeleteTransaction/{transactionId}")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult DeleteTransaction([FromRoute] int transactionId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.DeleteTransaction(transactionId, user.Id, business.Id));
    }
    #endregion

    #region get total amount base on partytype
    [HttpGet]
    [Route("GetTotalAmount/{partyType}")]
    [PermissionAuthorize("AnyRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetTotalAmount([FromRoute] EnumHelper.PartyType partyType)
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult SendReminderToParty(decimal netBalance, int partyId)
    {
        ApplicationUser user = GetCurrentUserIdentity();
        Businesses business = GetBusinessFromToken();
        return Ok(_partyService.SendReminder(netBalance, partyId, user.Id, business));
    }
    #endregion
}
