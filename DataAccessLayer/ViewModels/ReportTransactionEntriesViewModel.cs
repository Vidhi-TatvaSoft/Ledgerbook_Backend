namespace DataAccessLayer.ViewModels;

public class ReportTransactionEntriesViewModel
{
    public decimal youGave { get; set; }
    public decimal YouGot { get; set; }
    public decimal NetBalance { get; set; }
    public List<TransactionEntryViewModel> TransactionsList { get; set; }

    //for pdf
    public int BusinessId { get; set; }
    public string Businessname { get; set; }
    public int PartyId{ get; set; }
    public string PartyName { get; set; }
    public string TimePeriod { get; set; }
    public string Startdate{ get; set; }
    public string EndDate{ get; set; }

}
