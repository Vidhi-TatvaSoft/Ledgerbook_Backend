using System.ComponentModel.DataAnnotations;
using LedgerBook.Constant;

namespace LedgerBook.ViewModels;

public class ResetPasswordVM
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
    
    public string? ResetPasswordToken { get; set; }

}
