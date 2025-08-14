using System.Linq.Expressions;
using System.Threading.Tasks;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BusinessAcessLayer.Services;

public class UserBusinessMappingService : IUserBusinessMappingService
{
    private readonly LedgerBookDbContext _context;
    private readonly IGenericRepo _genericRepository;
    private readonly ITransactionRepository _transactionRepository;


    public UserBusinessMappingService(
        LedgerBookDbContext context,
        IGenericRepo genericRepository,
        ITransactionRepository transactionRepository
    )
    {
        _context = context;
        _genericRepository = genericRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<int> SaveUserBusinessMapping(int userId, int businessId, int roleId, int personalDetailId, int createdById)
    {
        UserBusinessMappings userBusinessMapping = new();
        userBusinessMapping.UserId = userId;
        userBusinessMapping.BusinessId = businessId;
        userBusinessMapping.RoleId = roleId;
        userBusinessMapping.PersonDetailId = personalDetailId;
        userBusinessMapping.CreatedAt = DateTime.UtcNow;
        userBusinessMapping.CreatedById = createdById;
        userBusinessMapping.IsActive = true;
        await _genericRepository.AddAsync<UserBusinessMappings>(userBusinessMapping);
        return userBusinessMapping.Id;
    }

    public async Task<List<UserViewmodel>> GetUsersByBusiness(int businessId, int userId)
    {
        var userMappingDetails = _genericRepository.GetAll<UserBusinessMappings>(
            predicate: ubm => ubm.BusinessId == businessId && ubm.DeletedAt == null && ubm.UserId != userId && ubm.UserId != ubm.Business.CreatedById,
            includes: new List<Expression<Func<UserBusinessMappings, object>>>
            {
                x => x.User,
                x => x.PersonalDetails,
                x=> x.Role,
                x => x.Business
            }
        ).GroupBy(userMappingKey => new { userMappingKey.BusinessId, userMappingKey.UserId }).ToList();

        return userMappingDetails.Select(selectedData =>
        {
            ApplicationUser user = selectedData.First().User;
            PersonalDetails? personalDetails = selectedData.FirstOrDefault()?.PersonalDetails;
            List<RoleViewModel> userRoleList = selectedData.Where(a => a.Role != null).Select(userMapping => new RoleViewModel
            {
                RoleId = userMapping.Role.Id,
                RoleName = userMapping.Role.RoleName,
                RoleDescription = userMapping.Role.Description
            }).ToList();

            return new UserViewmodel()
            {
                UserId = user.Id,
                PersonalDetailId = personalDetails?.Id,
                FirstName = personalDetails?.FirstName,
                LastName = personalDetails?.LastName,
                Email = user.Email,
                CreatedById = userRoleList.FirstOrDefault(x => x.RoleName == ConstantVariables.OwnerRole) != null ? selectedData.First(x => x.UserId == user.Id && x.Role.RoleName == ConstantVariables.OwnerRole).CreatedById : selectedData.First(x => x.UserId == user.Id).CreatedById,
                CanEdit = false,
                CanAddOwner = false,
                IsActive = selectedData.Any(x => !x.IsActive) ? false : true,
                MobileNumber = personalDetails?.MobileNumber == null ? 0 : (long)personalDetails.MobileNumber,
                Roles = userRoleList,
                RoleName = string.Join(",", userRoleList.Select(a => a.RoleName).ToList())
            };
        }).ToList();
    }

    public List<UserViewmodel> SetPermissions(List<UserViewmodel> usersList, int loginUserId, int businessId)
    {
        //main owner permissions
        List<UserBusinessMappings> mainOwner = _genericRepository.GetAll<UserBusinessMappings>(x => x.BusinessId == businessId && x.UserId == x.CreatedById).ToList();
        int ownerId = mainOwner.FirstOrDefault().UserId;
        if (loginUserId == ownerId)
        {
            foreach (UserViewmodel user in usersList)
            {
                user.CanEdit = true;
                user.CanAddOwner = true;
            }
            return usersList;
        }

        //other owner permissions
        foreach (UserViewmodel user in usersList)
        {
            if (user.CreatedById == loginUserId)
            {
                user.CanEdit = true;
            }
        }
        return usersList;
    }

    public bool IsMainOwner(int businessId, int loginUserId)
    {
        UserBusinessMappings mainOwner = _genericRepository.Get<UserBusinessMappings>(x => x.BusinessId == businessId && x.UserId == x.CreatedById);
        if (mainOwner != null)
        {
            if (mainOwner.UserId == loginUserId)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> UpdateUserBusinessMapping(int userId, int businessId, List<int> roleId, int personalDetailId, int updatedById)
    {
        List<int> newList = new();
        List<UserBusinessMappings> mappingEntries = _genericRepository.GetAll<UserBusinessMappings>(x => x.UserId == userId && x.BusinessId == businessId && x.DeletedAt == null).ToList();
        for (int i = 0; i < mappingEntries.Count; i++)
        {
            newList.Add(mappingEntries[i].RoleId);
            if (!roleId.Contains(mappingEntries[i].RoleId))
            {
                await DeleteUserBusinessMapping(userId, businessId, mappingEntries[i].RoleId, updatedById);
            }
            else if (mappingEntries[i].PersonDetailId == null)
            {
                mappingEntries[i].PersonDetailId = personalDetailId;
                await _genericRepository.UpdateAsync<UserBusinessMappings>(mappingEntries[i]);
            }
        }
        for (int i = 0; i < roleId.Count; i++)
        {
            if (!newList.Contains(roleId[i]))
            {
                await SaveUserBusinessMapping(userId, businessId, roleId[i], personalDetailId, updatedById);
            }
        }
        return true;

    }

    public async Task<bool> DeleteUserBusinessMapping(int userId, int businessId, int roleId, int deletedById)
    {
        List<UserBusinessMappings> userBusinessMapping = _genericRepository.GetAll<UserBusinessMappings>(x => x.BusinessId == businessId && x.UserId == userId && x.RoleId == roleId && x.DeletedAt == null).ToList();
        foreach (UserBusinessMappings mapping in userBusinessMapping)
        {
            mapping.DeletedAt = DateTime.UtcNow;
            mapping.DeletedById = deletedById;
            await _genericRepository.UpdateAsync<UserBusinessMappings>(mapping);
        }
        return true;
    }
    public async Task<bool> DeleteUserBusinessMappingByBusinessId(int userId, int businessId, int deletedById)
    {
        try
        {
            await _transactionRepository.BeginTransactionAsync();
            List<UserBusinessMappings> userBusinessMapping = _genericRepository.GetAll<UserBusinessMappings>(x => x.BusinessId == businessId && x.UserId == userId && x.DeletedAt == null).ToList();
            foreach (UserBusinessMappings mapping in userBusinessMapping)
            {
                PersonalDetails personalDetails = _genericRepository.Get<PersonalDetails>(pd => pd.Id == mapping.PersonDetailId && !pd.DeletedAt.HasValue);
                if (personalDetails != null)
                {
                    personalDetails.DeletedAt = DateTime.UtcNow;
                    personalDetails.DeletedById = userId;
                    _genericRepository.Update<PersonalDetails>(personalDetails);
                }
                mapping.DeletedAt = DateTime.UtcNow;
                mapping.DeletedById = deletedById;
                await _genericRepository.UpdateAsync<UserBusinessMappings>(mapping);
            }
            await _transactionRepository.CommitAsync();
            return true;
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackAsync();
            throw;
        }
    }

    public List<RoleViewModel> GetRolesByBusinessId(int businessId, int userId)
    {
        return _genericRepository.GetAll<UserBusinessMappings>(
            predicate: ubm => ubm.BusinessId == businessId && ubm.UserId == userId && ubm.DeletedAt == null && ubm.IsActive,
            includes: new List<Expression<Func<UserBusinessMappings, object>>>
            {
                x => x.Role
            }
        ).Select(x => new RoleViewModel
        {
            RoleId = x.Role.Id,
            RoleName = x.Role.RoleName,
            RoleDescription = x.Role.Description
        }).ToList();
    }

    public PersonalDetails GetPersonalDetalsByMapping(int businessId, int userId)
    {
        UserBusinessMappings userBusinessMappings = _genericRepository.Get<UserBusinessMappings>(x => x.BusinessId == businessId && x.UserId == userId && !x.DeletedAt.HasValue);
        if (userBusinessMappings == null)
        {
            return null;
        }
        else
        {
            return _genericRepository.Get<PersonalDetails>(x => x.Id == userBusinessMappings.PersonDetailId && !x.DeletedAt.HasValue);
        }
    }

    public async Task<bool> ActiveInactiveUser(int userId, int businessId, bool isActive, int updatedById)
    {
        try
        {
            await _transactionRepository.BeginTransactionAsync();
            List<UserBusinessMappings> userBusinessMappings = _genericRepository.GetAll<UserBusinessMappings>(x => x.BusinessId == businessId && x.UserId == userId && !x.DeletedAt.HasValue).ToList();
            if (userBusinessMappings != null)
            {
                foreach (UserBusinessMappings userBusinessMapping in userBusinessMappings)
                {
                    userBusinessMapping.IsActive = isActive;
                    userBusinessMapping.UpdatedAt = DateTime.UtcNow;
                    userBusinessMapping.UpdatedById = updatedById;
                    _genericRepository.Update<UserBusinessMappings>(userBusinessMapping);
                }
                await _genericRepository.SaveChangesAsync();
                await _transactionRepository.CommitAsync();
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackAsync();
            throw;
        }
    }

    public bool HasPermission(int businessId, int userId, string partyType)
    {
        List<RoleViewModel> rolesByUser = GetRolesByBusinessId(businessId, userId);
        if (partyType == PartyType.Customer)
        {
            if (rolesByUser.Any(role => role.RoleName == ConstantVariables.SalesManagerRole || role.RoleName == ConstantVariables.OwnerRole))
            {
                return true;
            }
        }else if (partyType == PartyType.Supplier)
        {
            if (rolesByUser.Any(role => role.RoleName == ConstantVariables.PurchaseManagerRole || role.RoleName == ConstantVariables.OwnerRole))
            {
                return true;
            }
        }
        return false;
    }
}