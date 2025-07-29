using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

public class UserBusinessMappings
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int BusinessId { get; set; }

    public int RoleId { get; set; }

    public int? PersonDetailId  { get; set; }

    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedById { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsActive { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    [ForeignKey("BusinessId")]
    public virtual Businesses Business { get; set; }

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; }

    [ForeignKey("PersonDetailId ")]
    public virtual PersonalDetails PersonalDetails { get; set; } = null;
    
    [ForeignKey("CreatedById")]
    public virtual ApplicationUser CreatedUser { get; set; }

    [ForeignKey("UpdatedById")]
    public virtual ApplicationUser UpdatedUser { get; set; } = null!;

    [ForeignKey("DeletedById")]
    public virtual ApplicationUser DeletedUser { get; set; } = null!;
}
