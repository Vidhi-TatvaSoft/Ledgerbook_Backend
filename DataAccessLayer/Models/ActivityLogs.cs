using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLayer.Constant;

namespace DataAccessLayer.Models;

public class ActivityLogs
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Message { get; set; }

    public EnumHelper.Actiontype Action { get; set; } //Add,Edit,Delete

    public EnumHelper.ActivityEntityType EntityType { get; set; } //user, business

    public int? EntityTypeId { get; set; } = null; //id of entity type like bId, userId etc

    public DateTime CreatedAt { get; set; }

    public int? CreatedById { get; set; } = null;

    public EnumHelper.ActivityEntityType? SubEntityType { get; set; } = null; //party, transactions,Role

    public int? SubEntityTypeId { get; set; } = null; 

    [ForeignKey("CreatedById")]
    public virtual ApplicationUser CreatedUser { get; set; }
}
