using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAcessLayer.Interface;

public interface ITransactionReportSevice
{
    ApiResponse<ReportCountsViewModel> GetReportCounts(int businessId);
    ApiResponse<ReportTransactionEntriesViewModel> GetReportTransactionEntries(int businessId, string partyType, int searchPartyId = 0, string startDate = "", string endDate = "");
    ApiResponse<ReportTransactionEntriesViewModel> GetReportdata(string partytype, string timePeriod, Businesses curBusiness, int searchPartyId = 0, string startDate = "", string endDate = "");
    Task<FileContentResult> GetExcelData(string partytype, string timePeriod, Businesses business, int searchPartyId = 0, string startDate = "", string endDate = "");
    Task<byte[]> ExportData(ReportTransactionEntriesViewModel reportExcel);
}
