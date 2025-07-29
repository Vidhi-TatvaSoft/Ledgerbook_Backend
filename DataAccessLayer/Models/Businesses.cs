using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

public class Businesses
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(100)]
    public string BusinessName { get; set; }

    [Range(0,999999999999999)]
    public long? MobileNumber { get; set; }
    public int? AddressId { get; set; }
    public int? LogoAttachmentId { get; set; }
    public int BusinessCategoryId { get; set; }
    public int BusinessTypeId { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedById { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsActive { get; set; }
    [StringLength(15)]
    public string? GSTIN { get; set; } = null;


    [ForeignKey("AddressId")]
    public virtual Address Address { get; set; } = null!;

    [ForeignKey("LogoAttachmentId")]
    public virtual Attachment LogoAttachment { get; set; } = null!;


    [ForeignKey("BusinessCategoryId")]
    public virtual ReferenceDataValues BusinessCategory { get; set; }

    [ForeignKey("BusinessTypeId")]
    public virtual ReferenceDataValues BusinessType { get; set; }

    [ForeignKey("CreatedById")]
    public virtual ApplicationUser CreatedUser { get; set; }

    [ForeignKey("UpdatedById")]
    public virtual ApplicationUser UpdatedUser { get; set; } = null!;

    [ForeignKey("DeletedById")]
    public virtual ApplicationUser DeletedUser { get; set; } = null!;
}
