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
    Task<bool> DeleteBusiness(int businessId, int userId);
    string GetBusinessNameById(int businessId);
    List<BusinessViewModel> GetAllBusinesses(int userId);

}