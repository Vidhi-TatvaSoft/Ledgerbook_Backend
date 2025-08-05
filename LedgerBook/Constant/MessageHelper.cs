namespace LedgerBook.Constant;

public class MessageHelper
{
    /************validation messages *******************/
    //require messages
    public const string EmailRequireMessage = "Email is required.";
    public const string PasswordRequireMessage = "Password is required.";
    public const string CurrentPasswordRequireMessage = "Current Password is required.";
    public const string ConfirmPasswordRequireMessage = "Confirm password is required.";
    public const string LastNameRequireMessage = "Last Name is required.";
    public const string FirstNameRequireMessage = "First Name is required.";
    public const string BusinessNameRequireMessage = "Business Name is required.";
    public const string MobileNumberRequire = "Mobile Number is required.";
    public const string PincodeRequire = "Pincode is required.";
    public const string PartyNameRequire = "Party Name is required.";
    public const string TransactionAmountRequire = "Amount is required.";


    //validations
    public const string ValidEmailMessage = "Please enter a valid Email.";
    public const string EmailLengthMessage = "Email can not exceed 255 characters.";
    public const string minLengthPasswordMessage = "Password must be at least 6 characters long.";
    public const string minLengthPasswordRegisterMessage = "Password must be at least 8 characters long.";
    public const string maxLengthPasswordMessage = "Password can not exceed 128 characters.";
    public const string InvalidPasswordMessage = "Password must contain at least one uppercase letter, number, special character and should not contain whitespace.";
    public const string minLengthConfirmPasswordMessage = "Confirm Password must be at least 8 characters long.";
    public const string maxLengthConfirmPasswordMessage = "Confirm Password cannot exceed 128 characters.";
    public const string InvalidConfirmPasswordMessage = "Confirm Password must contain at least one uppercase letter, number, special character and should not contain whitespace.";
    public const string comparePasswords = "Password and Confirm Password should be same.";
    public const string InvalidLastNameMessage = "Last Name must contain only alphabets.";
    public const string InvalidFirstNameMessage = "First Name must contain only alphabets.";
    public const string FirstNameLengthMessage = "First Name can not exceed 50 characters.";
    public const string LastNameLengthMessage = "Last Name can not exceed 50 characters.";
    public const string BusinessNameLengthMessage = "Business Name can not exceedd 100 characters.";
    public const string PartyNameNameLengthMessage = "Business Name can not exceedd 100 characters.";
    public const string MobileNumberlength = "Mobile Number must be 10 digits long.";
    public const string Pincodelength = "Pincode must be 6 digits long.";
    public const string TransactionAmountValidation = "Amount should be in between 1 to 10000.";

    public const string InvalidGSTIN = "Enter valid GST Number";

}