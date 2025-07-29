using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IPartyService
{
    Task<int> SavePartyDetails(PartyViewModel partyViewModel, int userId, string partyType);
    string GetEmailVerifiactionTokenForParty(int partyId);
    Task<bool> PartyEmailVerification(PartyVerifiedViewModel partyVerifiedViewModel);
    bool IsPartyverified(int partyId);
    bool IsEmailChanged(PartyViewModel partyViewModel);
    List<PartyViewModel> GetPartiesByType(string partyType, int businessId, string searchText, string? filter, string? sort);
    Task<int> SaveTransactionEntry(TransactionEntryViewModel transactionEntryViewModel, int userId);
    PartyViewModel GetPartyById(int partyId);
    List<TransactionEntryViewModel> GetTransactionsByPartyId(int partyId);
    TransactionEntryViewModel GetTransactionbyTransactionId(int transactionId);
    int DeleteTransaction(int transactionId, int userId);
    List<Parties> GetAllPartiesByBusiness(int businessId, int userId);
    List<TransactionEntryViewModel> GetAllTransaction(int businessId);
    List<TransactionEntryViewModel> GetUpcomingDues(int businessId);
    List<decimal> GetPartyRevenue(int businessId, string year = null);
}