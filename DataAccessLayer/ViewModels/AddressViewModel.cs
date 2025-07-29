using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Constant;

namespace DataAccessLayer.ViewModels;

public class AddressViewModel
{
    public int AddressId { get; set; }
    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    // [Required(ErrorMessage = MessageHelper.PincodeRequire)]
    [Range(100000, 999999, ErrorMessage = MessageHelper.Pincodelength)]
    public int? Pincode { get; set; }
}
