using DataAccessLayer.Constant;

namespace DataAccessLayer.ViewModels;

public class ActivityLogsViewModel
{
    public int ActivityId { get; set; }
    public string Message { get; set; }
    public EnumHelper.Actiontype Action { get; set; }
    public EnumHelper.ActivityEntityType EntityType { get; set; }
    public int? EntityTypeId { get; set; } = null;
    public string? EntityTypeName { get; set; } = null;
    public DateTime CreatedAt { get; set; }
    public string ActivityDate { get; set; }
    public string ActivityTime { get; set; }
    public int? CreatedById { get; set; } = null;
    public string? createdByName { get; set; } = null;
    public EnumHelper.ActivityEntityType? SubEntityType { get; set; } = null;
    public int? SubEntityTypeId { get; set; } = null;
    public string? SubEntityTypeName { get; set; } = null;
    public string? TransactionType { get; set; } = null;
    public string? PartyName { get; set; } = null;
    public string? PartyType { get; set; } = null;
    public decimal? TransactionAMount { get; set; }
    public bool IsSettled { get; set; } = false;
    // public bool IsActiveInactive { get; set; } = false;
    public bool IsActive { get; set; } = false;
    public bool IsInactive { get; set; } = false;
}
