using System.Drawing;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BusinessAcessLayer.Services;

public class TransactionReportService : ITransactionReportSevice
{
    private readonly IGenericRepo _genericRepository;
    private readonly LedgerBookDbContext _context;
    private readonly IBusinessService _businessService;
    private readonly IPartyService _partyService;
    private readonly IUserBusinessMappingService _userBusinessMappingService;

    public TransactionReportService(LedgerBookDbContext context,
     IGenericRepo genericRepo,
     IBusinessService businessService,
     IPartyService partyService,
     IUserBusinessMappingService userBusinessMappingService
     )
    {
        _genericRepository = genericRepo;
        _context = context;
        _businessService = businessService;
        _partyService = partyService;
        _userBusinessMappingService = userBusinessMappingService;
    }

    public ApiResponse<ReportCountsViewModel> GetReportCounts(int businessId)
    {
        if (businessId == 0)
        {
            return new ApiResponse<ReportCountsViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        int customerTypeId = _genericRepository.Get<ReferenceDataValues>(x => x.EntityType.EntityType == ConstantVariables.PartyType && x.EntityValue == PartyType.Customer,
         includes: new List<Expression<Func<ReferenceDataValues, object>>>
            {
                x => x.EntityType
            })!.Id;
        int supplierTypeId = _genericRepository.Get<ReferenceDataValues>(x => x.EntityType.EntityType == ConstantVariables.PartyType && x.EntityValue == PartyType.Supplier,
    includes: new List<Expression<Func<ReferenceDataValues, object>>>
       {
                x => x.EntityType
       })!.Id;

        ReportCountsViewModel reportCountsViewModel = new();
        reportCountsViewModel.CustomerCount = _genericRepository.GetAll<Parties>(x => x.PartyTypId == customerTypeId && x.BusinessId == businessId && x.DeletedAt == null).Count();
        reportCountsViewModel.SupplierCount = _genericRepository.GetAll<Parties>(x => x.PartyTypId == supplierTypeId && x.BusinessId == businessId && x.DeletedAt == null).Count();
        return new ApiResponse<ReportCountsViewModel>(true, null, reportCountsViewModel, HttpStatusCode.OK);
    }

    public ApiResponse<List<PartyViewModel>> GetSearchPartyOptions(int businessId, int userId, string partyType, string searchText = "")
    {
        if (partyType.IsNullOrEmpty() || businessId == 0 || userId == 0)
        {
            return new ApiResponse<List<PartyViewModel>>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        if (!_userBusinessMappingService.HasPermission(businessId, userId, partyType))
        {
            return new ApiResponse<List<PartyViewModel>>(true, Messages.ForbiddenMessage, null, HttpStatusCode.OK);
        }
        List<PartyViewModel> parties = _partyService.GetPartiesByType(partyType, businessId, searchText, "-1", "-1");
        return new ApiResponse<List<PartyViewModel>>(true, null, parties, HttpStatusCode.OK);
    }

    public ApiResponse<ReportTransactionEntriesViewModel> GetReportTransactionEntries(int businessId, int userId, string partyType, int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        if (businessId == 0 || partyType == "" || userId == 0)
        {
            return new ApiResponse<ReportTransactionEntriesViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        if (!_userBusinessMappingService.HasPermission(businessId, userId, partyType))
        {
            return new ApiResponse<ReportTransactionEntriesViewModel>(false, Messages.ForbiddenMessage, null, HttpStatusCode.BadRequest);
        }
        List<LedgerTransactions> Entries = new();
        int partyTypeId = _genericRepository.Get<ReferenceDataValues>(x => x.EntityType.EntityType == ConstantVariables.PartyType && x.EntityValue == partyType,
            includes: new List<Expression<Func<ReferenceDataValues, object>>>
            {
                        x => x.EntityType
            })!.Id;
        if (searchPartyId == 0)
        {
            Entries = _genericRepository.GetAll<LedgerTransactions>(x => x.Party.BusinessId == businessId && x.Party.PartyTypId == partyTypeId && !x.DeletedAt.HasValue,
             includes: new List<Expression<Func<LedgerTransactions, object>>>
                {
                    x => x.Party
                }).ToList();
        }
        else
        {
            Entries = _genericRepository.GetAll<LedgerTransactions>(x => x.Party.BusinessId == businessId && x.Party.PartyTypId == partyTypeId && x.PartyId == searchPartyId && !x.DeletedAt.HasValue,
             includes: new List<Expression<Func<LedgerTransactions, object>>>
                {
                    x => x.Party
                }).ToList();
        }
        List<TransactionEntryViewModel> EntriesList = Entries.Select(x => new TransactionEntryViewModel
        {
            TransactionId = x.Id,
            PartyId = x.PartyId,
            TransactionAmount = x.Amount,
            TransactionType = x.TransactionType,
            CreatedAt = x.CreatedAt,
            CreateDate = x.CreatedAt.ToString("dd MMMM yyyy"),
            UpdatedAt = x.UpdatedAt,
            Balance = 0,
            PartyName = _genericRepository.Get<Parties>(y => y.Id == x.PartyId && !y.DeletedAt.HasValue).PartyName,
            Description = x.Description,
            DueDate = x.DueDate
        }).OrderByDescending(x => x.CreatedAt).ToList();

        List<LedgerTransactions> allEntriesOfParty = new();
        if (searchPartyId == 0)
        {
            allEntriesOfParty = _genericRepository.GetAll<LedgerTransactions>(x => x.Party.BusinessId == businessId && x.Party.PartyTypId == partyTypeId && !x.DeletedAt.HasValue,
             includes: new List<Expression<Func<LedgerTransactions, object>>>
                {
                    x => x.Party
                }).ToList();
        }
        else
        {
            allEntriesOfParty = _genericRepository.GetAll<LedgerTransactions>(x => x.Party.BusinessId == businessId && x.Party.PartyTypId == partyTypeId && x.PartyId == searchPartyId && !x.DeletedAt.HasValue,
             includes: new List<Expression<Func<LedgerTransactions, object>>>
                {
                    x => x.Party
                }).ToList();
        }
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
        if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
        {
            EntriesList = EntriesList.Where(x => x.CreatedAt >= DateTime.Parse(startDate) && x.CreatedAt <= DateTime.Parse(endDate)).ToList();
        }
        ReportTransactionEntriesViewModel reportTransactionEntriesVM = new()
        {
            TransactionsList = EntriesList,
            youGave = EntriesList.Where(x => x.TransactionType == (byte)EnumHelper.TransactionType.GAVE).Sum(x => x.TransactionAmount),
            YouGot = EntriesList.Where(x => x.TransactionType == (byte)EnumHelper.TransactionType.GOT).Sum(x => x.TransactionAmount),
        };
        reportTransactionEntriesVM.NetBalance = reportTransactionEntriesVM.YouGot - reportTransactionEntriesVM.youGave;
        return new ApiResponse<ReportTransactionEntriesViewModel>(true, null, reportTransactionEntriesVM, HttpStatusCode.OK); ;
    }

    public async Task<FileContentResult> GetExcelData(string partytype, string timePeriod, Businesses business, int userId, int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        var reportData = GetReportdata(partytype, timePeriod, business, userId, searchPartyId, startDate, endDate);
        ReportTransactionEntriesViewModel reportExcel = new();
        if ((bool)reportData.IsSuccess)
        {
            reportExcel = reportData.Result;
        }
        byte[] FileData = await ExportData(reportExcel);
        FileContentResult result = new FileContentResult(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = "TransactionReport_" + reportExcel.Startdate + "_to_" + reportExcel.EndDate + ".xlsx"
        };
        return result;
    }

    public Task<byte[]> ExportData(ReportTransactionEntriesViewModel reportExcel)
    {
        // Create Excel package
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (ExcelPackage package = new ExcelPackage())
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Transactions");
            int currentRow = 1;
            int currentCol = 1;

            // this is first row....................................
            worksheet.Cells[currentRow, currentCol, currentRow, currentCol + 2].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = reportExcel.Businessname;

            currentCol += 4;
            worksheet.Cells[currentRow, currentCol, currentRow, currentCol + 4].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = reportExcel.Startdate + " to " + reportExcel.EndDate;

            if (reportExcel.PartyId != 0)
            {
                currentRow += 2;
                currentCol = 1;
                worksheet.Cells[currentRow, currentCol].Value = "party:";

                currentCol += 1;
                worksheet.Cells[currentRow, currentCol].Value = reportExcel.PartyName;

                currentRow++;
                currentCol = 1;
                worksheet.Cells[currentRow, currentCol].Value = "Total Debit(-):";

                currentCol++;
                worksheet.Cells[currentRow, currentCol].Value = reportExcel.youGave;

                currentRow++;
                currentCol = 1;
                worksheet.Cells[currentRow, currentCol].Value = "Total Credit(+):";

                currentCol++;
                worksheet.Cells[currentRow, currentCol].Value = reportExcel.YouGot;

                currentRow++;
                currentCol = 1;
                worksheet.Cells[currentRow, currentCol].Value = "Net Balance:";

                currentCol++;
                worksheet.Cells[currentRow, currentCol].Value = reportExcel.NetBalance;
            }

            currentRow += 2;

            // this is table ....................................
            int headingRow = currentRow++;
            int headingCol = 1;

            worksheet.Cells[headingRow, headingCol].Value = "Sr No.";
            headingCol++;

            worksheet.Cells[headingRow, headingCol].Value = "Date";
            headingCol++;

            worksheet.Cells[headingRow, headingCol].Value = "Details";
            headingCol++;

            worksheet.Cells[headingRow, headingCol].Value = "Debit(-)";
            headingCol++;

            worksheet.Cells[headingRow, headingCol].Value = "Credit(+)";
            headingCol++;

            if (reportExcel.PartyId != 0)
            {
                worksheet.Cells[headingRow, headingCol].Value = "Balance";
                headingCol++;
            }

            // Populate data
            int row = headingRow + 1;
            int count = 1;
            if (reportExcel.TransactionsList.Count != 0)
            {
                foreach (TransactionEntryViewModel transaction in reportExcel.TransactionsList)
                {
                    int startCol = 1;

                    worksheet.Cells[row, startCol].Value = count;
                    startCol += 1;

                    worksheet.Cells[row, startCol].Value = transaction.PartyName;
                    startCol += 1;

                    worksheet.Cells[row, startCol].Value = transaction.Description == null ? "-" : transaction.Description;
                    startCol += 1;
                    if (transaction.TransactionType == (byte)EnumHelper.TransactionType.GAVE)
                    {
                        worksheet.Cells[row, startCol].Value = transaction.TransactionAmount;
                        startCol += 1;

                        worksheet.Cells[row, startCol].Value = 0;
                        startCol += 1;
                    }
                    else
                    {
                        worksheet.Cells[row, startCol].Value = 0;
                        startCol += 1;

                        worksheet.Cells[row, startCol].Value = transaction.TransactionAmount;
                        startCol += 1;
                    }

                    if (reportExcel.PartyId != 0)
                    {
                        worksheet.Cells[row, startCol].Value = transaction.Balance;
                        startCol += 1;
                    }
                    row++;
                    count++;
                }
            }
            else
            {
                int startCol = 1;
                worksheet.Cells[row, startCol, row, startCol + 4].Merge = true;
                worksheet.Cells[row, startCol].Value = "No entries found";
                worksheet.Cells[row, startCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                row++;

            }
            headingCol = 1;

            worksheet.Cells[row, headingCol, row, headingCol + 2].Merge = true;
            worksheet.Cells[row, headingCol].Value = "Grand Total";

            headingCol += 3;
            worksheet.Cells[row, headingCol].Value = reportExcel.youGave;

            headingCol++;
            worksheet.Cells[row, headingCol].Value = reportExcel.YouGot;

            if (reportExcel.PartyId == 0)
            {
                row++;
                headingCol = 1;
                worksheet.Cells[row, headingCol, row, headingCol + 2].Merge = true;
                worksheet.Cells[row, headingCol].Value = "Balance";

                headingCol += 3;
                worksheet.Cells[row, headingCol, row, headingCol + 1].Merge = true;
                worksheet.Cells[row, headingCol].Value = reportExcel.NetBalance;
            }
            else
            {
                headingCol++;
                worksheet.Cells[row, headingCol].Value = reportExcel.NetBalance;
            }

            //  It creates a Task that is already completed and contains the specified result 
            // (in this case, the byte array).
            // This is useful when you need to return a Task in an asynchronous method but already have 
            // the result available synchronously.
            return Task.FromResult(package.GetAsByteArray());

        }

    }

    public ApiResponse<ReportTransactionEntriesViewModel> GetReportdata(string partytype, string timePeriod, Businesses curBusiness, int userId, int searchPartyId = 0, string startDate = "", string endDate = "")
    {
        if (partytype.IsNullOrEmpty() || curBusiness == null)
        {
            return new ApiResponse<ReportTransactionEntriesViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        ReportTransactionEntriesViewModel reportVM = new();

        reportVM.BusinessId = curBusiness.Id;
        reportVM.Businessname = curBusiness.BusinessName;

        reportVM.TimePeriod = timePeriod;
        reportVM.PartyId = searchPartyId;
        if (searchPartyId != 0)
        {
            reportVM.PartyName = _partyService.GetPartyById(searchPartyId).PartyName;
        }
        else
        {
            reportVM.PartyName = "Account";
        }
        ApiResponse<ReportTransactionEntriesViewModel> transactionReportData = GetReportTransactionEntries(reportVM.BusinessId, userId, partytype, searchPartyId, startDate, endDate);
        if ((bool)transactionReportData.IsSuccess && transactionReportData.Result != null)
        {
            reportVM.TransactionsList = transactionReportData.Result.TransactionsList;
            reportVM.youGave = transactionReportData.Result.youGave;
            reportVM.YouGot = transactionReportData.Result.YouGot;
        }
        reportVM.NetBalance = reportVM.YouGot - reportVM.youGave;
        DateTime startDateTemp;
        DateTime endDateTemp;
        if (!startDate.IsNullOrEmpty())
        {
            startDateTemp = DateTime.Parse(startDate);
        }
        else
        {
            startDateTemp = DateTime.Now;
        }
        if (!endDate.IsNullOrEmpty())
        {
            endDateTemp = DateTime.Parse(endDate);
        }
        else
        {
            endDateTemp = DateTime.Now;
        }
        reportVM.Startdate = startDateTemp.ToString("dd MMMM yyyy");
        reportVM.EndDate = endDateTemp.ToString("dd MMMM yyyy");
        return new ApiResponse<ReportTransactionEntriesViewModel>(true, null, reportVM, HttpStatusCode.OK);
    }
}
