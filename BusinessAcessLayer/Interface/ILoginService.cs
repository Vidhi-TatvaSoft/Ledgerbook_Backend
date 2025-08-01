using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface ILoginService
{
    Task<ApiResponse<string>> RegisterUser(RegistrationViewModel RegisterVM);
    ApiResponse<CookiesViewModel> Login(string token);
    Task<ApiResponse<CookiesViewModel>> LoginAsync(LoginViewModel loginViewModel);
    Task<bool> SaveUser(RegistrationViewModel registrationViewModel);
    string GetEmailVerifiactionToken(string email);
    Task<ApiResponse<string>> EmailVerification(string verificationCode);
    ApiResponse<string> ForgotPassword(string email);
    ApiResponse<string> VerifyResetPasswordToken(string resetPasswordToken);
    Task<ApiResponse<string>> ResetPassword(ResetPasswordViewModel resetPasswordViewModel);
    bool IsEmailVerified(string email);
    Task<string> VerifyPassword(LoginViewModel loginViewModel);
    bool IsEmailExist(string Email);
    Task<bool> IsCorrectOldpassword(string email, string password);
    // User GetUserFromToken(string token);
    ApplicationUser GetUserFromTokenIdentity(string token);
}