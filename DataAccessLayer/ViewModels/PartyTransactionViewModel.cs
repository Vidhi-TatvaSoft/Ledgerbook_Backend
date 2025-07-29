namespace DataAccessLayer.ViewModels;

public class PartyTransactionViewModel
{
    public int BusinessId { get; set; }
    public string BusinessName { get; set; }
    public List<PartyViewModel> Parties { get; set; }
    public PartyViewModel PartyViewModel { get; set; }
    public string? PartyTypeString { get; set; }
    public TransactionEntryViewModel TransactionEntryViewModel { get; set; }
    public LedgerEntriesViewModel LedgerEntriesViewModel { get; set; }
    public TotalAmountViewModel TotalAmountViewModel { get; set; }
}