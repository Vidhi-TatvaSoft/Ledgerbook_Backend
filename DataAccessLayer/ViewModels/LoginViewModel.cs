using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Constant;

namespace DataAccessLayer.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = MessageHelper.EmailRequireMessage)]
    [EmailAddress(ErrorMessage = MessageHelper.ValidEmailMessage)]
    [StringLength(255, ErrorMessage = MessageHelper.EmailLengthMessage)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,8}$", ErrorMessage = MessageHelper.ValidEmailMessage)]
    public string Email { get; set; }

    [Required(ErrorMessage = MessageHelper.PasswordRequireMessage)]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = MessageHelper.minLengthPasswordMessage)]
    [MaxLength(128, ErrorMessage = MessageHelper.maxLengthPasswordMessage)]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}