using System.Net;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Services;

public class DashboardService : IDashboardService
{
    private readonly IPartyService _partyService;

    public DashboardService(IPartyService partyService)
    {
        _partyService = partyService;
    }

    public ApiResponse<DashboardViewModel> GetDashboardData(int businessId)
    {
        if (businessId == 0)
        {
            return new ApiResponse<DashboardViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        DashboardViewModel dashboardVM = new();
        List<PartyViewModel> CustomersList = _partyService.GetPartiesByType(PartyType.Customer, businessId, "", "-1", "-1");
        List<PartyViewModel> SuppliersList = _partyService.GetPartiesByType(PartyType.Supplier, businessId, "", "-1", "-1");
        dashboardVM.TopParty = CustomersList.Concat(SuppliersList).OrderBy(x => x.Amount).Take(5).ToList();
        dashboardVM.CustomerAmount = (decimal)CustomersList.Sum(x => x.Amount);
        dashboardVM.CustomerType = dashboardVM.CustomerAmount < 0 ? EnumHelper.TransactionType.GOT : EnumHelper.TransactionType.GAVE;
        dashboardVM.SupplierAmount = (decimal)SuppliersList.Sum(x => x.Amount);
        dashboardVM.SupplierType = dashboardVM.SupplierAmount < 0 ? EnumHelper.TransactionType.GOT : EnumHelper.TransactionType.GAVE;
        dashboardVM.TotalCustomer = CustomersList.Count();
        dashboardVM.TotalSupplier = SuppliersList.Count();
        dashboardVM.NetBalance = dashboardVM.CustomerAmount + dashboardVM.SupplierAmount;
        dashboardVM.NetBalanceTye = dashboardVM.NetBalanceTye < 0 ? EnumHelper.TransactionType.GOT : EnumHelper.TransactionType.GAVE;
        dashboardVM.RecentTransaction = _partyService.GetAllTransaction(businessId)!.Take(3).ToList();
        dashboardVM.UpcomingDue = _partyService.GetUpcomingDues(businessId).OrderBy(x => x.DueDate).ToList();
        return new ApiResponse<DashboardViewModel>(true, null, dashboardVM, HttpStatusCode.OK);
    }

    public ApiResponse<List<string>> Getyears(int businessId)
    {
        if (businessId == 0)
        {
            return new ApiResponse<List<string>>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        List<string> years = new();
        List<TransactionEntryViewModel> transactions = _partyService.GetAllTransaction(businessId);
        foreach (var transaction in transactions)
        {
            if (transaction.UpdatedAt != null)
            {
                years.Add(transaction.UpdatedAt.Value.ToString("yyyy"));
            }
            else
            {
                years.Add(transaction.CreatedAt.ToString("yyyy"));
            }
        }
        years = years.Distinct().ToList();
        return new ApiResponse<List<string>>(true, null, years, HttpStatusCode.OK);
    }
}
