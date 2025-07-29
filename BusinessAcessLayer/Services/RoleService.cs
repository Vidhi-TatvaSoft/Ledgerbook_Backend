using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Services;

public class RoleService : IRoleService
{
    private readonly LedgerBookDbContext _context;
    private readonly IGenericRepo _genericRepo;
    public RoleService(LedgerBookDbContext context, 
    IGenericRepo genericRepo)
    {
        _context = context;
        _genericRepo = genericRepo;
    }
    public List<RoleViewModel> GetAllRoles()
    {
        return _genericRepo.GetAll<Role>(r => r.DeletedAt == null).Select(x => new RoleViewModel
        {
            RoleId = x.Id,
            RoleName = x.RoleName,
            RoleDescription = x.Description
        }).ToList();
    }

    public Role GetRoleById(int id)
    {
        return _genericRepo.Get<Role>(r => r.Id == id)!;
    }

    public List<RoleViewModel> GetRolesExceptOwner()
    {
        return _genericRepo.GetAll<Role>(r => r.DeletedAt == null && r.RoleName != ConstantVariables.OwnerRole).Select(x => new RoleViewModel
        {
            RoleId = x.Id,
            RoleName = x.RoleName,
            RoleDescription = x.Description
        }).ToList();
    } 
}