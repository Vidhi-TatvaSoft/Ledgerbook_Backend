using DataAccessLayer.Constant;

namespace DataAccessLayer.ViewModels;

public class ActivityDataViewModel
{
    public string EntityType { get; set; }
    public int BusinessId { get; set; }
    public string SubEntityType { get; set; }
    public int PartyId { get; set; }
    public string ActionName { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

}
