using BusinessAcessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IUserBusinessMappingService
{
    Task<int> SaveUserBusinessMapping(int userId, int businessId, int roleId, int personalDetailId, int createdById);
    Task<List<UserViewmodel>> GetUsersByBusiness(int businessId, int userId);
    List<UserViewmodel> SetPermissions(List<UserViewmodel> usersList, int loginUserId, int businessId);
    bool IsMainOwner(int businessId, int loginUserId);
    Task<bool> UpdateUserBusinessMapping(int userId, int businessId, List<int> roleId, int personalDetailId, int updatedById);
    Task<bool> DeleteUserBusinessMapping(int userId, int businessId, int roleId, int deletedById);
    Task<bool> DeleteUserBusinessMappingByBusinessId(int userId, int businessId, int deletedById);
    List<RoleViewModel> GetRolesByBusinessId(int businessId, int userId);
    public PersonalDetails GetPersonalDetalsByMapping(int businessId, int userId);
    Task<bool> ActiveInactiveUser(int userId, int businessId, bool isActive, int updatedById);
    bool HasPermission(int businessId, int userId, string partyType);
}