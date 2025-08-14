using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IRoleService
{
    List<RoleViewModel> GetAllRoles();
    Role GetRoleById(int Id);
    List<RoleViewModel> GetRolesExceptOwner();
    int GetOwnerRoleId();
}