
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;
using DataAccessLayer.Constant;
using BusinessAcessLayer.Helper;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using System.Transactions;
using BusinessAcessLayer.Constant;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using BusinessAcessLayer.CustomException;

namespace BusinessAcessLayer.Services;

public class BusinessService : IBusinessService
{
    private readonly LedgerBookDbContext _context;
    private readonly IJWTTokenService _jwttokenService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IAttachmentService _attachmentService;
    private readonly IAddressService _addressService;
    private readonly IUserBusinessMappingService _userBusinessMappingService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IGenericRepo _genericRepository;
    private readonly IActivityLogService _activityLogService;
    private readonly IReferenceDataEntityService _referenceDataEntityService;


    public BusinessService(LedgerBookDbContext context,
    IJWTTokenService jWTTokenService,
     IUserService userService,
     IRoleService roleService,
     IAttachmentService attachmentService,
     IAddressService addressService,
     IUserBusinessMappingService userBusinessMappingService,
     ITransactionRepository transactionRepository,
     IGenericRepo genericRepository,
     IActivityLogService activityLogService,
     IReferenceDataEntityService referenceDataEntityService
     )
    {
        _context = context;
        _jwttokenService = jWTTokenService;
        _userService = userService;
        _roleService = roleService;
        _attachmentService = attachmentService;
        _addressService = addressService;
        _userBusinessMappingService = userBusinessMappingService;
        _transactionRepository = transactionRepository;
        _genericRepository = genericRepository;
        _activityLogService = activityLogService;
        _referenceDataEntityService = referenceDataEntityService;
    }


    public List<BusinessViewModel> GetBusinesses(int userId, string searchText = null)
    {
        IEnumerable<UserBusinessMappings> query = _genericRepository.GetAll<UserBusinessMappings>(predicate: ubm => ubm.UserId == userId && ubm.DeletedAt == null && ubm.IsActive,
                thenIncludes: new List<Func<IQueryable<UserBusinessMappings>, IQueryable<UserBusinessMappings>>>
                {
                   x => x.Include(ubm => ubm.Business)
                            .ThenInclude(b => b.LogoAttachment)
                        .Include(ubm => ubm.Business)
                            .ThenInclude(b => b.BusinessCategory)
                        .Include(ubm => ubm.Business)
                            .ThenInclude(b => b.BusinessType)
                        .Include(ubm => ubm.User)
                });

        // search filter
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            searchText = searchText.Trim().ToLower();
            query = query.Where(ubm =>
                ubm.Business.BusinessName.ToLower().Contains(searchText) ||
                ubm.Business.BusinessCategory.EntityValue.ToLower().Contains(searchText) ||
                ubm.Business.BusinessType.EntityValue.ToLower().Contains(searchText) ||
                _genericRepository.GetAll<UserBusinessMappings>(x => !x.DeletedAt.HasValue,
                 includes: new List<Expression<Func<UserBusinessMappings, object>>>
                {
                    x => x.User,
                    x => x.Role
                }).Any(x => x.BusinessId == ubm.BusinessId &&
                             x.Role.RoleName == Constant.ConstantVariables.OwnerRole &&
                             x.DeletedAt == null &&
                             (x.User.FirstName + " " + x.User.LastName).ToLower().Contains(searchText))
            );
        }

        var businesses = query
            .GroupBy(ubm => ubm.BusinessId)
            .ToList();

        return businesses.Select(data =>
        {
            UserBusinessMappings business = data.First();
            List<string> ownerNames = _genericRepository.GetAll<UserBusinessMappings>(predicate: x => x.BusinessId == data.Key && x.Role.RoleName == Constant.ConstantVariables.OwnerRole && x.DeletedAt == null && x.IsActive,
                    includes: new List<Expression<Func<UserBusinessMappings, object>>>
                    {
                        x => x.User
                    }
                    ).Select(x => x.User.FirstName + " " + x.User.LastName)
                        .ToList();

            List<int> ownerIds = _genericRepository.GetAll<UserBusinessMappings>(predicate: x => x.BusinessId == data.Key && x.Role.RoleName == Constant.ConstantVariables.OwnerRole && x.DeletedAt == null && x.IsActive,
                    includes: new List<Expression<Func<UserBusinessMappings, object>>>
                    {
                        x => x.User
                    }
                    ).Select(x => x.User.Id)
                    .ToList();

            return new BusinessViewModel
            {
                BusinessId = business.Business.Id,
                BusienssName = business.Business.BusinessName,
                LogoPath = business.Business.LogoAttachment?.Path,
                OwnerId = string.Join(", ", ownerIds),
                OwnerName = string.Join(", ", ownerNames),
                CurentUserId = userId,
                CanEditDelete = false,
                CanDelete = false
            };
        })
        .ToList();
    }

    public ApiResponse<List<BusinessViewModel>> GetRolewiseBusiness(int userId, string searchText = null)
    {
        if (userId == 0)
        {
            return new ApiResponse<List<BusinessViewModel>>(false, Messages.ExceptionMessage, null, HttpStatusCode.Forbidden);
        }
        List<BusinessViewModel> businessList = GetBusinesses(userId, searchText);
        if (businessList != null)
        {
            foreach (BusinessViewModel business in businessList)
            {
                List<UserBusinessMappings> mainOwner = _genericRepository.GetAll<UserBusinessMappings>(x => x.BusinessId == business.BusinessId && x.UserId == x.CreatedById && !x.DeletedAt.HasValue).ToList();
                if (mainOwner.Count != 0)
                {
                    int mainOwnerId = mainOwner.FirstOrDefault().UserId;
                    string[] userIdstring = business.OwnerId.Split(",");
                    List<int> userIds = new();
                    foreach (string id in userIdstring)
                    {
                        userIds.Add(Int32.Parse(id));
                    }
                    if (userIds.Contains(business.CurentUserId))
                    {
                        business.CanEditDelete = true;
                    }
                    if (business.CurentUserId == mainOwnerId)
                    {
                        business.CanDelete = true;
                    }
                }
            }
        }
        return new ApiResponse<List<BusinessViewModel>>(true, null, businessList, HttpStatusCode.OK);
    }

    public async Task<ApiResponse<BusinessItem>> SaveBusiness(BusinessItem businessItem, int userId)
    {
        try
        {
            if (userId == 0)
            {
                return new ApiResponse<BusinessItem>(false, Messages.ExceptionMessage, businessItem, HttpStatusCode.BadRequest);
            }
            if (businessItem.BusinessLogoIForm != null)
            {
                businessItem.BusinessLogoAttachment = new();
                string[] extension = businessItem.BusinessLogoIForm.FileName.Split(".");
                string fileNameTemp = businessItem.BusinessLogoIForm.FileName;
                if (extension[extension.Length - 1] == "jpg" || extension[extension.Length - 1] == "jpeg" || extension[extension.Length - 1] == "png")
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    string imageFileName = CommonMethods.UploadImage(businessItem.BusinessLogoIForm, path);

                    businessItem.BusinessLogoAttachment.BusinesLogoPath = $"/uploads/{imageFileName}";
                    businessItem.BusinessLogoAttachment.FileExtension = extension[extension.Length - 1];
                    businessItem.BusinessLogoAttachment.FileName = fileNameTemp;
                    businessItem.BusinessLogoIForm = null;
                }
                else
                {
                    return new ApiResponse<BusinessItem>(false, Messages.InvalidImageExtensionMessage, businessItem, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                Businesses updateBusiness = _genericRepository.Get<Businesses>(b => b.Id == businessItem.BusinessId && b.DeletedAt == null)!;
                if (updateBusiness != null && (updateBusiness.LogoAttachmentId != null))
                {
                    AttachmentViewModel attachmentViewModel = _attachmentService.GetAttachmentById((int)updateBusiness.LogoAttachmentId);
                    businessItem.BusinessLogoAttachment = attachmentViewModel;
                }
                else
                {
                    businessItem.BusinessLogoAttachment = new();
                }
            }


            await _transactionRepository.BeginTransactionAsync();
            int addressId = 0;
            int attachmentId = 0;
            int businessId;

            if (businessItem.BusinessLogoAttachment.BusinesLogoPath != null)
            {
                attachmentId = await _attachmentService.SaveAttachment(businessItem.BusinessLogoAttachment, userId);
            }

            if (businessItem.AddressLine1 != null || businessItem.AddressLine2 != null || businessItem.City != null || businessItem.Pincode != null)
            {
                AddressViewModel addressViewModel = new()
                {
                    AddressLine1 = businessItem.AddressLine1,
                    AddressLine2 = businessItem.AddressLine2,
                    City = businessItem.City,
                    Pincode = businessItem.Pincode
                };
                addressId = await _addressService.SaveAddress(addressViewModel, userId);
            }
            if (businessItem.BusinessId == 0)
            {
                Businesses business = new();
                business.BusinessName = businessItem.BusinessName;
                business.MobileNumber = businessItem.MobileNumber;
                if (addressId != 0)
                {
                    business.AddressId = addressId;
                }
                else
                {
                    business.AddressId = null;
                }
                if (attachmentId != 0)
                {
                    business.LogoAttachmentId = attachmentId;
                }
                else
                {
                    business.LogoAttachmentId = null;
                }
                business.BusinessCategoryId = businessItem.BusinescategoryId;
                business.BusinessTypeId = businessItem.BusinessTypeId;
                business.CreatedAt = DateTime.UtcNow;
                business.CreatedById = userId;
                business.IsActive = businessItem.IsActive;
                business.GSTIN = businessItem.GSTIN;
                businessItem.IsNewbusiness = true;
                await _genericRepository.AddAsync<Businesses>(business);
                businessId = business.Id;
                int roleId = _genericRepository.Get<Role>(r => r.RoleName == ConstantVariables.OwnerRole && !r.DeletedAt.HasValue)!.Id;

                UserViewmodel user = _genericRepository.GetAll<ApplicationUser>(x => x.Id == userId && x.DeletedAt == null).Select(x => new UserViewmodel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MobileNumber = x.PhoneNumber == null ? 0 : long.Parse(x.PhoneNumber),
                }).FirstOrDefault();
                if (user != null)
                {
                    int personalDetailId = await _userService.SavePersonalDetails(user, userId);
                    int userBusinessmappingOwnerId = await _userBusinessMappingService.SaveUserBusinessMapping(userId, businessId, roleId, personalDetailId, userId);
                }

            }
            else
            {
                if (!CanEditBusiness((int)businessItem.BusinessId, userId))
                {
                    await _transactionRepository.RollbackAsync();
                    return new ApiResponse<BusinessItem>(false, Messages.ForbiddenMessage, businessItem, HttpStatusCode.Forbidden);
                }
                Businesses updateBusiness = _genericRepository.Get<Businesses>(b => b.Id == businessItem.BusinessId && b.DeletedAt == null)!;
                if (updateBusiness != null)
                {
                    updateBusiness.BusinessName = businessItem.BusinessName;
                    updateBusiness.MobileNumber = businessItem.MobileNumber;
                    updateBusiness.BusinessCategoryId = businessItem.BusinescategoryId;
                    updateBusiness.BusinessTypeId = businessItem.BusinessTypeId;
                    updateBusiness.GSTIN = businessItem.GSTIN;
                    if (addressId != 0)
                    {
                        updateBusiness.AddressId = addressId;
                    }
                    else
                    {
                        updateBusiness.AddressId = null;
                    }
                    if (attachmentId != 0)
                    {
                        updateBusiness.LogoAttachmentId = attachmentId;
                    }
                    else
                    {
                        updateBusiness.LogoAttachmentId = null;
                    }
                    updateBusiness.UpdatedAt = DateTime.UtcNow;
                    updateBusiness.UpdatedById = userId;
                    updateBusiness.IsActive = businessItem.IsActive;
                    businessItem.IsNewbusiness = false;
                    await _genericRepository.UpdateAsync<Businesses>(updateBusiness);
                    businessId = updateBusiness.Id;
                }
                else
                {
                    await _transactionRepository.RollbackAsync();
                    return new ApiResponse<BusinessItem>(false, Messages.ExceptionMessage, businessItem, HttpStatusCode.BadRequest);
                }
            }
            await _transactionRepository.CommitAsync();
            string userName = _userService.GetuserNameById(userId);
            if (businessItem.BusinessId == 0)
            {
                string message = string.Format(Messages.BusinessActivity, "Business", "created", userName);
                await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Add, EnumHelper.ActivityEntityType.Business, businessId, userId);
                businessItem.BusinessId = businessId;
                return new ApiResponse<BusinessItem>(true, string.Format(Messages.GlobalAddUpdateMesage, "Business", "added"), businessItem, HttpStatusCode.OK);
            }
            else
            {
                string message = string.Format(Messages.BusinessActivity, "Business", "updated", userName);
                await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Update, EnumHelper.ActivityEntityType.Business, businessId, userId);
                return new ApiResponse<BusinessItem>(true, string.Format(Messages.GlobalAddUpdateMesage, "Business", "updated"), businessItem, HttpStatusCode.OK);
            }
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackAsync();
            throw;
        }
    }

    public ApiResponse<BusinessItem> GetBusinessItemById(int? businessId)
    {
        BusinessItem businessViewModel = new();
        businessViewModel.BusinessCategories = _referenceDataEntityService.GetReferenceValues(EnumHelper.EntityType.BusinessCategory.ToString());
        businessViewModel.BusinessTypes = _referenceDataEntityService.GetReferenceValues(EnumHelper.EntityType.BusinessType.ToString());
        Businesses presentBusiness = _genericRepository.Get<Businesses>(b => b.Id == businessId && b.DeletedAt == null)!;
        if (presentBusiness != null)
        {
            businessViewModel.BusinessId = presentBusiness.Id;
            businessViewModel.BusinessName = presentBusiness.BusinessName;
            businessViewModel.BusinescategoryId = presentBusiness.BusinessCategoryId;
            businessViewModel.BusinessTypeId = presentBusiness.BusinessTypeId;
            businessViewModel.MobileNumber = presentBusiness.MobileNumber == null ? 0 : (long)presentBusiness.MobileNumber;
            businessViewModel.GSTIN = presentBusiness.GSTIN;
            if (presentBusiness.LogoAttachmentId.HasValue)
            {
                businessViewModel.BusinessLogoAttachment = _attachmentService.GetAttachmentById(presentBusiness.LogoAttachmentId.Value);
            }
            if (presentBusiness.AddressId == null)
            {
                businessViewModel.AddressLine1 = null;
                businessViewModel.AddressLine2 = null;
                businessViewModel.City = null;
                businessViewModel.Pincode = null;
            }
            else
            {
                AddressViewModel addressViewModel = _addressService.GetAddressById((int)presentBusiness.AddressId);
                businessViewModel.AddressLine1 = addressViewModel.AddressLine1;
                businessViewModel.AddressLine2 = addressViewModel.AddressLine2;
                businessViewModel.City = addressViewModel.City;
                businessViewModel.Pincode = addressViewModel.Pincode;
            }
            businessViewModel.IsActive = presentBusiness.IsActive;
            return new ApiResponse<BusinessItem>(true, null, businessViewModel, HttpStatusCode.OK);
        }
        return new ApiResponse<BusinessItem>(false, null, businessViewModel, HttpStatusCode.PartialContent);
    }

    public Businesses GetBusinessFromToken(string token)
    {
        try
        {
            ClaimsPrincipal claims = _jwttokenService.GetClaimsFromToken(token);
            int businessId = int.Parse(_jwttokenService.GetClaimValue(token, "id"));
            return _genericRepository.Get<Businesses>(b => b.Id == businessId && b.DeletedAt == null);
        }
        catch (Exception e)
        {
            throw new BusinessNotFoundException("Business Token Not found.");
        }

    }

    public async Task<ApiResponse<string>> DeleteBusiness(int businessId, int userId)
    {
        try
        {
            if (businessId == 0 || userId == 0)
            {
                return new ApiResponse<string>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
            }
            if (!CanDeleteBusiness(businessId, userId))
            {
                return new ApiResponse<string>(false, Messages.CanNotDeleteBusinessMessage, null, HttpStatusCode.Forbidden);
            }
            await _transactionRepository.BeginTransactionAsync();
            Businesses business = _genericRepository.Get<Businesses>(b => b.Id == businessId && !b.DeletedAt.HasValue)!;
            if (business != null)
            {
                business.DeletedAt = DateTime.UtcNow;
                business.DeletedById = userId;
                _genericRepository.Update<Businesses>(business);

                List<UserBusinessMappings> userBusinessMappings = _genericRepository.GetAll<UserBusinessMappings>(ubm => ubm.BusinessId == businessId && !ubm.DeletedAt.HasValue).ToList();
                foreach (UserBusinessMappings mapping in userBusinessMappings)
                {
                    PersonalDetails personalDetails = _genericRepository.Get<PersonalDetails>(pd => pd.Id == mapping.PersonDetailId && !pd.DeletedAt.HasValue);
                    if (personalDetails != null)
                    {
                        personalDetails.DeletedAt = DateTime.UtcNow;
                        personalDetails.DeletedById = userId;
                        _genericRepository.Update<PersonalDetails>(personalDetails);
                    }
                    mapping.DeletedAt = DateTime.UtcNow;
                    mapping.DeletedById = userId;
                    _genericRepository.Update<UserBusinessMappings>(mapping);
                }

                List<Parties> parties = _genericRepository.GetAll<Parties>(p => p.BusinessId == businessId && !p.DeletedAt.HasValue).ToList();
                foreach (Parties party in parties)
                {
                    List<LedgerTransactions> transactions = _genericRepository.GetAll<LedgerTransactions>(t => t.PartyId == party.Id && !t.DeletedAt.HasValue).ToList();
                    foreach (LedgerTransactions transaction in transactions)
                    {
                        transaction.DeletedAt = DateTime.UtcNow;
                        transaction.DeletedById = userId;
                        _genericRepository.Update<LedgerTransactions>(transaction);
                    }
                    party.DeletedAt = DateTime.UtcNow;
                    party.DeletedById = userId;
                    _genericRepository.Update<Parties>(party);
                }
                await _genericRepository.SaveChangesAsync();
                await _transactionRepository.CommitAsync();
                string userName = _userService.GetuserNameById(userId);
                string message = string.Format(Messages.BusinessActivity, "Business", "deleted", userName);
                await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Delete, EnumHelper.ActivityEntityType.Business, businessId, userId);
                return new ApiResponse<string>(true, string.Format(Messages.GlobalAddUpdateMesage, "Business", "deleted"), null, HttpStatusCode.OK);
            }
            else
            {
                return new ApiResponse<string>(false, string.Format(Messages.GlobalAddUpdateMesage, "business", "delete"), null, HttpStatusCode.BadRequest);
            }
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackAsync();
            throw;
        }
    }

    public string GetBusinessNameById(int businessId)
    {
        return _genericRepository.Get<Businesses>(x => x.Id == businessId && !x.DeletedAt.HasValue).BusinessName;
    }

    //get all business including deleted business to show activity business drop down
    public List<BusinessViewModel> GetAllBusinesses(int userId)
    {
        int OwnerRoleId = _genericRepository.Get<Role>(x => x.RoleName == ConstantVariables.OwnerRole).Id;
        IEnumerable<UserBusinessMappings> query = _genericRepository.GetAll<UserBusinessMappings>(predicate: ubm => ubm.UserId == userId && ubm.RoleId == OwnerRoleId && (ubm.CreatedById == userId || (!ubm.DeletedAt.HasValue && ubm.IsActive)),
                thenIncludes: new List<Func<IQueryable<UserBusinessMappings>, IQueryable<UserBusinessMappings>>>
                {
                   x => x.Include(ubm => ubm.Business)
                            .ThenInclude(b => b.LogoAttachment)
                        .Include(ubm => ubm.Business)
                            .ThenInclude(b => b.BusinessCategory)
                        .Include(ubm => ubm.Business)
                            .ThenInclude(b => b.BusinessType)
                        .Include(ubm => ubm.User)
                });

        var businesses = query
            .GroupBy(ubm => ubm.BusinessId)
            .ToList();

        return businesses.Select(data =>
                {
                    UserBusinessMappings business = data.First();

                    return new BusinessViewModel
                    {
                        BusinessId = business.Business.Id,
                        BusienssName = business.Business.BusinessName,
                    };
                })
                .ToList();
    }

    //get all users of business
    public async Task<ApiResponse<List<UserViewmodel>>> GetUsersOfBusiness(int businessId, int userId)
    {
        if (businessId == 0 || userId == 0)
        {
            return new ApiResponse<List<UserViewmodel>>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        else
        {
            if (!CanEditBusiness(businessId, userId))
            {
                return new ApiResponse<List<UserViewmodel>>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
            }
            List<UserViewmodel> users = await _userBusinessMappingService.GetUsersByBusiness((int)businessId, userId);
            users = _userBusinessMappingService.SetPermissions(users, userId, (int)businessId);
            return new ApiResponse<List<UserViewmodel>>(true, null, users, HttpStatusCode.OK);
        }
    }

    //get user by id
    public async Task<ApiResponse<UserViewmodel>> GetUserById(int businessId, int curentUserId, int? userId)
    {
        if (businessId == 0 || curentUserId == 0)
        {
            return new ApiResponse<UserViewmodel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        if (!CanEditBusiness(businessId, curentUserId))
        {
            return new ApiResponse<UserViewmodel>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
        }
        UserViewmodel userDetail = new();
        if (userId != 0 || userId == null)
        {
            List<UserViewmodel> users = await _userBusinessMappingService.GetUsersByBusiness((int)businessId, curentUserId);
            users = _userBusinessMappingService.SetPermissions(users, curentUserId, (int)businessId);
            userDetail = users.FirstOrDefault(x => x.UserId == userId) == null ? new UserViewmodel() : users.FirstOrDefault(x => x.UserId == userId);
        }
        bool isMainOwner = _userBusinessMappingService.IsMainOwner((int)businessId, curentUserId);
        if (isMainOwner)
        {
            userDetail.CanAddOwner = true;
            userDetail.AllRoles = _roleService.GetAllRoles();
        }
        else
        {
            userDetail.AllRoles = _roleService.GetRolesExceptOwner();
        }
        return new ApiResponse<UserViewmodel>(true, null, userDetail, HttpStatusCode.OK);
    }

    //Save user details in business
    public async Task<ApiResponse<List<UserViewmodel>>> SaveUserDetails(UserViewmodel userViewmodel, List<int> selectedRole, int businessId, int userId)
    {
        if (businessId == 0 || userId == 0 || selectedRole.Count <= 0)
        {
            return new ApiResponse<List<UserViewmodel>>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        if (!CanEditBusiness(businessId, userId))
        {
            return new ApiResponse<List<UserViewmodel>>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
        }
        bool isUserPresent = false;
        string businessName = GetBusinessNameById(businessId);
        UserViewmodel user = _userService.GetuserById(userId, businessId);
        int createdUserId;
        int createdPersonaldetailId;

        //update user
        if (userViewmodel.UserId != 0)
        {
            if (!await CanEditDeleteUser(businessId, userId, userViewmodel.UserId))
            {
                return new ApiResponse<List<UserViewmodel>>(false, Messages.CanNotEditUserMessage, null, HttpStatusCode.Forbidden);
            }
            createdUserId = userViewmodel.UserId;
            if (userViewmodel.PersonalDetailId != null)
            {
                createdPersonaldetailId = await _userService.UpdatePersonalDetails(userViewmodel, userId);
            }
            else
            {
                createdPersonaldetailId = await _userService.SavePersonalDetails(userViewmodel, userId);
            }
            bool isMappingUpdated = await _userBusinessMappingService.UpdateUserBusinessMapping(createdUserId, businessId, selectedRole, createdPersonaldetailId, userId);
            for (int i = 0; i < selectedRole.Count; i++)
            {
                if (i == selectedRole.Count - 1)
                {
                    userViewmodel.RoleName += _roleService.GetRoleById(selectedRole[i]).RoleName;
                }
                else
                {
                    userViewmodel.RoleName += _roleService.GetRoleById(selectedRole[i]).RoleName + ", ";
                }
            }
            Task.Run(async () =>
            {
                await CommonMethods.UpdateRoleEmail(userViewmodel.Email, userViewmodel.RoleName, businessName, ConstantVariables.LoginLink);
            });

            string message = string.Format(Messages.UserInBusinessActivity, userViewmodel.FirstName + " " + userViewmodel.LastName, "updated", user.FirstName + " " + user.LastName);
            isUserPresent = true;
            await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Update, EnumHelper.ActivityEntityType.Business, businessId, userId, EnumHelper.ActivityEntityType.Role, userViewmodel.UserId);
        }
        else
        {
            if (!await CanAddOwner(businessId, userId))
            {
                int OwnerId = _roleService.GetOwnerRoleId();
                if (selectedRole.Contains(OwnerId))
                {
                    return new ApiResponse<List<UserViewmodel>>(false, Messages.CanNotAddOwnerMessage, null, HttpStatusCode.Forbidden);
                }
                return new ApiResponse<List<UserViewmodel>>(false, Messages.CanNotAddUserMessage, null, HttpStatusCode.Forbidden);
            }
            //add user
            if (_userService.IsUserRegistered(userViewmodel.Email))
            {
                userViewmodel.IsUserRegistered = true;
            }
            else
            {
                userViewmodel.IsUserRegistered = false;
            }
            int personalDetailsId = await _userService.SavePersonalDetails(userViewmodel, userId);
            if (personalDetailsId == 0)
            {
                return new ApiResponse<List<UserViewmodel>>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
            }

            ApplicationUser userPresent = _userService.GetuserByEmail(userViewmodel.Email);

            if (userPresent == null)
            {
                createdUserId = await _userService.SaveUser(userViewmodel);
                if (createdUserId == 0)
                {
                    return new ApiResponse<List<UserViewmodel>>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                userViewmodel.UserId = userPresent.Id;
                userViewmodel.PersonalDetailId = personalDetailsId;
                createdUserId = userPresent.Id;
            }
            for (int i = 0; i < selectedRole.Count; i++)
            {
                int userBusinessMappingId = await _userBusinessMappingService.SaveUserBusinessMapping(createdUserId, businessId, selectedRole[i], personalDetailsId, userId);
                if (userBusinessMappingId == 0)
                {
                    return new ApiResponse<List<UserViewmodel>>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
                }
                if (i == selectedRole.Count - 1)
                {
                    userViewmodel.RoleName += _roleService.GetRoleById(selectedRole[i]).RoleName;
                }
                else
                {
                    userViewmodel.RoleName += _roleService.GetRoleById(selectedRole[i]).RoleName + ", ";
                }
            }
            if (userPresent == null)
            {
                Task.Run(async () =>
                {
                    CommonMethods.CreateUserAndRoleEmail(userViewmodel.Email, userViewmodel.RoleName, businessName, ConstantVariables.LoginLink);
                });
            }
            else
            {
                Task.Run(async () =>
                {
                    CommonMethods.CreateRoleEmail(userViewmodel.Email, userViewmodel.RoleName, businessName, ConstantVariables.LoginLink);
                });
            }
            string message = string.Format(Messages.AddUserInBusinessActivity, userViewmodel.FirstName + " " + userViewmodel.LastName, user.FirstName + " " + user.LastName);
            isUserPresent = false;
            await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Add, EnumHelper.ActivityEntityType.Business, businessId, userId, EnumHelper.ActivityEntityType.Role, createdUserId);
            userViewmodel.UserId = createdUserId;
        }

        List<UserViewmodel> users = await _userBusinessMappingService.GetUsersByBusiness(businessId, userId);
        users = _userBusinessMappingService.SetPermissions(users, userId, businessId);
        if (isUserPresent)
        {
            return new ApiResponse<List<UserViewmodel>>(true, string.Format(Messages.GlobalAddUpdateMesage, "User", "updated"), users, HttpStatusCode.OK);
        }
        return new ApiResponse<List<UserViewmodel>>(true, string.Format(Messages.GlobalAddUpdateMesage, "User", "added"), users, HttpStatusCode.OK);

    }

    public async Task<ApiResponse<string>> DeleteUserFromBusiness(int userId, int businessId, ApplicationUser logedinUser)
    {
        if (userId == 0 || businessId == 0 || logedinUser.Id == 0)
        {
            return new ApiResponse<string>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        if (!CanEditBusiness(businessId, userId))
        {
            return new ApiResponse<string>(false, Messages.CanNotDeletUserMessage, null, HttpStatusCode.Forbidden);
        }
        if (!await CanEditDeleteUser(businessId, logedinUser.Id, userId))
        {
            return new ApiResponse<string>(false, Messages.CanNotDeletUserMessage, null, HttpStatusCode.Forbidden);
        }
        UserViewmodel user = _userService.GetuserById(userId, businessId);
        bool isDeletedMapping = await _userBusinessMappingService.DeleteUserBusinessMappingByBusinessId(userId, businessId, logedinUser.Id);
        if (isDeletedMapping)
        {
            string busienssName = GetBusinessNameById(businessId);
            Task.Run(async () =>
                      {
                          CommonMethods.DeleteUserEmail(user.Email, busienssName, ConstantVariables.LoginLink);
                      });
            string message = string.Format(Messages.UserInBusinessActivity, user.FirstName + " " + user.LastName, "deleted", logedinUser.FirstName + " " + logedinUser.LastName);

            await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Delete, EnumHelper.ActivityEntityType.Business, businessId, logedinUser.Id, EnumHelper.ActivityEntityType.Role, user.UserId);
            return new ApiResponse<string>(true, string.Format(Messages.GlobalAddUpdateMesage, "User", "deleted"));
        }
        else
        {
            return new ApiResponse<string>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "user", "delete"));
        }
    }

    public async Task<ApiResponse<string>> ActiveInactiveUser(int userId, bool isActive, int businessId, ApplicationUser logedinUser)
    {
        if (userId == 0 || businessId == 0)
        {
            return new ApiResponse<string>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        if (!CanEditBusiness(businessId, userId))
        {
            return new ApiResponse<string>(false, Messages.ForbiddenMessage, null, HttpStatusCode.Forbidden);
        }
        if (!await CanEditDeleteUser(businessId, logedinUser.Id, userId))
        {
            return new ApiResponse<string>(false, Messages.CanNotEditUserMessage, null, HttpStatusCode.Forbidden);
        }

        bool isUserUpdated = await _userBusinessMappingService.ActiveInactiveUser(userId, businessId, isActive, logedinUser.Id);
        if (isUserUpdated)
        {
            string message;
            UserViewmodel userUpdated = _userService.GetuserById(userId, businessId);
            if (isActive)
            {
                message = string.Format(Messages.UserInBusinessActivity, userUpdated.FirstName + " " + userUpdated.LastName, "activated", logedinUser.FirstName + " " + logedinUser.LastName);
                await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Update, EnumHelper.ActivityEntityType.Business, businessId, logedinUser.Id, EnumHelper.ActivityEntityType.Role, userId);
                return new ApiResponse<string>(true, string.Format(Messages.GlobalAddUpdateMesage, "User", "activated"), null, HttpStatusCode.OK);
            }
            else
            {
                message = string.Format(Messages.UserInBusinessActivity, userUpdated.FirstName + " " + userUpdated.LastName, "inactivated", logedinUser.FirstName + " " + logedinUser.LastName);
                await _activityLogService.SetActivityLog(message, EnumHelper.Actiontype.Update, EnumHelper.ActivityEntityType.Business, businessId, logedinUser.Id, EnumHelper.ActivityEntityType.Role, userId);
                return new ApiResponse<string>(true, string.Format(Messages.GlobalAddUpdateMesage, "User", "inactivated"), null, HttpStatusCode.OK);
            }
        }
        else
        {
            if (isActive)
            {
                return new ApiResponse<string>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "activate", "user"), null, HttpStatusCode.BadRequest);
            }
            else
            {
                return new ApiResponse<string>(false, string.Format(Messages.GlobalAddUpdateFailMessage, "inactivated", "user"), null, HttpStatusCode.BadRequest);
            }
        }
    }

    public ApiResponse<CookiesViewModel> GetBusinessData(int businessId, ApplicationUser user)
    {
        if (businessId == 0)
        {
            return new ApiResponse<CookiesViewModel>(false, Messages.ExceptionMessage, null, HttpStatusCode.BadRequest);
        }
        string businessToken = _jwttokenService.GenerateBusinessToken(businessId);
        List<BusinessViewModel> businessList = GetBusinesses(user.Id);
        List<string> businessNamesList = new();
        foreach (BusinessViewModel business in businessList)
        {
            string businessName = business.BusienssName + "-" + business.BusinessId;
            businessNamesList.Add(businessName);
        }
        string businessNamesMerged = string.Join(",", businessNamesList);

        CookiesViewModel cookiesViewModel = new()
        {
            BusinessId = businessId.ToString(),
            BusinessToken = businessToken,
            AllBusinesses = businessNamesMerged
        };
        return new ApiResponse<CookiesViewModel>(true, null, cookiesViewModel, HttpStatusCode.OK);
    }

    public bool CanEditBusiness(int businessId, int userId)
    {
        List<UserBusinessMappings> businessMappings = _genericRepository.GetAll<UserBusinessMappings>(x => x.BusinessId == businessId && x.UserId == userId && x.Role.RoleName == ConstantVariables.OwnerRole && !x.DeletedAt.HasValue).ToList();
        if (businessMappings.Count > 0)
        {
            return true;
        }
        return false;
    }

    public bool CanDeleteBusiness(int businessId, int userId)
    {
        List<UserBusinessMappings> businessMappings = _genericRepository.GetAll<UserBusinessMappings>(x => x.BusinessId == businessId && x.UserId == userId && x.Role.RoleName == ConstantVariables.OwnerRole && x.UserId == x.CreatedById && !x.DeletedAt.HasValue).ToList();
        if (businessMappings.Count > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> CanEditDeleteUser(int businessId, int userId, int userToUpdateId)
    {
        var busienss = GetBusinesses(userId).Where(x => x.BusinessId == businessId);
        if (_userBusinessMappingService.IsMainOwner(businessId, userId))
        {
            return true;
        }
        else
        {
            List<UserViewmodel> users = await _userBusinessMappingService.GetUsersByBusiness((int)businessId, userId);
            users = _userBusinessMappingService.SetPermissions(users, userId, businessId);
            var user = users.FirstOrDefault(x => x.UserId == userToUpdateId);
            if (user != null && user.CanEdit == true)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> CanAddOwner(int businessId, int userId)
    {
        if (_userBusinessMappingService.IsMainOwner(businessId, userId))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}