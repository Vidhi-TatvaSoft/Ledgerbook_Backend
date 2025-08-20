using System.Net;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IGenericRepo _genericRepository;
    public ActivityLogService(IGenericRepo genericRepo)
    {
        _genericRepository = genericRepo;
    }

    public async Task<ActivityLogs> SetActivityLog(string message, EnumHelper.Actiontype action, EnumHelper.ActivityEntityType entityType, int EntityTypeId, int? createdById = null, EnumHelper.ActivityEntityType? subEntityType = null, int? subEntityTypeId = null)
    {
        ActivityLogs activityLogs = new()
        {
            Message = message,
            Action = action,
            EntityType = entityType,
            EntityTypeId = EntityTypeId,
            SubEntityType = subEntityType,
            SubEntityTypeId = subEntityTypeId,
            CreatedAt = DateTime.Now,
            CreatedById = createdById
        };
        await _genericRepository.AddAsync<ActivityLogs>(activityLogs);
        return activityLogs;
    }

    public ApiResponse<PaginationViewModel<ActivityLogsViewModel>> GetActivities(ActivityDataViewModel activityDataVM, int userId)
    {
        if (activityDataVM == null || userId == 0)
        {
            return new ApiResponse<PaginationViewModel<ActivityLogsViewModel>>(false, Messages.ExceptionMessage, null,HttpStatusCode.BadRequest);
        }
        List<ActivityLogs> activities = new();
        List<int?> businessIdsList = _genericRepository.GetAll<ActivityLogs>(x => x.EntityType == EnumHelper.ActivityEntityType.Business).Select(x => x.EntityTypeId).Distinct().ToList();
        List<int?> businessIdToDisplay = new();
        int OwnerRoleId = _genericRepository.Get<Role>(x => x.RoleName == ConstantVariables.OwnerRole).Id;
        foreach (int businessId in businessIdsList)
        {
            IEnumerable<UserBusinessMappings> mappings = _genericRepository.GetAll<UserBusinessMappings>(x => x.BusinessId == businessId && !x.DeletedAt.HasValue && x.RoleId == OwnerRoleId && x.IsActive);
            if (mappings.Any(x => x.UserId == userId))
            {
                businessIdToDisplay.Add(businessId);
            }
        }
        switch (activityDataVM.EntityType)
        {
            case "-1":
                activities = _genericRepository.GetAll<ActivityLogs>(x => x.CreatedById == userId || (x.EntityType == EnumHelper.ActivityEntityType.Business && businessIdToDisplay.Contains(x.EntityTypeId))).ToList();
                break;
            case "User":
                activities = _genericRepository.GetAll<ActivityLogs>(x => x.CreatedById == userId && x.EntityType == EnumHelper.ActivityEntityType.User).ToList();
                break;
            case "Business":
                activities = _genericRepository.GetAll<ActivityLogs>(x => x.CreatedById == userId && x.EntityType == EnumHelper.ActivityEntityType.Business || (x.EntityType == EnumHelper.ActivityEntityType.Business && businessIdToDisplay.Contains(x.EntityTypeId))).ToList();
                if (activityDataVM.BusinessId != 0)
                {
                    activities = activities.Where(x => x.EntityTypeId == activityDataVM.BusinessId).ToList();
                }
                if (activityDataVM.SubEntityType != "-1")
                {
                    if (activityDataVM.SubEntityType == EnumHelper.ActivityEntityType.Party.ToString())
                    {
                        List<ActivityLogs> transactionActivity = activities.Where(x => x.SubEntityType == EnumHelper.ActivityEntityType.Transaction).ToList();
                        activities = activities.Where(x => x.SubEntityType == EnumHelper.ActivityEntityType.Party).ToList();
                        List<int?> partyIds = activities.Select(x => x.SubEntityTypeId).ToList();
                        foreach (ActivityLogs transaction in transactionActivity)
                        {
                            LedgerTransactions ledgerTransactions = _genericRepository.Get<LedgerTransactions>(x => x.Id == transaction.SubEntityTypeId);
                            if (partyIds.Contains(ledgerTransactions.PartyId))
                            {
                                activities.Add(transaction);
                            }
                        }
                        if (activityDataVM.PartyId != 0)
                        {
                            activities = activities.Where(x => x.SubEntityTypeId == activityDataVM.PartyId).ToList();
                            foreach (ActivityLogs transaction in transactionActivity)
                            {
                                LedgerTransactions ledgerTransactions = _genericRepository.Get<LedgerTransactions>(x => x.Id == transaction.SubEntityTypeId);
                                if (ledgerTransactions.PartyId == activityDataVM.PartyId)
                                {
                                    activities.Add(transaction);
                                }
                            }
                        }
                    }
                    else if (activityDataVM.SubEntityType == EnumHelper.ActivityEntityType.Transaction.ToString())
                    {
                        activities = activities.Where(x => x.SubEntityType == EnumHelper.ActivityEntityType.Transaction).ToList();
                    }
                    else
                    {
                        activities = activities.Where(x => x.SubEntityType == EnumHelper.ActivityEntityType.Role).ToList();
                    }
                }
                break;
        }

        switch (activityDataVM.ActionName)
        {
            case "-1":
                activities = activities.OrderByDescending(x => x.CreatedAt).ToList();
                break;
            case "Add":
                activities = activities.Where(x => x.Action == EnumHelper.Actiontype.Add).OrderByDescending(x => x.CreatedAt).ToList();
                break;
            case "Update":
                activities = activities.Where(x => x.Action == EnumHelper.Actiontype.Update).OrderByDescending(x => x.CreatedAt).ToList();
                break;
            case "Delete":
                activities = activities.Where(x => x.Action == EnumHelper.Actiontype.Delete).OrderByDescending(x => x.CreatedAt).ToList();
                break;
        }

        List<ActivityLogsViewModel> activityList = activities.Select(x => new ActivityLogsViewModel
        {
            ActivityId = x.Id,
            Message = x.Message,
            Action = x.Action,
            ActionString = x.Action.ToString(),
            EntityType = x.EntityType,
            EntityTypeString = x.EntityType.ToString(),
            EntityTypeId = x.EntityTypeId,
            CreatedAt = x.CreatedAt,
            ActivityDate = x.CreatedAt.ToString("dd MMMM yyyy"),
            ActivityTime = x.CreatedAt.ToString("hh:mm:ss tt"),
            CreatedById = x.CreatedById,
            SubEntityType = x.SubEntityType,
            SubEntityTypeString = x.SubEntityType.ToString(),
            SubEntityTypeId = x.SubEntityTypeId
        }).ToList();

        foreach (ActivityLogsViewModel activity in activityList)
        {
            ApplicationUser usercreatedBy = _genericRepository.Get<ApplicationUser>(x => x.Id == activity.CreatedById);
            activity.createdByName = usercreatedBy.FirstName + " " + usercreatedBy.LastName;
            switch (activity.EntityType)
            {
                case EnumHelper.ActivityEntityType.User:
                    ApplicationUser user = _genericRepository.Get<ApplicationUser>(x => x.Id == activity.EntityTypeId);
                    activity.EntityTypeName = user.FirstName + " " + user.LastName;
                    break;
                case EnumHelper.ActivityEntityType.Business:
                    Businesses business = _genericRepository.Get<Businesses>(x => x.Id == activity.EntityTypeId);
                    activity.EntityTypeName = business.BusinessName;
                    break;
            }

            if (activity.SubEntityType != null)
            {
                switch (activity.SubEntityType)
                {
                    case EnumHelper.ActivityEntityType.Party:
                        Parties party = _genericRepository.Get<Parties>(x => x.Id == activity.SubEntityTypeId);
                        activity.PartyName = party.PartyName;
                        activity.PartyType = _genericRepository.Get<ReferenceDataValues>(x => x.Id == party.PartyTypId).EntityValue;
                        break;
                    case EnumHelper.ActivityEntityType.Transaction:
                        LedgerTransactions ledgerTransactions = _genericRepository.Get<LedgerTransactions>(x => x.Id == activity.SubEntityTypeId);
                        Parties party2 = _genericRepository.Get<Parties>(x => x.Id == ledgerTransactions.PartyId);
                        activity.PartyName = party2.PartyName;
                        activity.PartyType = _genericRepository.Get<ReferenceDataValues>(x => x.Id == party2.PartyTypId).EntityValue;
                        activity.TransactionAMount = _genericRepository.Get<LedgerTransactions>(x => x.Id == activity.SubEntityTypeId).Amount;
                        break;
                    case EnumHelper.ActivityEntityType.Role:
                        ApplicationUser userAdded = _genericRepository.Get<ApplicationUser>(x => x.Id == activity.SubEntityTypeId);
                        activity.SubEntityTypeName = userAdded.FirstName + " " + userAdded.LastName;
                        if (activity.Message.Contains("inactivated"))
                        {
                            activity.IsInactive = true;
                        }
                        else if (activity.Message.Contains("activated"))
                        {
                            activity.IsActive = true;
                        }
                        break;
                }
                if (activity.Action == EnumHelper.Actiontype.Add && activity.SubEntityType == EnumHelper.ActivityEntityType.Transaction && activity.Message.Contains("mark as paid"))
                {
                    activity.IsSettled = true;
                }
            }
        }

        int totalCount = activityList.Count();
        List<ActivityLogsViewModel> items = activityList.Skip((activityDataVM.PageNumber - 1) * activityDataVM.PageSize).Take(activityDataVM.PageSize).ToList();
        PaginationViewModel<ActivityLogsViewModel> paginatedData = new PaginationViewModel<ActivityLogsViewModel>(items, totalCount, activityDataVM.PageNumber, activityDataVM.PageSize);
        return new ApiResponse<PaginationViewModel<ActivityLogsViewModel>>(true, null, paginatedData, HttpStatusCode.OK);
    }
}
