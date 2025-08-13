using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IPartyService
{
    ApiResponse<List<PartyViewModel>> GetParties(string partyType, int businessId, int userId, string searchText, string? filter = "-1", string? sort = "-1");
    Task<ApiResponse<SavePartyViewModel>> SaveParty(SavePartyViewModel partyViewModel, int userId, Businesses business);
    Task<int> SavePartyDetails(SavePartyViewModel partyViewModel, int userId);
    string GetEmailVerifiactionTokenForParty(int partyId);
    Task<ApiResponse<PartyVerifiedViewModel>> PartyEmailVerification(string verificationCode);
    bool IsPartyverified(int partyId);
    bool IsEmailChanged(SavePartyViewModel partyViewModel);
    List<PartyViewModel> GetPartiesByType(string partyType, int businessId, string searchText, string? filter, string? sort);
    Task<ApiResponse<int>> SaveTransactionEntryWithPermission(TransactionEntryViewModel transactionEntryViewModel, int userId, Businesses business);
    Task<int> SaveTransactionEntry(TransactionEntryViewModel transactionEntryViewModel, int userId);
    PartyViewModel GetPartyById(int partyId);
    ApiResponse<LedgerEntriesViewModel> GetTransactionsByPartyId(int partyId, int businessId, int userId);
    ApiResponse<TransactionEntryViewModel> GetTransactionDetailById(int businessId, int userId, int partyId, EnumHelper.TransactionType? transactionType, int? transactionId);
    TransactionEntryViewModel GetTransactionbyTransactionId(int transactionId);
    ApiResponse<int> DeleteTransaction(int transactionId, int userId, int businessId);
    List<Parties> GetAllPartiesByBusiness(int businessId, int userId);
    List<TransactionEntryViewModel> GetAllTransaction(int businessId);
    List<TransactionEntryViewModel> GetUpcomingDues(int businessId);
    List<decimal> GetPartyRevenue(int businessId, string year = null);
    Task<ApiResponse<int>> SettleUp(decimal netBalance, int partyId, int userId, Businesses business);
    ApiResponse<string> SendReminder(decimal netBalance, int partyId, int userId, Businesses business);
    ApiResponse<CookiesViewModel> CheckRolepermission(int businessId, int userId);
    ApiResponse<PartyViewModel> GetpartyByIdResponse(int partyId, int businessId, int userId);
    ApiResponse<TotalAmountViewModel> GetTotalByPartyType(EnumHelper.PartyType partyType, int userId, int businessId);
}