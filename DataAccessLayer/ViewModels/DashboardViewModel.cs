using DataAccessLayer.Constant;

namespace DataAccessLayer.ViewModels;

public class DashboardViewModel
{
    public int TotalCustomer { get; set; }
    public int TotalSupplier{ get; set; }
    public decimal CustomerAmount { get; set; }
    public EnumHelper.TransactionType CustomerType { get; set; }
    public decimal SupplierAmount { get; set; }
    public EnumHelper.TransactionType SupplierType { get; set; }
    public decimal NetBalance { get; set; }
    public EnumHelper.TransactionType NetBalanceTye { get; set; }

    public List<TransactionEntryViewModel> RecentTransaction { get; set; }
    public List<TransactionEntryViewModel> UpcomingDue { get; set; }
    public List<PartyViewModel> TopParty { get; set; }

    

}
