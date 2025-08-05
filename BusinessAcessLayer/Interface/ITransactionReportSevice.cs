using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface ITransactionReportSevice
{
    ReportCountsViewModel GetReportCounts(int businessId);
    List<TransactionEntryViewModel> GetTransactionEntries(int businessId, string partyType, int searchPartyId = 0, string startDate = "", string endDate = "");
    // ReportTransactionEntriesViewModel GetReportdata(string partytype, string timePeriod, int businessId, int searchPartyId = 0, string startDate = "", string endDate = "");
    Task<byte[]> ExportData(ReportTransactionEntriesViewModel reportExcel);
}
