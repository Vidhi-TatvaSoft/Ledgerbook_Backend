using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Models;

public class ApplicationRole : IdentityRole<int>
{
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
