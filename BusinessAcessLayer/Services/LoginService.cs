using System.Net;
using System.Security.Claims;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Helper;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BusinessAcessLayer.Services;

public class LoginService : ILoginService
{
    private readonly IJWTTokenService _jwttokenService;
    private readonly IGenericRepo _genericRepository;
    private readonly IActivityLogService _activityLogService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserService _userService;
    private readonly IAttachmentService _attachmentService;

    public LoginService(
    IJWTTokenService jWTTokenService,
    IGenericRepo genericRepository,
    IActivityLogService activityLogService,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IUserService userService,
    IAttachmentService attachmentService
     )
    {
        _jwttokenService = jWTTokenService;
        _genericRepository = genericRepository;
        _activityLogService = activityLogService;
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
        _attachmentService = attachmentService;
    }

    public async Task<ApiResponse<string>> RegisterUser(RegistrationViewModel RegisterVM)
    {
        if (IsEmailExist(RegisterVM.Email) && _userService.IsUserRegistered(RegisterVM.Email))
        {
            return new ApiResponse<string>(false, Messages.EmailExistMessage, null, HttpStatusCode.BadRequest);
        }
        else
        {
            if (await SaveUser(RegisterVM))
            {
                string verificationToken = GetEmailVerifiactionToken(RegisterVM.Email);
                string verificationCode = _jwttokenService.GenerateTokenEmailVerificationToken(RegisterVM.Email, verificationToken);
                string verificationLink = "http://localhost:5189/Login/VerifyEmail?verificationCode=" + verificationCode;
                _ = CommonMethods.RegisterEmail(RegisterVM.FirstName + " " + RegisterVM.LastName, RegisterVM.Email, verificationLink, ConstantVariables.LoginLink);
                return new ApiResponse<string>(true, Messages.RegistrationSuccessMessage, verificationToken, HttpStatusCode.Created);
            }
            else
            {
                return new ApiResponse<string>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
            }
        }
    }

    public async Task<ApiResponse<CookiesViewModel>> LoginAsync(LoginViewModel loginViewModel)
    {
        CookiesViewModel cookiesViewModel = new();
        if (!IsEmailExist(loginViewModel.Email))
        {
            return new ApiResponse<CookiesViewModel>(false, Messages.EmailDoesNotExistMessage, null, HttpStatusCode.BadRequest);
        }
        else if (!_userService.IsUserRegistered(loginViewModel.Email))
        {
            return new ApiResponse<CookiesViewModel>(false, Messages.EmailDoesNotExistMessage, null, HttpStatusCode.BadRequest);
        }
        else
        {
            if (!IsEmailVerified(loginViewModel.Email))
            {
                return new ApiResponse<CookiesViewModel>(false, Messages.NotVerifiedEmailMessae, null, HttpStatusCode.BadRequest);
            }
            else
            {
                string verificaitonToken = await VerifyPassword(loginViewModel);
                if (verificaitonToken != null)
                {
                    cookiesViewModel.UserToken = verificaitonToken;
                    ApplicationUser user = GetUserFromTokenIdentity(verificaitonToken);
                    if (user == null)
                    {
                        return new ApiResponse<CookiesViewModel>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        if (user.ProfileAttachmentId != null)
                        {
                            AttachmentViewModel attachmentViewModel = _attachmentService.GetAttachmentById((int)user.ProfileAttachmentId);
                            cookiesViewModel.ProfilePhoto = attachmentViewModel.BusinesLogoPath;
                        }
                        cookiesViewModel.UserName = user.FirstName + " " + user.LastName;
                        return new ApiResponse<CookiesViewModel>(true, null, cookiesViewModel, HttpStatusCode.OK);
                    }
                }
                else
                {
                    return new ApiResponse<CookiesViewModel>(false, Messages.InvalidCredentilMessage, null, HttpStatusCode.BadRequest);
                }
            }
        }
    }

    public async Task<bool> SaveUser(RegistrationViewModel registrationViewModel)
    {
        ApplicationUser userToSaveOrUpdate = _genericRepository.Get<ApplicationUser>(a => a.Email.ToLower() == registrationViewModel.Email.ToLower() && !a.DeletedAt.HasValue);
        bool existingUser = false;
        if (userToSaveOrUpdate != null)
            existingUser = true;
        else
            userToSaveOrUpdate = new();

        //add in ApplicationUser
        userToSaveOrUpdate.Email = registrationViewModel.Email.ToLower().Trim();
        userToSaveOrUpdate.FirstName = registrationViewModel.FirstName.Trim();
        userToSaveOrUpdate.LastName = registrationViewModel.LastName.Trim();
        userToSaveOrUpdate.UserName = userToSaveOrUpdate.Email;
        userToSaveOrUpdate.IsEmailVerified = false;
        Guid guid = Guid.NewGuid();
        userToSaveOrUpdate.VerificationToken = guid.ToString();

        if (existingUser)
        {
            userToSaveOrUpdate.IsUserRegistered = true;
            userToSaveOrUpdate.UpdatedAt = DateTime.UtcNow;
            userToSaveOrUpdate.PasswordHash = _userManager.PasswordHasher.HashPassword(userToSaveOrUpdate, registrationViewModel.Password);
            IdentityResult result = await _userManager.UpdateAsync(userToSaveOrUpdate);
            if (result.Succeeded)
            {
                string message = string.Format(Messages.UserActivity, "User", "created");
                await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Add, EnumHelper.ActivityEntityType.User, userToSaveOrUpdate.Id);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            userToSaveOrUpdate.IsUserRegistered = true;
            userToSaveOrUpdate.CreatedAt = DateTime.UtcNow;
            IdentityResult result = await _userManager.CreateAsync(userToSaveOrUpdate, registrationViewModel.Password);
            if (result.Succeeded)
            {
                string message = string.Format(Messages.UserActivity, "User", "created");
                await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Add, EnumHelper.ActivityEntityType.User, userToSaveOrUpdate.Id);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public string GetEmailVerifiactionToken(string email)
    {
        return _genericRepository.Get<ApplicationUser>(x => x.Email.ToLower().Trim() == email.ToLower().Trim() && !x.DeletedAt.HasValue).VerificationToken;
    }

    public async Task<ApiResponse<string>> EmailVerification(string verificationCode)
    {
        string email = _jwttokenService.GetClaimValue(verificationCode, "email")!;
        string emailToken = _jwttokenService.GetClaimValue(verificationCode, "token")!;

        ApplicationUser user = _genericRepository.Get<ApplicationUser>(x => x.Email.ToLower().Trim() == email.ToLower().Trim() && x.VerificationToken == emailToken && !x.DeletedAt.HasValue)!;
        if (user != null)
        {
            user.IsEmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedById = user.Id;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return new ApiResponse<string>(true, Messages.VerificationSuccessMessage, null, HttpStatusCode.OK);
        }
        return new ApiResponse<string>(false, Messages.VerificationErrorMessage, null, HttpStatusCode.BadRequest);
    }

    public ApiResponse<string> ForgotPassword(string email)
    {
        if (IsEmailExist(email))
        {
            if (_userService.IsUserRegistered(email))
            {
                //send email
                ApplicationUser user = _userService.GetuserByEmail(email);
                string username = user.FirstName + " " + user.LastName;
                string resetPasswordToken = _jwttokenService.GenerateTokenEmailPassword(email, user.PasswordHash);
                string resetLink = ConstantVariables.LoginLink + "/Login/ResetPassword?resetPasswordToken=" + resetPasswordToken;
                _ = CommonMethods.ResetPasswordEmail(email, username, resetLink, ConstantVariables.LoginLink);
                return new ApiResponse<string>(true, Messages.SendResetPasswordMailSuccess, null, HttpStatusCode.OK);
            }
            else
            {
                return new ApiResponse<string>(false, Messages.EmailDoesNotExistMessage, null, HttpStatusCode.BadRequest);
            }
        }
        else
        {
            return new ApiResponse<string>(false, Messages.EmailDoesNotExistMessage, null, HttpStatusCode.BadRequest);
        }
    }

    public ApiResponse<string> VerifyResetPasswordToken(string resetPasswordToken)
    {
        try
        {
            string email = _jwttokenService.GetClaimValue(resetPasswordToken, "email")!;
            string newpassword = _jwttokenService.GetClaimValue(resetPasswordToken, "password")!;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newpassword) || !IsEmailExist(email))
                return new ApiResponse<string>(false, Messages.InvalidResetPasswordLink, null, HttpStatusCode.BadRequest);

            ApplicationUser user = _userService.GetuserByEmail(email);
            string savedPassword = user.PasswordHash!;
            if (savedPassword == newpassword)
                return new ApiResponse<string>(true, null, email, HttpStatusCode.BadRequest);
            return new ApiResponse<string>(false, Messages.LinkAlreadyUsedMessage, null, HttpStatusCode.BadRequest);
        }
        catch (Exception e)
        {
            return new ApiResponse<string>(false, Messages.InvalidResetPasswordLink, null, HttpStatusCode.BadRequest);

        }
    }

    public async Task<ApiResponse<string>> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        ApplicationUser user = _userService.GetuserByEmail(resetPasswordViewModel.Email);
        if (user != null)
        {
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, resetPasswordViewModel.Password);
            if (result != PasswordVerificationResult.Failed)
            {
                return new ApiResponse<string>(false, Messages.SamePasswordsErrorMessage, null, HttpStatusCode.BadRequest);
            }
            else
            {
                bool IsPasswordUpdated = await _userService.UpdatePassword(resetPasswordViewModel);
                if (IsPasswordUpdated)
                    return new ApiResponse<string>(true, string.Format(Messages.GlobalAddUpdateMesage, "Password", "updated"), null, HttpStatusCode.OK);
                else
                    return new ApiResponse<string>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "update", "Password"), null, HttpStatusCode.BadRequest);
            }
        }
        else
        {
            return new ApiResponse<string>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
    }

    public bool IsEmailVerified(string email)
    {
        return _genericRepository.IsPresentAny<ApplicationUser>(x => x.Email.ToLower().Trim() == email.ToLower().Trim() && x.IsEmailVerified && !x.DeletedAt.HasValue)!;
    }

    public async Task<string> VerifyPassword(LoginViewModel loginViewModel)
    {
        try
        {
            ApplicationUser user = _genericRepository.Get<ApplicationUser>(e => e.Email == loginViewModel.Email.ToLower().Trim() && e.DeletedAt == null)!;

            if (user != null)
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(user.Email, loginViewModel.Password, isPersistent:true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    string token = _jwttokenService.GenerateToken(loginViewModel.Email);
                    return token;
                }
                return null;
            }
            return null;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public bool IsEmailExist(string Email)
    {
        return _genericRepository.IsPresentAny<ApplicationUser>(u => u.Email.ToLower().Trim() == Email.ToLower().Trim() && u.DeletedAt == null)!;
    }

    public async Task<bool> IsCorrectOldpassword(string email, string password)
    {
        SignInResult result = await _signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);
        if (result.Succeeded)
            return true;
        return false;
    }

    public ApplicationUser GetUserFromTokenIdentity(string token)
    {
        try
        {
            ClaimsPrincipal claims = _jwttokenService.GetClaimsFromToken(token);
            string Email = _jwttokenService.GetClaimValue(token, "email");
            return _genericRepository.Get<ApplicationUser>(x => x.Email.ToLower().Trim() == Email.ToLower().Trim())!;
        }
        catch (Exception e)
        {
            throw new UnauthorizedAccessException();
        }
    }
}