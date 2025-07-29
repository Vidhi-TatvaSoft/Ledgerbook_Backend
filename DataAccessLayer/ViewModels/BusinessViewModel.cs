namespace DataAccessLayer.ViewModels;

public class BusinessViewModel
{
    public int BusinessId { get; set; }

    public string BusienssName { get; set; }

    public string LogoPath { get; set; }

    public string OwnerId { get; set; }

    public string OwnerName { get; set; }
    public int CurentUserId { get; set; }
    public bool CanEditDelete { get; set; }
    public bool CanDelete{ get; set; }
}