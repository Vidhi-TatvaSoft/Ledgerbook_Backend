using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IActivityLogService
{
    Task<ActivityLogs> SetActivityLog(string message, EnumHelper.Actiontype action, EnumHelper.ActivityEntityType entityType, int EntityTypeId, int? createdById = null, EnumHelper.ActivityEntityType? subEntityType = null, int? subEntityTypeId = null);
     ApiResponse<PaginationViewModel<ActivityLogsViewModel>> GetActivities(ActivityDataViewModel activityDataVM, int userId);
}
