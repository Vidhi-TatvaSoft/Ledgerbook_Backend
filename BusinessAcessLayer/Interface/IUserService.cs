using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IUserService
{
    ApplicationUser GetuserByEmail(string Email);
    Task<int> SaveUser(UserViewmodel userViewmodel);
    // User GetUserByEmailForUser(string email);
    UserViewmodel GetuserById(int userId, int businessId);
    bool IsUserRegistered(string email);
    Task<int> SavePersonalDetails(UserViewmodel userViewmodel, int userId);
    Task<int> UpdatePersonalDetails(UserViewmodel userViewmodel, int userId);
    Task<bool> UpdatePassword(ResetPasswordViewModel resetPasswordViewModel);
    UserProfileViewModel GetUserProfile(int userId);
    Task<bool> UpdateUserProfile(UserProfileViewModel userProfileViewModel);
    string GetuserNameById(int userId);
    // Task<bool> ConvertdataToAspNetUsers();
}