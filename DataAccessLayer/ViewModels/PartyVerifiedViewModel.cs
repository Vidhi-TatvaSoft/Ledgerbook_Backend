namespace DataAccessLayer.ViewModels;

public class PartyVerifiedViewModel
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string BusinessName { get; set; }
    public int PartyId { get; set; }
    public string PartyType { get; set; }
    public bool IsEmailVerified { get; set; } = false;

}
