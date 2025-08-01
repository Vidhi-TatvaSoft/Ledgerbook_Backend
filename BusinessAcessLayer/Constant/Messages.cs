namespace BusinessAcessLayer.Constant;

public class Messages
{
    //validation messages
    public static string RequireMessage = "{0} is required";
    public static string InvalidCredentilMessage = "Enter Valid Credentials";
    public static string RegistrationSuccessMessage = "Registration successfull. Check your email for verification.";
    public static string NotVerifiedEmailMessae = "Email not verified. Check your email for verificaiton.";
    public static string EmailExistMessage = "An account with this email already exists. Please sign in or use a different email.";
    public static string EmailDoesNotExistMessage = "Email does not Exist. Please register your email.";
    public static string SamePasswordsMessage = "Password and Confirm password should be same.";
    public static string ExceptionMessage = "Something went wrong. Please try again later.";
    public static string UnhandledExceptionMessage = "An unhandled exception occurred.";
    public static string ResponseStartedLogWarning = "Response already started, cannot Redirect.";
    public static string SendResetPasswordMailSuccess = "Check your email to reset the password of your account.";
    public static string PartyAddedWithOpeningBalance = "Party added with opening balance successfully.";
    public static string InvalidTokenBusinessNotFound = "Invalid token or business not found.";

    //global messages
    public static string GlobalAddUpdateMesage = "{0} {1} successfully.";
    public static string GlobalAddUpdateFailMessage = "Failed to {0} {1}. Try again!";
    public static string VerificationSuccessMessage = "Email has been verified successfully.";
    public static string VerificationErrorMessage = "Verification failed.";

    //reset password
    public static string InvalidResetPasswordLink = "Invalid reset password link.";
    public static string LinkAlreadyUsedMessage = "You have already changed the password once.";
    public static string SamePasswordsErrorMessage = "New password and Previous password can't be same.";
    public static string UserNotExistMessage = "User does not exist. Create an account first.";

    //profile
    public static string InvalidImageExtensionMessage = "Please Upload an Image in JPEG, PNG or JPG format.";

    //change password
    public static string IncorrectOldPAssword = "Current pasword is incorrect.";

    //send reminder to party for due payment
    public static string ReminderSentMessage = "Reminder sent successfully.";

    //settleup
    public static string SettleUpMessage = "Balance settled up successfully.";
    public static string SettleUpFailMessage = "Failed to settle up balance.";

    //activity logs messages
    public static string UserActivity = "{0} has been {1}.";
    public static string BusinessActivity = "{0} has been {1} by {2}.";
    public static string PartyActivity = "{0} '{1}' has been {2} to business '{3}' by {4}.";
    public static string PartyUpdateActivity = "{0} '{1}' in business '{2}' has been {3} by {4}.";
    public static string PartyDeleteActivity = "{0} '{1}' has been removed from business '{3}' by {4}.";
    public static string TransactionAddActivity = "{0} {1} has processed a transaction of ₹{2} for business '{3}' by {4}.";
    public static string TransactionActivity = "Transaction for {0} '{1}' in business '{2}' has been {3} by {4}.";
    public static string AddUserInBusinessActivity = "New user '{0}' invited by {1}.";
    public static string UserInBusinessActivity = "User '{0}' {1} by {2}.";
}