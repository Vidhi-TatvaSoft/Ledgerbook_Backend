using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

public class Role
{
    public int Id { get; set; }

    [StringLength(50)]
    public string RoleName { get; set; }

    public string Description { get; set; }

    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedById { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("CreatedById")]
    public virtual ApplicationUser CreatedUser { get; set; }

    [ForeignKey("UpdatedById")]
    public virtual ApplicationUser UpdatedUser { get; set; } = null!;

    [ForeignKey("DeletedById")]
    public virtual ApplicationUser DeletedUser { get; set; } = null!;

}
