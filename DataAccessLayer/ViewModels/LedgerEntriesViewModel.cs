namespace DataAccessLayer.ViewModels;

public class LedgerEntriesViewModel
{
    public decimal NetBalance { get; set; }
    public bool IspartyVerified { get; set; } = false;
    
    public List<TransactionEntryViewModel> TransactionsList { get; set; }

}
