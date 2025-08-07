using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IBusinessService
{
    List<BusinessViewModel> GetBusinesses(int userId, string searchText = null);
    ApiResponse<List<BusinessViewModel>> GetRolewiseBusiness(int userId, string searchText = null);
    Task<ApiResponse<BusinessItem>> SaveBusiness(BusinessItem businessItem, int userId);
    ApiResponse<BusinessItem> GetBusinessItemById(int? businessId);
    Businesses GetBusinessFromToken(string token);
    Task<ApiResponse<string>> DeleteBusiness(int businessId, int userId);
    string GetBusinessNameById(int businessId);
    List<BusinessViewModel> GetAllBusinesses(int userId);
    Task<ApiResponse<List<UserViewmodel>>> GetUsersOfBusiness(int businessId, int userId);
    Task<ApiResponse<UserViewmodel>> GetUserById(int businessId, int curentUserId, int? userId);
    Task<ApiResponse<List<UserViewmodel>>> SaveUserDetails(UserViewmodel userViewmodel, List<int> selectedRole, int businessId, int userId);
    Task<ApiResponse<string>> DeleteUserFromBusiness(int userId, int businessId, ApplicationUser logedinUser);
    Task<ApiResponse<string>> ActiveInactiveUser(int userId, bool isActive, int businessId, ApplicationUser logedinUser);
}