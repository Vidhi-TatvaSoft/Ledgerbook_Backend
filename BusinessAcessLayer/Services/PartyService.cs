using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Helper;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace BusinessAcessLayer.Services;

public class PartyService : IPartyService
{
    private readonly LedgerBookDbContext _context;
    private readonly IAddressService _addressService;
    private readonly IReferenceDataEntityService _referenceDataEntityService;
    private readonly IGenericRepo _genericRepository;
    private readonly IActivityLogService _activityLogService;
    private readonly IBusinessService _businessService;
    private readonly IUserService _userService;
    private readonly IUserBusinessMappingService _userBusinessMappingService;
    private readonly IJWTTokenService _jwtTokenService;


    public PartyService(LedgerBookDbContext context,
    IAddressService addressService,
    IReferenceDataEntityService referenceDataEntityService,
    IGenericRepo genericRepository,
    IActivityLogService activityLogService,
    IBusinessService businessService,
    IUserService userService,
    IUserBusinessMappingService userBusinessMappingService,
    IJWTTokenService jWTTokenService
    )
    {
        _context = context;
        _addressService = addressService;
        _referenceDataEntityService = referenceDataEntityService;
        _genericRepository = genericRepository;
        _activityLogService = activityLogService;
        _businessService = businessService;
        _userService = userService;
        _userBusinessMappingService = userBusinessMappingService;
        _jwtTokenService = jWTTokenService;
    }


    public ApiResponse<List<PartyViewModel>> GetParties(string partyType, int businessId, int userId, string searchText, string? filter = "-1", string? sort = "-1")
    {
        if (partyType == null || businessId == 0 || userId == 0)
        {
            return new ApiResponse<List<PartyViewModel>>(false, Messages.ExceptionMessage, null, HttpStatusCode.ServiceUnavailable);
        }
        if (_userBusinessMappingService.HasPermission(businessId, userId, partyType))
        {
            List<PartyViewModel> parties = GetPartiesByType(partyType, businessId, searchText, filter, sort);
            return new ApiResponse<List<PartyViewModel>>(true, null, parties, HttpStatusCode.OK);
        }
        return new ApiResponse<List<PartyViewModel>>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);

    }

    public List<PartyViewModel> GetPartiesByType(string partyType, int businessId, string searchText, string? filter = "-1", string? sort = "-1")
    {
        int partyTypeId = _genericRepository.Get<ReferenceDataValues>(x => x.EntityType.EntityType == ConstantVariables.PartyType && x.EntityValue == partyType,
         includes: new List<Expression<Func<ReferenceDataValues, object>>>
            {
                x => x.EntityType
            })!.Id;

        List<Parties> allParties = _genericRepository.GetAll<Parties>(x => x.PartyTypId == partyTypeId && x.BusinessId == businessId && x.DeletedAt == null).ToList();
        List<PartyViewModel> partyList = new();

        foreach (Parties party in allParties)
        {
            PartyViewModel partyViewModel = new();
            partyViewModel.PartyId = party.Id;
            partyViewModel.PartyName = party.PartyName;
            partyViewModel.Email = party.Email;
            partyViewModel.PartyTypId = party.PartyTypId;
            partyViewModel.PartyTypeString = partyType;
            partyViewModel.CreatedAt = party.CreatedAt;
            partyViewModel.CreateMonth = party.UpdatedAt != null ? party.UpdatedAt.Value.Month : party.CreatedAt.Month;
            partyViewModel.UpdatedAt = party.UpdatedAt;
            partyViewModel.Amount = _genericRepository.GetAll<LedgerTransactions>(x => x.PartyId == party.Id && x.DeletedAt == null && x.TransactionType == (byte)EnumHelper.TransactionType.GAVE).Sum(x => x.Amount) - _genericRepository.GetAll<LedgerTransactions>(x => x.PartyId == party.Id && x.DeletedAt == null && x.TransactionType == (byte)EnumHelper.TransactionType.GOT).Sum(x => x.Amount);

            partyViewModel.TransactionType = partyViewModel.Amount < 0 ? EnumHelper.TransactionType.GOT : EnumHelper.TransactionType.GAVE;

            partyViewModel.GSTIN = party.GSTIN == null ? null : party.GSTIN;
            partyViewModel.Address = party.AddressId == null ? null : _addressService.GetAddressById((int)party.AddressId);
            partyViewModel.BusinessId = party.BusinessId;
            partyList.Add(partyViewModel);
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            string lowerSearchTerm = searchText.ToLower();
            partyList = partyList.Where(x =>
                x.PartyName.ToLower().Contains(lowerSearchTerm) ||
                x.Email.ToLower().Contains(lowerSearchTerm)
            ).ToList();
        }

        switch (filter)
        {
            case "-1":
                partyList = partyList;
                break;
            case "All":
                partyList = partyList;
                break;
            case "Give":
                partyList = partyList.Where(x => x.Amount < 0).ToList();
                break;
            case "Get":
                partyList = partyList.Where(x => x.Amount > 0).ToList();
                break;
            case "Settled":
                partyList = partyList.Where(x => x.Amount == 0).ToList();
                break;
        }

        switch (sort)
        {
            case "-1":
                partyList = partyList.OrderByDescending(x => x.CreatedAt).ToList();;
                break;
            case "mostRecent":
                partyList = partyList.OrderByDescending(x => x.CreatedAt).ToList();
                break;
            case "HighestAmount":
                partyList = partyList.OrderByDescending(x => Math.Abs((decimal)x.Amount)).ToList();
                break;
            case "LeastAmount":
                partyList = partyList.OrderBy(x => Math.Abs((decimal)x.Amount)).ToList();
                break;
            case "ByName":
                partyList = partyList.OrderBy(x => x.PartyName).ToList();
                break;
            case "Oldest":
                partyList = partyList.OrderBy(x => x.CreatedAt).ToList();
                break;
        }
        return partyList;
    }

    public async Task<ApiResponse<SavePartyViewModel>> SaveParty(SavePartyViewModel partyViewModel, int userId, Businesses business)
    {
        if (!_userBusinessMappingService.HasPermission(business.Id, userId, partyViewModel.PartyTypeString.ToString()))
        {
            return new ApiResponse<SavePartyViewModel>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
        }
        bool IsPartyPresent = partyViewModel.PartyId == 0 ? false : true;
        //check if email changes or not
        if (partyViewModel.PartyId != 0)
        {
            if (IsEmailChanged(partyViewModel))
                partyViewModel.IsEmailChaneged = true;
            else
                partyViewModel.IsEmailChaneged = false;
        }

        int partyId = await SavePartyDetails(partyViewModel, userId);
        if (partyViewModel.PartyId == 0 || partyViewModel.IsEmailChaneged)
        {
            string verificationToken = GetEmailVerifiactionTokenForParty(partyId);
            string verificationCode = _jwtTokenService.GenerateTokenPartyEmailVerification(partyViewModel.Email, verificationToken, partyId, business.BusinessName, partyViewModel.PartyTypeString.ToString());
            string verificationLink = "http://localhost:5189/Party/VerifyParty?verificationCode=" + verificationCode;
            _ = CommonMethods.VerifyParty(partyViewModel.PartyName, partyViewModel.Email, verificationLink, partyViewModel.PartyTypeString.ToString(), business.BusinessName);
        }

        partyViewModel.PartyId = partyId;
        if (partyViewModel.Amount != null)
        {
            TransactionEntryViewModel transactionViewModel = new();
            transactionViewModel.TransactionAmount = (Decimal)partyViewModel.Amount;
            transactionViewModel.TransactionType = (byte)partyViewModel.TransactionType;
            transactionViewModel.PartyId = partyId;
            transactionViewModel.BusinessName = business.BusinessName;
            int transactionId = await SaveTransactionEntry(transactionViewModel, userId);
            if (transactionId != 0)
            {
                return new ApiResponse<SavePartyViewModel>(true, Messages.PartyAddedWithOpeningBalance, partyViewModel, HttpStatusCode.OK);
            }
            else
            {
                return new ApiResponse<SavePartyViewModel>(true, string.Format(Messages.GlobalAddUpdateMesage, "Party", "added"), partyViewModel, HttpStatusCode.OK);
            }
        }
        if (IsPartyPresent)
        {
            if (partyId != 0)
            {
                return new ApiResponse<SavePartyViewModel>(true, string.Format(Messages.GlobalAddUpdateMesage, "Party", "updated"), partyViewModel, HttpStatusCode.OK);
            }
            else
            {
                return new ApiResponse<SavePartyViewModel>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "update", "party"), partyViewModel, HttpStatusCode.BadRequest);
            }
        }
        else
        {
            if (partyId != 0)
            {
                return new ApiResponse<SavePartyViewModel>(true, string.Format(Messages.GlobalAddUpdateMesage, "Party", "added"), partyViewModel, HttpStatusCode.OK);
            }
            else
            {
                return new ApiResponse<SavePartyViewModel>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "add", "party"), partyViewModel, HttpStatusCode.BadRequest);
            }
        }
    }

    public async Task<int> SavePartyDetails(SavePartyViewModel partyViewModel, int userId)
    {
        string businessName = _businessService.GetBusinessNameById(partyViewModel.BusinessId);
        string userName = _userService.GetuserNameById(userId);
        if (partyViewModel.PartyId == 0)
        {
            Parties party = new();
            party.PartyName = partyViewModel.PartyName;
            party.BusinessId = partyViewModel.BusinessId;
            party.PartyTypId = _genericRepository.Get<ReferenceDataValues>(x => x.EntityType.EntityType == ConstantVariables.PartyType && x.EntityValue == partyViewModel.PartyTypeString.ToString(),
                includes: new List<Expression<Func<ReferenceDataValues, object>>>
                {
                    x => x.EntityType
                })!.Id;
            // _referenceDataValueRepo.Get(x => x.EntityType.EntityType == "PartyType" && x.EntityValue == partyType).Id;
            party.Email = partyViewModel.Email.ToLower().Trim();
            party.CreatedAt = DateTime.UtcNow;
            party.CreatedById = userId;
            party.IsEmailVerified = false;
            Guid guid = Guid.NewGuid();
            party.VerificationToken = guid.ToString();
            await _genericRepository.AddAsync<Parties>(party);
            string message = string.Format(Messages.PartyActivity, partyViewModel.PartyTypeString, party.PartyName, "added ", businessName, userName);
            await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Add, EnumHelper.ActivityEntityType.Business, party.BusinessId, userId, EnumHelper.ActivityEntityType.Party, party.Id);
            return party.Id;
        }
        else
        {
            Parties party = _genericRepository.Get<Parties>(x => x.Id == partyViewModel.PartyId && x.DeletedAt == null);
            party.PartyName = partyViewModel.PartyName;
            party.BusinessId = partyViewModel.BusinessId;
            party.PartyTypId = _genericRepository.Get<ReferenceDataValues>(x => x.EntityType.EntityType == ConstantVariables.PartyType && x.EntityValue == partyViewModel.PartyTypeString.ToString(),
                includes: new List<Expression<Func<ReferenceDataValues, object>>>
                    {
                        x => x.EntityType
                    })!.Id;
            //  _referenceDataValueRepo.Get(x => x.EntityType.EntityType == "PartyType" && x.EntityValue == partyType).Id;
            if (partyViewModel.IsEmailChaneged)
            {
                party.IsEmailVerified = false;
                Guid guid = Guid.NewGuid();
                party.VerificationToken = guid.ToString();
            }
            party.Email = partyViewModel.Email.ToLower().Trim();
            party.UpdatedAt = DateTime.UtcNow;
            party.UpdatedById = userId;
            await _genericRepository.UpdateAsync<Parties>(party);
            string message = string.Format(Messages.PartyUpdateActivity, partyViewModel.PartyTypeString, party.PartyName, businessName, "updated ", userName);
            await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Update, EnumHelper.ActivityEntityType.Business, party.BusinessId, userId, EnumHelper.ActivityEntityType.Party, party.Id);
            return party.Id;
        }
    }

    public string GetEmailVerifiactionTokenForParty(int partyId)
    {
        return _genericRepository.Get<Parties>(x => x.Id == partyId && !x.DeletedAt.HasValue).VerificationToken;
    }

    public async Task<ApiResponse<PartyVerifiedViewModel>> PartyEmailVerification(string verificationCode)
    {
        if (verificationCode == null )
        {
            return new ApiResponse<PartyVerifiedViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        PartyVerifiedViewModel partyVerifiedVM = new();
        partyVerifiedVM.Email = _jwtTokenService.GetClaimValue(verificationCode, "email");
        partyVerifiedVM.Token = _jwtTokenService.GetClaimValue(verificationCode, "token");
        partyVerifiedVM.PartyId = int.Parse(_jwtTokenService.GetClaimValue(verificationCode, "partyId"));
        partyVerifiedVM.BusinessName = _jwtTokenService.GetClaimValue(verificationCode, "businessName");
        partyVerifiedVM.PartyType = _jwtTokenService.GetClaimValue(verificationCode, "partyType");

        bool isEmailVerified = false;
        Parties party = _genericRepository.Get<Parties>(x => x.Email.ToLower().Trim() == partyVerifiedVM.Email.ToLower().Trim() && x.VerificationToken == partyVerifiedVM.Token && x.Id == partyVerifiedVM.PartyId && !x.DeletedAt.HasValue);
        if (party != null)
        {
            party.IsEmailVerified = true;
            await _genericRepository.UpdateAsync<Parties>(party);
            isEmailVerified = true;
        }
        if (isEmailVerified)
        {
            partyVerifiedVM.IsEmailVerified = true;
            return new ApiResponse<PartyVerifiedViewModel>(true, null, partyVerifiedVM, HttpStatusCode.OK);
        }
        else
        {
            return new ApiResponse<PartyVerifiedViewModel>(false, null, partyVerifiedVM, HttpStatusCode.BadRequest);
        }

    }

    public bool IsPartyverified(int partyId)
    {
        return _genericRepository.IsPresentAny<Parties>(x => x.Id == partyId && !x.DeletedAt.HasValue && x.IsEmailVerified);
    }

    public bool IsEmailChanged(SavePartyViewModel partyViewModel)
    {
        Parties party = _genericRepository.Get<Parties>(x => x.Id == partyViewModel.PartyId && !x.DeletedAt.HasValue);
        if (party != null)
        {
            if (party.Email.ToLower().Trim() != partyViewModel.Email.ToLower().Trim())
            {
                return true;
            }
        }
        return false;
    }

    public async Task<ApiResponse<int>> SaveTransactionEntryWithPermission(TransactionEntryViewModel transactionEntryViewModel, int userId, Businesses business)
    {
        if (userId == 0 || business.Id == 0 || transactionEntryViewModel.PartyId == 0)
        {
            return new ApiResponse<int>(false, Messages.ExceptionMessage, transactionEntryViewModel.PartyId, HttpStatusCode.BadRequest);
        }
        PartyViewModel partyViewModel = GetPartyById(transactionEntryViewModel.PartyId);
        if (!_userBusinessMappingService.HasPermission(business.Id, userId, partyViewModel.PartyTypeString.ToString()))
        {
            return new ApiResponse<int>(false, Messages.ForbiddenMessage, transactionEntryViewModel.PartyId, HttpStatusCode.Forbidden);
        }
        transactionEntryViewModel.BusinessName = business.BusinessName;
        transactionEntryViewModel.TransactionType = (byte)transactionEntryViewModel.TransactionTypeEnum;
        bool IsPresentTransaction = transactionEntryViewModel.TransactionId == 0 ? false : true;
        int transactionId = await SaveTransactionEntry(transactionEntryViewModel, userId);
        transactionEntryViewModel.TransactionId = transactionId;
        if (transactionId != 0)
        {
            if (!IsPresentTransaction)
            {
                return new ApiResponse<int>(true, string.Format(Messages.GlobalAddUpdateMesage, "Transaction", "added"), transactionEntryViewModel.PartyId, HttpStatusCode.OK);
            }
            else
            {
                return new ApiResponse<int>(true, string.Format(Messages.GlobalAddUpdateMesage, "Transaction", "updated"), transactionEntryViewModel.PartyId, HttpStatusCode.OK);
            }
        }
        else
        {
            if (!IsPresentTransaction)
            {
                return new ApiResponse<int>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "add", "transaction"), transactionEntryViewModel.PartyId, HttpStatusCode.BadRequest);
            }
            else
            {
                return new ApiResponse<int>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "update", "transaction"), transactionEntryViewModel.PartyId, HttpStatusCode.BadRequest);
            }
        }

    }

    public async Task<int> SaveTransactionEntry(TransactionEntryViewModel transactionEntryViewModel, int userId)
    {

        if (transactionEntryViewModel.PartyId == 0 || transactionEntryViewModel.TransactionType == null)
        {
            return 0;
        }

        PartyViewModel partyViewModel = GetPartyById(transactionEntryViewModel.PartyId);
        string userName = _userService.GetuserNameById(userId);
        if (transactionEntryViewModel.TransactionId == 0)
        {
            //add
            LedgerTransactions ledgerTransaction = new();
            ledgerTransaction.PartyId = transactionEntryViewModel.PartyId;
            ledgerTransaction.Amount = transactionEntryViewModel.TransactionAmount;
            ledgerTransaction.TransactionType = transactionEntryViewModel.TransactionType;
            ledgerTransaction.DueDate = transactionEntryViewModel.DueDate == null ? null : transactionEntryViewModel.DueDate;
            ledgerTransaction.Description = transactionEntryViewModel.Description == null ? null : transactionEntryViewModel.Description;
            ledgerTransaction.PaymentMethodId = transactionEntryViewModel.PaymentMethodId == null ? null : transactionEntryViewModel.PaymentMethodId;
            ledgerTransaction.IsSettled = transactionEntryViewModel.IsSettleup;
            ledgerTransaction.CreatedAt = DateTime.UtcNow;
            ledgerTransaction.CreatedById = userId;


            await _genericRepository.AddAsync<LedgerTransactions>(ledgerTransaction);
            string message;
            if (transactionEntryViewModel.IsSettleup)
            {
                message = string.Format(Messages.TransactionActivity, partyViewModel.PartyTypeString, partyViewModel.PartyName, transactionEntryViewModel.BusinessName, "mark as paid", userName);
            }
            else
            {
                message = string.Format(Messages.TransactionAddActivity, partyViewModel.PartyTypeString, partyViewModel.PartyName, ledgerTransaction.Amount.ToString(), transactionEntryViewModel.BusinessName, userName);
            }
            await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Add, EnumHelper.ActivityEntityType.Business, partyViewModel.BusinessId, userId, EnumHelper.ActivityEntityType.Transaction, ledgerTransaction.Id);
            return ledgerTransaction.Id;
        }
        else
        {
            //update
            LedgerTransactions ledgerTransaction = _genericRepository.Get<LedgerTransactions>(x => x.Id == transactionEntryViewModel.TransactionId && x.DeletedAt == null);
            if (ledgerTransaction != null)
            {
                if (ledgerTransaction.IsSettled)
                {
                    if (ledgerTransaction.Amount == transactionEntryViewModel.TransactionAmount && ledgerTransaction.TransactionType == transactionEntryViewModel.TransactionType)
                    {
                        transactionEntryViewModel.IsSettleup = true;
                    }
                    else
                    {
                        transactionEntryViewModel.IsSettleup = false;
                    }
                }
                else
                {
                    transactionEntryViewModel.IsSettleup = false;
                }
                ledgerTransaction.Amount = transactionEntryViewModel.TransactionAmount;
                ledgerTransaction.TransactionType = transactionEntryViewModel.TransactionType;
                ledgerTransaction.DueDate = transactionEntryViewModel.DueDate == null ? null : transactionEntryViewModel.DueDate;
                ledgerTransaction.Description = transactionEntryViewModel.Description == null ? null : transactionEntryViewModel.Description;
                ledgerTransaction.PaymentMethodId = transactionEntryViewModel.PaymentMethodId == null ? null : transactionEntryViewModel.PaymentMethodId;
                ledgerTransaction.IsSettled = transactionEntryViewModel.IsSettleup;
                ledgerTransaction.UpdatedAt = DateTime.UtcNow;
                ledgerTransaction.UpdatedById = userId;

                await _genericRepository.UpdateAsync<LedgerTransactions>(ledgerTransaction);
                string message = string.Format(Messages.TransactionActivity, partyViewModel.PartyTypeString, partyViewModel.PartyName, transactionEntryViewModel.BusinessName, "updated", userName);

                // if (ledgerTransaction.TransactionType == (byte)EnumHelper.TransactionType.GAVE)
                // {
                //     message = string.Format(Messages.TransactionUpdateGAVEMessage, ledgerTransaction.Amount.ToString(), transactionEntryViewModel.BusinessName, partyViewModel.PartyName);
                // }
                // else
                // {
                //     message = string.Format(Messages.TransactionUpdateGOTMessage, ledgerTransaction.Amount.ToString(), transactionEntryViewModel.BusinessName, partyViewModel.PartyName);
                // }
                await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Update, EnumHelper.ActivityEntityType.Business, partyViewModel.BusinessId, userId, EnumHelper.ActivityEntityType.Transaction, ledgerTransaction.Id);
                return ledgerTransaction.Id;
            }
            else
            {
                return 0;
            }

        }
    }

    public ApiResponse<PartyViewModel> GetpartyByIdResponse(int partyId, int businessId, int userId)
    {
        if (partyId == 0 || businessId == 0 || userId == 0)
        {
            return new ApiResponse<PartyViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        PartyViewModel partyViewModel = GetPartyById(partyId);
        if (!_userBusinessMappingService.HasPermission(businessId, userId, partyViewModel.PartyTypeString.ToString()))
        {
            return new ApiResponse<PartyViewModel>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
        }
        return new ApiResponse<PartyViewModel>(true, null, partyViewModel, HttpStatusCode.OK);
    }

    public PartyViewModel GetPartyById(int partyId)
    {
        Parties party = _genericRepository.Get<Parties>(x => x.Id == partyId && x.DeletedAt == null);
        if (party == null)
        {
            return new PartyViewModel();
        }
        else
        {
            PartyViewModel partyViewModel = new();
            partyViewModel.PartyId = party.Id;
            partyViewModel.PartyName = party.PartyName;
            partyViewModel.Email = party.Email;
            partyViewModel.PartyTypId = party.PartyTypId;
            partyViewModel.PartyTypeString = _referenceDataEntityService.GetReferenceValueById(partyViewModel.PartyTypId);
            partyViewModel.GSTIN = party.GSTIN == null ? null : party.GSTIN;
            partyViewModel.Address = party.AddressId == null ? null : _addressService.GetAddressById((int)party.AddressId);
            partyViewModel.BusinessId = party.BusinessId;
            return partyViewModel;
        }
    }

    public decimal GetBalanceTillDate(int partyId, DateTime date)
    {
        decimal amount = 0;
        List<LedgerTransactions> allEntriesOfParty = _genericRepository.GetAll<LedgerTransactions>(x => x.PartyId == partyId && !x.DeletedAt.HasValue).ToList();
        foreach (LedgerTransactions entry in allEntriesOfParty)
        {
            if (entry.UpdatedAt != null)
            {
                if (entry.UpdatedAt <= date)
                {
                    if (entry.TransactionType == (byte)EnumHelper.TransactionType.GAVE)
                    {
                        amount -= entry.Amount;
                    }
                    else
                    {
                        amount += entry.Amount;
                    }
                }

            }
            else
            {
                if (entry.CreatedAt <= date)
                {
                    if (entry.TransactionType == (byte)EnumHelper.TransactionType.GAVE)
                    {
                        amount -= entry.Amount;
                    }
                    else
                    {
                        amount += entry.Amount;
                    }
                }
            }
        }
        return amount;
    }

    public ApiResponse<LedgerEntriesViewModel> GetTransactionsByPartyId(int partyId, int businessId, int userId)
    {
        if (partyId == 0 || businessId == 0 || userId == 0)
        {
            return new ApiResponse<LedgerEntriesViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        PartyViewModel partyViewModel = GetPartyById(partyId);
        if (!_userBusinessMappingService.HasPermission(businessId, userId, partyViewModel.PartyTypeString))
        {
            return new ApiResponse<LedgerEntriesViewModel>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
        }
        LedgerEntriesViewModel ledgerEntriesVM = new();
        List<TransactionEntryViewModel> EntriesList = _genericRepository.GetAll<LedgerTransactions>(x => x.PartyId == partyId && !x.DeletedAt.HasValue)
                               .Select(x => new TransactionEntryViewModel
                               {
                                   TransactionId = x.Id,
                                   PartyId = x.PartyId,
                                   TransactionAmount = x.Amount,
                                   TransactionType = x.TransactionType,
                                   CreatedAt = x.CreatedAt,
                                   CreateDate = x.CreatedAt.ToString("dd MMM yyyy hh:mm tt"),
                                   UpdatedAt = x.UpdatedAt,
                                   Balance = 0,
                                   Description = x.Description,
                                   DueDateString = x.DueDate?.ToString("dd MMM yyyy"),
                               }).OrderByDescending(x => x.CreatedAt).ToList();

        List<LedgerTransactions> allEntriesOfParty = _genericRepository.GetAll<LedgerTransactions>(x => x.PartyId == partyId && !x.DeletedAt.HasValue).ToList();
        for (int i = 0; i < EntriesList.Count; i++)
        {
            decimal amount = 0;
            foreach (LedgerTransactions entry in allEntriesOfParty)
            {
                if (entry.CreatedAt <= EntriesList[i].CreatedAt)
                {
                    if (entry.TransactionType == (byte)EnumHelper.TransactionType.GAVE)
                    {
                        amount -= entry.Amount;
                    }
                    else
                    {
                        amount += entry.Amount;
                    }
                }
            }
            EntriesList[i].Balance = amount;
        }

        ledgerEntriesVM.TransactionsList = EntriesList;
        ledgerEntriesVM.NetBalance = 0;
        foreach (TransactionEntryViewModel entry in ledgerEntriesVM.TransactionsList)
        {
            if (entry.TransactionType == (byte)EnumHelper.TransactionType.GAVE)
                ledgerEntriesVM.NetBalance += entry.TransactionAmount;
            else
                ledgerEntriesVM.NetBalance -= entry.TransactionAmount;
        }
        ledgerEntriesVM.IspartyVerified = IsPartyverified(partyId);
        ledgerEntriesVM.NetBalance = ledgerEntriesVM.TransactionsList.Where(x => x.TransactionType == (byte)EnumHelper.TransactionType.GAVE).Sum(a => a.TransactionAmount) - ledgerEntriesVM.TransactionsList.Where(x => x.TransactionType == (byte)EnumHelper.TransactionType.GOT).Sum(a => a.TransactionAmount);
        return new ApiResponse<LedgerEntriesViewModel>(true, null, ledgerEntriesVM, HttpStatusCode.OK);
    }

    public ApiResponse<TransactionEntryViewModel> GetTransactionDetailById(int businessId, int userId, int partyId, EnumHelper.TransactionType? transactionType, int? transactionId)
    {
        if (businessId == 0 || userId == 0)
        {
            return new ApiResponse<TransactionEntryViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        PartyViewModel partyViewModel = GetPartyById(partyId);
        if (!_userBusinessMappingService.HasPermission(businessId, userId, partyViewModel.PartyTypeString))
        {
            return new ApiResponse<TransactionEntryViewModel>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
        }
        TransactionEntryViewModel transactionEntryVM = new();
        if (transactionId == null)
        {
            transactionEntryVM.TransactionTypeEnum = (EnumHelper.TransactionType)transactionType;
            transactionEntryVM.TransactionId = 0;
            transactionEntryVM.PartyId = partyId;
        }
        else
        {
            //display edit entry modal
            transactionEntryVM = GetTransactionbyTransactionId((int)transactionId);
            transactionEntryVM.TransactionTypeEnum = (EnumHelper.TransactionType)transactionEntryVM.TransactionType;
        }
        return new ApiResponse<TransactionEntryViewModel>(true, null, transactionEntryVM, HttpStatusCode.OK);
    }

    public TransactionEntryViewModel GetTransactionbyTransactionId(int transactionId)
    {
        return _genericRepository.GetAll<LedgerTransactions>(x => x.Id == transactionId && !x.DeletedAt.HasValue).Select(x => new TransactionEntryViewModel
        {
            TransactionId = x.Id,
            PartyId = x.PartyId,
            TransactionAmount = x.Amount,
            TransactionType = x.TransactionType,
            CreatedAt = x.CreatedAt,
            Description = x.Description,
            DueDate = x.DueDate
        }).ToList().FirstOrDefault();
    }

    public ApiResponse<int> DeleteTransaction(int transactionId, int userId, int businessId)
    {
        if (transactionId == 0 || businessId == 0 || userId == 0)
        {
            return new ApiResponse<int>(false, Messages.ExceptionMessage, 0, HttpStatusCode.BadRequest);
        }
        LedgerTransactions transaction = _genericRepository.Get<LedgerTransactions>(t => t.Id == transactionId && !t.DeletedAt.HasValue);
        PartyViewModel partyViewModel = GetPartyById(transaction.PartyId);
        if (!_userBusinessMappingService.HasPermission(businessId, userId, partyViewModel.PartyTypeString))
        {
            return new ApiResponse<int>(false, Messages.ForbiddenMessage, transaction.PartyId, HttpStatusCode.Forbidden);
        }

        bool isDeleted = false;
        string businessname = _businessService.GetBusinessNameById(partyViewModel.BusinessId);
        string userName = _userService.GetuserNameById(userId);
        if (transaction != null)
        {
            transaction.DeletedAt = DateTime.UtcNow;
            transaction.DeletedById = userId;
            _genericRepository.Update<LedgerTransactions>(transaction);
            string message = string.Format(Messages.TransactionActivity, partyViewModel.PartyTypeString, partyViewModel.PartyName, businessname, "deleted", userName);
            _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Delete, EnumHelper.ActivityEntityType.Business, partyViewModel.BusinessId, userId, EnumHelper.ActivityEntityType.Transaction, transaction.Id);
            isDeleted = true;
        }
        if (isDeleted)
            return new ApiResponse<int>(true, string.Format(Messages.GlobalAddUpdateMesage, "Transaction", "deleted"), partyViewModel.PartyId, HttpStatusCode.OK);
        else
            return new ApiResponse<int>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "delete", "transaction"), partyViewModel.PartyId, HttpStatusCode.BadRequest);
    }

    public List<Parties> GetAllPartiesByBusiness(int businessId, int userId)
    {
        if (businessId != 0)
            return _genericRepository.GetAll<Parties>(x => x.BusinessId == businessId).ToList();
        else
        {
            int OwnerRoleId = _genericRepository.Get<Role>(x => x.RoleName == ConstantVariables.OwnerRole).Id;
            List<int> businessIds = _genericRepository.GetAll<UserBusinessMappings>(x => x.UserId == userId && x.RoleId == OwnerRoleId && (x.CreatedById == userId || (!x.DeletedAt.HasValue && x.IsActive))).Select(x => x.BusinessId).Distinct().ToList();
            List<Parties> parties = new();
            foreach (int id in businessIds)
            {
                List<Parties> partiesTemp = _genericRepository.GetAll<Parties>(x => x.BusinessId == id).ToList();
                parties = parties.Concat(partiesTemp).ToList();
            }
            return parties;
        }
    }

    public List<TransactionEntryViewModel> GetAllTransaction(int businessId)
    {
        List<TransactionEntryViewModel> EntriesList = _genericRepository.GetAll<LedgerTransactions>(x => x.Party.BusinessId == businessId && !x.DeletedAt.HasValue,
        includes: new List<Expression<Func<LedgerTransactions, object>>>
           {
                x => x.Party
           }).Select(x => new TransactionEntryViewModel
           {
               TransactionId = x.Id,
               PartyId = x.PartyId,
               PartyName = x.Party.PartyName,
               PartyTypeString = _genericRepository.Get<ReferenceDataValues>(r => r.Id == x.Party.PartyTypId).EntityValue,
               TransactionAmount = x.Amount,
               TransactionType = x.TransactionType,
               CreatedAt = x.CreatedAt,
               CreateDate = x.CreatedAt.ToString("dd MMM yyyy h:mm tt"),
               UpdatedAt = x.UpdatedAt,
               Description = x.Description,
               DueDateString = x.DueDate?.ToString("dd MMM yyyy"),
           }).OrderByDescending(x => x.CreatedAt).ToList();
        return EntriesList;
    }

    public List<TransactionEntryViewModel> GetUpcomingDues(int businessId)
    {
        List<TransactionEntryViewModel> EntriesList = _genericRepository.GetAll<LedgerTransactions>(x => x.Party.BusinessId == businessId && !x.DeletedAt.HasValue && x.DueDate.HasValue && x.DueDate > DateTime.Now.AddDays(-1) && x.DueDate < DateTime.Now.AddDays(6),
        includes: new List<Expression<Func<LedgerTransactions, object>>>
           {
                x => x.Party
           }).Select(x => new TransactionEntryViewModel
           {
               TransactionId = x.Id,
               PartyId = x.PartyId,
               PartyName = x.Party.PartyName,
               TransactionAmount = x.Amount,
               PartyTypeString = _genericRepository.Get<ReferenceDataValues>(r => r.Id == x.Party.PartyTypId).EntityValue,
               TransactionType = x.TransactionType,
               CreatedAt = x.CreatedAt,
               CreateDate = x.CreatedAt.ToString("dd MMM yyyy h:mm tt"),
               UpdatedAt = x.UpdatedAt,
               Description = x.Description,
               DueDate = x.DueDate,
               DueDateString = x.DueDate?.ToString("dd MMM yyyy"),
           }).ToList();
        return EntriesList;
    }

    public List<decimal> GetPartyRevenue(int businessId, string year = null)
    {
        List<decimal> partyRevenue = new();
        List<PartyViewModel> CustomersList = GetPartiesByType(PartyType.Customer, businessId, "", "-1", "-1");
        List<PartyViewModel> SuppliersList = GetPartiesByType(PartyType.Supplier, businessId, "", "-1", "-1");
        List<PartyViewModel> Parties = CustomersList.Concat(SuppliersList).ToList();
        List<TransactionEntryViewModel> transactions = GetAllTransaction(businessId);

        for (int i = 1; i <= 12; i++)
        {
            decimal monthParty = 0 + transactions.Where(x => x.UpdatedAt != null && x.UpdatedAt.Value.Month == i && x.UpdatedAt.Value.Year.ToString() == year && x.TransactionType == (byte)EnumHelper.TransactionType.GOT)
                                .Sum(x => x.TransactionAmount)
                                - transactions.Where(x => x.UpdatedAt != null && x.UpdatedAt.Value.Month == i && x.UpdatedAt.Value.Year.ToString() == year && x.TransactionType == (byte)EnumHelper.TransactionType.GAVE)
                                .Sum(x => x.TransactionAmount)
                                + transactions.Where(x => x.UpdatedAt == null && x.CreatedAt.Month == i && x.CreatedAt.Year.ToString() == year && x.TransactionType == (byte)EnumHelper.TransactionType.GOT)
                                .Sum(x => x.TransactionAmount)
                                - transactions.Where(x => x.UpdatedAt == null && x.CreatedAt.Month == i && x.CreatedAt.Year.ToString() == year && x.TransactionType == (byte)EnumHelper.TransactionType.GAVE)
                                .Sum(x => x.TransactionAmount);
            // decimal? monthParty = Parties.Where(x => x.CreateMonth == i).Sum(x => x.Amount);
            partyRevenue.Add((decimal)monthParty);
        }
        return partyRevenue;
    }

    public async Task<ApiResponse<int>> SettleUp(decimal netBalance, int partyId, int userId, Businesses business)
    {
        if (partyId == 0 || userId == 0 || business.Id == 0 || netBalance == 0)
        {
            return new ApiResponse<int>(false, Messages.ExceptionMessage, 0, HttpStatusCode.BadRequest);
        }
        PartyViewModel partyViewModel = GetPartyById(partyId);
        if (!_userBusinessMappingService.HasPermission(business.Id, userId, partyViewModel.PartyTypeString.ToString()))
        {
            return new ApiResponse<int>(false, Messages.ForbiddenMessage, partyId, HttpStatusCode.Forbidden);
        }
        TransactionEntryViewModel transactionEntryVM = new();
        transactionEntryVM.PartyId = partyId;
        transactionEntryVM.TransactionAmount = Math.Abs(netBalance);
        transactionEntryVM.BusinessName = business.BusinessName;
        transactionEntryVM.IsSettleup = true;
        //entry in Gave
        if (netBalance < 0)
            transactionEntryVM.TransactionType = (byte)EnumHelper.TransactionType.GAVE;
        //got
        else
            transactionEntryVM.TransactionType = (byte)EnumHelper.TransactionType.GOT;
        int transactionId = await SaveTransactionEntry(transactionEntryVM, userId);
        if (transactionId != 0)
            return new ApiResponse<int>(true, Messages.SettleUpMessage, partyId, HttpStatusCode.OK);
        else
            return new ApiResponse<int>(false, Messages.SettleUpFailMessage, partyId, HttpStatusCode.BadRequest);
    }

    public ApiResponse<string> SendReminder(decimal netBalance, int partyId, int userId, Businesses business)
    {
        if (partyId == 0 || userId == 0 || business.Id == 0 || netBalance == 0)
        {
            return new ApiResponse<string>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        PartyViewModel partyViewModel = GetPartyById(partyId);
        if (!_userBusinessMappingService.HasPermission(business.Id, userId, partyViewModel.PartyTypeString.ToString()))
        {
            return new ApiResponse<string>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
        }
        _ = CommonMethods.Sendreminder(partyViewModel.Email, partyViewModel.PartyTypeString, business.BusinessName, netBalance);
        return new ApiResponse<string>(true, Messages.ReminderSentMessage, null, HttpStatusCode.OK);
    }

    public ApiResponse<TotalAmountViewModel> GetTotalByPartyType(EnumHelper.PartyType partyType, int userId, int businessId)
    {
        if (userId == 0 || businessId == 0)
        {
            return new ApiResponse<TotalAmountViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        if (!_userBusinessMappingService.HasPermission(businessId, userId, partyType.ToString()))
        {
            return new ApiResponse<TotalAmountViewModel>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
        }
        List<PartyViewModel> parties = GetPartiesByType(partyType.ToString(), businessId, "", "-1", "-1");

        TotalAmountViewModel totalAmountViewModel = new();
        totalAmountViewModel.AmountToGet = 0;
        totalAmountViewModel.AmountToGive = 0;
        if (parties.Count != 0)
        {
            foreach (PartyViewModel party in parties)
            {
                if (party.TransactionType == EnumHelper.TransactionType.GAVE)
                    totalAmountViewModel.AmountToGet += (decimal)party.Amount;
                else
                    totalAmountViewModel.AmountToGive += (decimal)party.Amount;
            }
        }
        return new ApiResponse<TotalAmountViewModel>(true, null, totalAmountViewModel, HttpStatusCode.OK);
    }

    public ApiResponse<CookiesViewModel> CheckRolepermission(int businessId, int userId)
    {
        if (businessId == 0 || userId == 0)
        {
            return new ApiResponse<CookiesViewModel>(false, null, null, HttpStatusCode.ServiceUnavailable);
        }
        CookiesViewModel cookiesViewModel = new();
        List<RoleViewModel> rolesByUser = _userBusinessMappingService.GetRolesByBusinessId(businessId, userId);
        if (rolesByUser.Any(role => role.RoleName == ConstantVariables.OwnerRole))
        {
            cookiesViewModel.CustomerPermission = true;
            cookiesViewModel.SupplierPermission = true;
        }
        else if (rolesByUser.Any(role => role.RoleName == ConstantVariables.PurchaseManagerRole))
            cookiesViewModel.SupplierPermission = true;
        else if (rolesByUser.Any(role => role.RoleName == ConstantVariables.SalesManagerRole))
            cookiesViewModel.CustomerPermission = true;
        else
        {
            cookiesViewModel.CustomerPermission = false;
            cookiesViewModel.SupplierPermission = false;
        }
        return new ApiResponse<CookiesViewModel>(true, null, cookiesViewModel, HttpStatusCode.OK);
    }
}