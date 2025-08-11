namespace DataAccessLayer.ViewModels;

public class CookiesViewModel
{
    public string? ProfilePhoto { get; set; }
    public string? UserName { get; set; }
    public string? UserToken { get; set; }
    public string? BusinessId { get; set; }
    public string? BusinessToken { get; set; }
    public string? AllBusinesses { get; set; }
    public bool CustomerPermission { get; set; } = false;
    public bool SupplierPermission { get; set; } = false;
}
