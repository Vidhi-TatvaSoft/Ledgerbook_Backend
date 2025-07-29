using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

public class LedgerTransactions
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PartyId { get; set; }

    public decimal Amount { get; set; }

    public byte TransactionType { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int? PaymentMethodId { get; set; }

    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedById { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsSettled { get; set; } = false;

    [ForeignKey("PartyId")]
    public virtual Parties Party { get; set; }

    [ForeignKey("PaymentMethodId")]
    public virtual ReferenceDataValues Paymentmethod { get; set; } 

    [ForeignKey("CreatedById")]
    public virtual ApplicationUser CreatedUser { get; set; }

    [ForeignKey("UpdatedById")]
    public virtual ApplicationUser UpdatedUser { get; set; } = null!;

    [ForeignKey("DeletedById")]
    public virtual ApplicationUser DeletedUser { get; set; } = null!;


}
