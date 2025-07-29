using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Constant;

namespace DataAccessLayer.ViewModels;

public class ChangePasswordViewModel
{

    public string Email { get; set; }

    [Required(ErrorMessage = MessageHelper.CurrentPasswordRequireMessage)]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = MessageHelper.minLengthPasswordMessage)]
    [MaxLength(128, ErrorMessage = MessageHelper.maxLengthPasswordMessage)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
              ErrorMessage = MessageHelper.InvalidPasswordMessage)]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = MessageHelper.PasswordRequireMessage)]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = MessageHelper.minLengthPasswordMessage)]
    [MaxLength(128, ErrorMessage = MessageHelper.maxLengthPasswordMessage)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
              ErrorMessage = MessageHelper.InvalidPasswordMessage)]
    public string Password { get; set; }


    [Required(ErrorMessage = MessageHelper.ConfirmPasswordRequireMessage)]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = MessageHelper.minLengthConfirmPasswordMessage)]
    [MaxLength(128, ErrorMessage = MessageHelper.maxLengthConfirmPasswordMessage)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
           ErrorMessage = MessageHelper.InvalidConfirmPasswordMessage)]
    [Compare("Password", ErrorMessage = MessageHelper.comparePasswords)]
    public string ConfirmPassword { get; set; }
}
