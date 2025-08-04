using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Helper;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;

namespace BusinessAcessLayer.Services;

public class UserService : IUserService
{
    private readonly LedgerBookDbContext _context;
    private readonly IJWTTokenService _jwttokenService;
    private readonly IAttachmentService _attachmentService;
    private readonly IGenericRepo _genericRepository;
    private readonly IActivityLogService _activityLogService;
    private readonly UserManager<ApplicationUser> _userManager;


    public UserService(
        LedgerBookDbContext context,
         IJWTTokenService jWTTokenService,
         IAttachmentService attachmentService,
        IGenericRepo genericRepository,
        UserManager<ApplicationUser> userManager,
        IActivityLogService activityLogService
    )
    {
        _context = context;
        _jwttokenService = jWTTokenService;
        _attachmentService = attachmentService;
        _genericRepository = genericRepository;
        _userManager = userManager;
        _activityLogService = activityLogService;
    }

    public ApplicationUser GetuserByEmail(string Email)
    {
        return _genericRepository.Get<ApplicationUser>(u => u.Email.ToLower().Trim() == Email.ToLower().Trim() && u.DeletedAt == null)!;
    }

    // public User GetUserByEmailForUser(string email)
    // {
    //     return _genericRepository.Get<User>(u => u.Email.ToLower().Trim() == email.ToLower().Trim() && u.DeletedAt == null)!;
    // }


    public async Task<int> SaveUser(UserViewmodel userViewmodel)
    {
        ApplicationUser user = new();
        user.FirstName = userViewmodel.FirstName;
        user.LastName = userViewmodel.LastName;
        user.Email = userViewmodel.Email;
        user.UserName = userViewmodel.Email;
        user.PhoneNumber = userViewmodel.MobileNumber.ToString();
        user.IsUserRegistered = false;
        user.CreatedAt = DateTime.UtcNow;
        userViewmodel.Pasword = ConstantVariables.BasicPassword;
        IdentityResult result = await _userManager.CreateAsync(user, userViewmodel.Pasword);

        if (result.Succeeded)
        {
            return user.Id;
        }
        return 0;
    }

    public UserViewmodel GetuserById(int userId, int businessId)
    {
        return _genericRepository.GetAll<UserBusinessMappings>(
            predicate: ubm => ubm.BusinessId == businessId && ubm.DeletedAt == null && ubm.UserId == userId,
            includes: new List<Expression<Func<UserBusinessMappings, object>>>
            {
                x => x.User,
                x => x.Role
            }
        ).Select(x => new UserViewmodel
        {
            UserId = x.UserId,
            FirstName = x.User.FirstName,
            LastName = x.User.LastName,
            Email = x.User.Email,
            MobileNumber = x.User.PhoneNumber == null ? 0 : long.Parse(x.User.PhoneNumber),
            RoleId = x.RoleId,
            RoleName = x.Role.RoleName
        }).ToList().FirstOrDefault()!;
    }

    public bool IsUserRegistered(string email)
    {
        return _genericRepository.IsPresentAny<ApplicationUser>(x => x.Email.ToLower().Trim() == email.ToLower().Trim() && x.DeletedAt == null && x.IsUserRegistered == true);
    }

    public async Task<int> SavePersonalDetails(UserViewmodel userViewmodel, int userId)
    {
        PersonalDetails personalDetail = new();
        personalDetail.FirstName = userViewmodel.FirstName;
        personalDetail.LastName = userViewmodel.LastName;
        personalDetail.MobileNumber = userViewmodel.MobileNumber;
        personalDetail.CreatedAt = DateTime.UtcNow;
        personalDetail.CreatedById = userId;
        await _genericRepository.AddAsync<PersonalDetails>(personalDetail);

        return personalDetail.Id;
    }

    public async Task<int> UpdatePersonalDetails(UserViewmodel userViewmodel, int userId)
    {
        PersonalDetails personalDetail = _genericRepository.Get<PersonalDetails>(u => u.Id == userViewmodel.PersonalDetailId && u.DeletedAt == null)!;
        if (personalDetail != null)
        {
            personalDetail.FirstName = userViewmodel.FirstName;
            personalDetail.LastName = userViewmodel.LastName;
            personalDetail.MobileNumber = userViewmodel.MobileNumber;
            personalDetail.UpdatedAt = DateTime.UtcNow;
            personalDetail.UpdatedById = userId;
            await _genericRepository.UpdateAsync<PersonalDetails>(personalDetail);
            return personalDetail.Id;
        }
        else
        {
            return 0;
        }
    }



    public async Task<bool> UpdatePassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        ApplicationUser user = _genericRepository.Get<ApplicationUser>(x => x.Email.Trim().ToLower() == resetPasswordViewModel.Email.Trim().ToLower());

        if (user != null)
        {
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, resetPasswordViewModel.Password);
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedById = user.Id;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    public UserProfileViewModel GetUserProfile(int userId)
    {
        UserProfileViewModel userProfileViewModel = new();
        userProfileViewModel.AttachmentViewModel = new();
        ApplicationUser user = _genericRepository.Get<ApplicationUser>(x => x.Id == userId && !x.DeletedAt.HasValue);
        if (user != null)
        {
            userProfileViewModel.FirstName = user.FirstName;
            userProfileViewModel.LastName = user.LastName;
            userProfileViewModel.Email = user.Email;
            userProfileViewModel.MobileNumber = user.PhoneNumber == null ? 0 : long.Parse(user.PhoneNumber);
            userProfileViewModel.ProfileAttachmentId = user.ProfileAttachmentId;
            if (user.ProfileAttachmentId != null)
            {
                userProfileViewModel.AttachmentViewModel = _genericRepository.GetAll<Attachment>(x => x.Id == user.ProfileAttachmentId && !x.DeletedAt.HasValue)
                .Select(x => new AttachmentViewModel
                {
                    BusinesLogoPath = x.Path,
                    FileName = x.FileName,
                    FileExtension = x.FileExtensions
                }).FirstOrDefault();
            }
        }
        return userProfileViewModel;
    }

    public async Task<ApiResponse<CookiesViewModel>> UpdateUserProfile(UserProfileViewModel userProfileViewModel)
    {
        ApplicationUser user = _genericRepository.Get<ApplicationUser>(x => x.Email.ToLower().Trim() == userProfileViewModel.Email.ToLower().Trim() && !x.DeletedAt.HasValue);
        if (user != null)
        {
            user.FirstName = userProfileViewModel.FirstName;
            user.LastName = userProfileViewModel.LastName;
            user.PhoneNumber = userProfileViewModel.MobileNumber == null ? null : userProfileViewModel.MobileNumber.ToString();
            if (userProfileViewModel.BusinessLogo != null)
            {
                string[] extension = userProfileViewModel.BusinessLogo.FileName.Split(".");
                string fileNameTemp = userProfileViewModel.BusinessLogo.FileName;
                if (extension[extension.Length - 1] == "jpg" || extension[extension.Length - 1] == "jpeg" || extension[extension.Length - 1] == "png")
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    string imageFileName = CommonMethods.UploadImage(userProfileViewModel.BusinessLogo, path);
                    userProfileViewModel.AttachmentViewModel = new()
                    {
                        BusinesLogoPath = $"/uploads/{imageFileName}",
                        FileExtension = extension[extension.Length - 1],
                        FileName = fileNameTemp
                    };
                }
                else
                {
                    return new ApiResponse<CookiesViewModel>(false, Messages.InvalidImageExtensionMessage, null, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                userProfileViewModel.AttachmentViewModel = new();
            }
            if (userProfileViewModel.AttachmentViewModel != null)
            {
                if (userProfileViewModel.BusinessLogo != null)
                {
                    int attachmentId = await _attachmentService.SaveAttachment(userProfileViewModel.AttachmentViewModel, user.Id);
                    if (attachmentId == 0)
                    {
                        return new ApiResponse<CookiesViewModel>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "update", "user Profile"), null, HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        user.ProfileAttachmentId = attachmentId;
                    }
                }
            }
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedById = user.Id;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                string message = string.Format(Messages.BusinessActivity, "User", "updated", user.FirstName + " " + user.LastName);
                await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Update, EnumHelper.ActivityEntityType.User, user.Id, user.Id);
                CookiesViewModel cookiesViewModel = new()
                {
                    UserToken = _jwttokenService.GenerateToken(userProfileViewModel.Email),
                    UserName = user.FirstName + " " + user.LastName,
                    ProfilePhoto = userProfileViewModel.AttachmentViewModel.BusinesLogoPath
                };
                return new ApiResponse<CookiesViewModel>(true, string.Format(Messages.GlobalAddUpdateMesage, "User Profile", "Updated"), cookiesViewModel, HttpStatusCode.OK);
            }
        }
        return new ApiResponse<CookiesViewModel>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "update", "user Profile"), null, HttpStatusCode.BadRequest);

    }

    public string GetuserNameById(int userId)
    {
        ApplicationUser user = _genericRepository.Get<ApplicationUser>(x => x.Id == userId && !x.DeletedAt.HasValue);
        return user.FirstName + " " + user.LastName;
    }


    //temp method to conver all data of user into aspNetusers
    // public async Task<bool> ConvertdataToAspNetUsers()
    // {
    //     List<User> allUsers = _genericRepository.GetAll<User>(x => x.Id != 0).ToList();

    //     foreach (User user in allUsers)
    //     {
    //         ApplicationUser applicationUser = new()
    //         {
    //             FirstName = user.FirstName,
    //             LastName = user.LastName,
    //             Email = user.Email,
    //             UserName = user.Email,
    //             PhoneNumber = user.MobileNumber.ToString(),
    //             ProfileAttachmentId = user.ProfileAttachmentId,
    //             IsUserRegistered = user.IsUserRegistered,
    //             VerificationToken = user.VerificationToken,
    //             IsEmailVerified = user.IsEmailVerified,
    //             CreatedAt = user.CreatedAt,
    //             UpdatedById = user.UpdatedById,
    //             UpdatedAt = user.UpdatedAt,
    //             DeletedById = user.DeletedById,
    //             DeletedAt = user.DeletedAt,

    //         };
    //         string password = CommonMethods.Base64Decode(user.Password);
    //         IdentityResult result = await _userManager.CreateAsync(applicationUser, password);
    //         if (!result.Succeeded)
    //         {
    //             throw new Exception("Error while adding " + applicationUser.Email);
    //             return false;
    //         }
    //         else
    //         {
    //             user.ApplicationUserId = applicationUser.Id;
    //             user.Password = applicationUser.PasswordHash;
    //             await _genericRepository.UpdateAsync<User>(user);
    //         }
    //     }
    //     return true;
    // }
}