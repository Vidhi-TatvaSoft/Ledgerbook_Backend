using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Constant;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.ViewModels;

public class UserProfileViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = MessageHelper.FirstNameRequireMessage)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = MessageHelper.InvalidFirstNameMessage)]
    [StringLength(50, ErrorMessage = MessageHelper.FirstNameLengthMessage)]
    public string FirstName { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = MessageHelper.LastNameRequireMessage)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = MessageHelper.InvalidLastNameMessage)]
    [StringLength(50, ErrorMessage = MessageHelper.LastNameLengthMessage)]
    public string LastName { get; set; }

    [Required(ErrorMessage = MessageHelper.EmailRequireMessage)]
    [EmailAddress(ErrorMessage = MessageHelper.ValidEmailMessage)]
    [StringLength(255, ErrorMessage = MessageHelper.EmailLengthMessage)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,8}$", ErrorMessage = MessageHelper.ValidEmailMessage)]
    public string Email { get; set; }

    [Range(1000000000, 9999999999, ErrorMessage = MessageHelper.MobileNumberlength)]
    public long? MobileNumber { get; set; }

    public int? ProfileAttachmentId { get; set; }
    public IFormFile BusinessLogo { get; set; }

    public AttachmentViewModel AttachmentViewModel { get; set; }

}
