using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Constant;

namespace DataAccessLayer.ViewModels;

public class UserViewmodel
{
    public int UserId { get; set; }

    public int? PersonalDetailId{ get; set; }

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

    [Required(ErrorMessage = MessageHelper.MobileNumberRequire)]
    [Range(1000000000, 9999999999, ErrorMessage = MessageHelper.MobileNumberlength)]
    public long MobileNumber { get; set; }

    public int CreatedById { get; set; }

    public bool CanEdit { get; set; }
    public bool CanAddOwner { get; set; }
    public bool IsActive{ get; set; }

    public string Pasword { get; set; }

    public int? RoleId { get; set; }

    public string? RoleName { get; set; }

    public List<RoleViewModel> Roles { get; set; }

    public bool? IsUserRegistered { get; set; }
}