using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface ILoginService
{
    Task<bool> SaveUser(RegistrationViewModel registrationViewModel);
    string GetEmailVerifiactionToken(string email);
    Task<bool> EmailVerification(string email, string emailToken);
    bool IsEmailVerified(string email);
    Task<string> VerifyPassword(LoginViewModel loginViewModel);
    bool IsEmailExist(string Email);
    Task<bool> IsCorrectOldpassword(string email, string password);
    // User GetUserFromToken(string token);
    ApplicationUser GetUserFromTokenIdentity(string token);
}