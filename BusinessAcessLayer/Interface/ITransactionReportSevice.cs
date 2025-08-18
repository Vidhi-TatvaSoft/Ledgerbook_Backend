using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAcessLayer.Interface;

public interface ITransactionReportSevice
{
    ApiResponse<ReportCountsViewModel> GetReportCounts(int businessId);
    ApiResponse<List<PartyViewModel>> GetSearchPartyOptions(int businessId, int userId, string partyType, string searchText = "");
    ApiResponse<ReportTransactionEntriesViewModel> GetReportTransactionEntries(int businessId, int userId, string partyType, int searchPartyId = 0, string startDate = "", string endDate = "");
    ApiResponse<ReportTransactionEntriesViewModel> GetReportdata(string partytype, string timePeriod, Businesses curBusiness, int userId, int searchPartyId = 0, string startDate = "", string endDate = "");
    Task<FileContentResult> GetExcelData(string partytype, string timePeriod, Businesses business, int userId, int searchPartyId = 0, string startDate = "", string endDate = "");
    Task<byte[]> ExportData(ReportTransactionEntriesViewModel reportExcel);
}
