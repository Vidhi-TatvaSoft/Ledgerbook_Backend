using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using BusinessAcessLayer.Constant;
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


    public PartyService(LedgerBookDbContext context,
    IAddressService addressService,
    IReferenceDataEntityService referenceDataEntityService,
    IGenericRepo genericRepository,
    IActivityLogService activityLogService,
    IBusinessService businessService,
    IUserService userService,
    IUserBusinessMappingService userBusinessMappingService
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
        return new ApiResponse<List<PartyViewModel>>(false, null, null, HttpStatusCode.Forbidden);

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
                partyList = partyList;
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

    public async Task<int> SavePartyDetails(PartyViewModel partyViewModel, int userId, string partyType)
    {
        string businessName = _businessService.GetBusinessNameById(partyViewModel.BusinessId);
        string userName = _userService.GetuserNameById(userId);
        if (partyViewModel.PartyId == 0)
        {
            Parties party = new();
            party.PartyName = partyViewModel.PartyName;
            party.BusinessId = partyViewModel.BusinessId;
            party.PartyTypId = _genericRepository.Get<ReferenceDataValues>(x => x.EntityType.EntityType == ConstantVariables.PartyType && x.EntityValue == partyType,
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
            string message = string.Format(Messages.PartyActivity, partyType, party.PartyName, "added ", businessName, userName);
            await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Add, EnumHelper.ActivityEntityType.Business, party.BusinessId, userId, EnumHelper.ActivityEntityType.Party, party.Id);
            return party.Id;
        }
        else
        {
            Parties party = _genericRepository.Get<Parties>(x => x.Id == partyViewModel.PartyId && x.DeletedAt == null);
            party.PartyName = partyViewModel.PartyName;
            party.BusinessId = partyViewModel.BusinessId;
            party.PartyTypId = _genericRepository.Get<ReferenceDataValues>(x => x.EntityType.EntityType == ConstantVariables.PartyType && x.EntityValue == partyType,
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
            string message = string.Format(Messages.PartyUpdateActivity, partyType, party.PartyName, businessName, "updated ", userName);
            await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Update, EnumHelper.ActivityEntityType.Business, party.BusinessId, userId, EnumHelper.ActivityEntityType.Party, party.Id);
            return party.Id;
        }
    }

    public string GetEmailVerifiactionTokenForParty(int partyId)
    {
        return _genericRepository.Get<Parties>(x => x.Id == partyId && !x.DeletedAt.HasValue).VerificationToken;
    }

    public async Task<bool> PartyEmailVerification(PartyVerifiedViewModel partyVerifiedViewModel)
    {
        Parties party = _genericRepository.Get<Parties>(x => x.Email.ToLower().Trim() == partyVerifiedViewModel.Email.ToLower().Trim() && x.VerificationToken == partyVerifiedViewModel.Token && x.Id == partyVerifiedViewModel.PartyId && !x.DeletedAt.HasValue);
        if (party != null)
        {
            party.IsEmailVerified = true;
            await _genericRepository.UpdateAsync<Parties>(party);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsPartyverified(int partyId)
    {
        return _genericRepository.IsPresentAny<Parties>(x => x.Id == partyId && !x.DeletedAt.HasValue && x.IsEmailVerified);
    }

    public bool IsEmailChanged(PartyViewModel partyViewModel)
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

    public List<TransactionEntryViewModel> GetTransactionsByPartyId(int partyId)
    {
        List<TransactionEntryViewModel> EntriesList = _genericRepository.GetAll<LedgerTransactions>(x => x.PartyId == partyId && !x.DeletedAt.HasValue)
                        .Select(x => new TransactionEntryViewModel
                        {
                            TransactionId = x.Id,
                            PartyId = x.PartyId,
                            TransactionAmount = x.Amount,
                            TransactionType = x.TransactionType,
                            CreatedAt = x.CreatedAt,
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
        return EntriesList;
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

    public int DeleteTransaction(int transactionId, int userId)
    {
        LedgerTransactions transaction = _genericRepository.Get<LedgerTransactions>(t => t.Id == transactionId && !t.DeletedAt.HasValue);
        PartyViewModel partyViewModel = GetPartyById(transaction.PartyId);
        string businessname = _businessService.GetBusinessNameById(partyViewModel.BusinessId);
        string userName = _userService.GetuserNameById(userId);
        if (transaction != null)
        {
            transaction.DeletedAt = DateTime.UtcNow;
            transaction.DeletedById = userId;
            _genericRepository.Update<LedgerTransactions>(transaction);
            string message = string.Format(Messages.TransactionActivity, partyViewModel.PartyTypeString, partyViewModel.PartyName, businessname, "deleted", userName);
            _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Delete, EnumHelper.ActivityEntityType.Business, partyViewModel.BusinessId, userId, EnumHelper.ActivityEntityType.Transaction, transaction.Id);
            return transaction.PartyId;
        }
        else
        {
            return 0;
        }
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

    public ApiResponse<CookiesViewModel> CheckRolepermission(int businessId, int userId)
    {
        if (businessId == 0 || userId == 0)
        {
            return new ApiResponse<CookiesViewModel>(false, null, null, HttpStatusCode.ServiceUnavailable);
        }
        CookiesViewModel cookiesViewModel = new();
        List<RoleViewModel> rolesByUser = _userBusinessMappingService.GetRolesByBusinessId(businessId, userId);
        if (rolesByUser.Any(role => role.RoleName == "Owner/Admin"))
        {
            cookiesViewModel.CustomerPermission = true;
            cookiesViewModel.SupplierPermission = true;
        }
        else if (rolesByUser.Any(role => role.RoleName == "Purchase Manager"))
            cookiesViewModel.SupplierPermission = true;
        else if (rolesByUser.Any(role => role.RoleName == "Sales Manager"))
            cookiesViewModel.CustomerPermission = true;
        else
        {
            cookiesViewModel.CustomerPermission = false;
            cookiesViewModel.SupplierPermission = false;
        }
        return new ApiResponse<CookiesViewModel>(true, null, cookiesViewModel, HttpStatusCode.OK);
    }
}