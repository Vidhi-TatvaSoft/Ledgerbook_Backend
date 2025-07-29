using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IBusinessService
{
    List<BusinessViewModel> GetBusinesses(int userId, string searchText = null);
    List<BusinessViewModel> GetRolewiseBusiness(List<BusinessViewModel> businessList);
    Task<int> SaveBusiness(BusinessItem businessItem, int userId);
    BusinessItem GetBusinessItemById(int? businessId);
    Businesses GetBusinessFromToken(string token);
    Task<bool> DeleteBusiness(int businessId, int userId);
    string GetBusinessNameById(int businessId);
    List<BusinessViewModel> GetAllBusinesses(int userId);

}