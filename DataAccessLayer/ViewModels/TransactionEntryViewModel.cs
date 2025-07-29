using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Constant;

namespace DataAccessLayer.ViewModels;

public class TransactionEntryViewModel
{
    public int TransactionId { get; set; }
    public int PartyId { get; set; }
    public string? PartyName { get; set; }

    public string? PartyTypeString{ get; set; }

    [Required(ErrorMessage = MessageHelper.TransactionAmountRequire)]
    [Range(1, 99999, ErrorMessage = MessageHelper.TransactionAmountValidation)]
    public decimal TransactionAmount { get; set; }
    public byte TransactionType { get; set; }
    public EnumHelper.TransactionType TransactionTypeEnum { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreateDate { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public decimal Balance { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? DueDateString { get; set; } = null;

    public int? PaymentMethodId { get; set; }
    public string? BusinessName { get; set; }
    public bool IsSettleup { get; set; } = false;

}