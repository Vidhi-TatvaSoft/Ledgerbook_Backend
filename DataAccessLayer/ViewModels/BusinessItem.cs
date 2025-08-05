using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.ViewModels;

public class BusinessItem
{
    public int? BusinessId { get; set; }

    [Required(ErrorMessage = MessageHelper.BusinessNameRequireMessage)]
    [StringLength(100, ErrorMessage = MessageHelper.BusinessNameLengthMessage)]
    public string BusinessName { get; set; }
    public IFormFile BusinessLogoIForm { get; set; }
    public int BusinescategoryId { get; set; }
    public int BusinessTypeId { get; set; }
    // public AddressViewModel? BusinessAddress { get; set; }

    [Required(ErrorMessage = MessageHelper.MobileNumberRequire)]
    [Range(1000000000, 9999999999, ErrorMessage = MessageHelper.MobileNumberlength)]
    public long MobileNumber { get; set; }

    public bool IsActive { get; set; }

    [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$",
              ErrorMessage = MessageHelper.InvalidGSTIN)]
    public string? GSTIN { get; set; } = null;

    public List<ReferenceDataValues> BusinessCategories { get; set; }

    public List<ReferenceDataValues> BusinessTypes { get; set; }
    // public int AddressId { get; set; }
    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    // [Required(ErrorMessage = MessageHelper.PincodeRequire)]
    [Range(100000, 999999, ErrorMessage = MessageHelper.Pincodelength)]
    public int? Pincode { get; set; } 

    public AttachmentViewModel? BusinessLogoAttachment { get; set; }

}