using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

public class Parties
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(200)]
    public string PartyName { get; set; }

    public int PartyTypId { get; set; }

    public int BusinessId { get; set; }

    [StringLength(250)]
    public string? Email { get; set; }

    public decimal? GSTIN { get; set; }

    public int? AddressId { get; set; }

    public string? VerificationToken{ get; set; }

    public bool IsEmailVerified { get; set; } = false;

    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedById { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("PartyTypeId")]
    public virtual ReferenceDataValues PartyType { get; set; }

    [ForeignKey("BusinessId")]
    public virtual Businesses Business { get; set; }

    [ForeignKey("AddressId")]
    public virtual Address Address { get; set; } = null!;

    [ForeignKey("CreatedById")]
    public virtual ApplicationUser CreatedUser { get; set; }

    [ForeignKey("UpdatedById")]
    public virtual ApplicationUser UpdatedUser { get; set; } = null!;

    [ForeignKey("DeletedById")]
    public virtual ApplicationUser DeletedUser { get; set; } = null!;

}
