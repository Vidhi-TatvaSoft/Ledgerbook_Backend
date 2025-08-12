using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Constant;

namespace DataAccessLayer.ViewModels;

public class SavePartyViewModel
{
    public int PartyId { get; set; }

    [Required(ErrorMessage = MessageHelper.PartyNameRequire)]
    [StringLength(100, ErrorMessage = MessageHelper.PartyNameNameLengthMessage)]
    public string PartyName { get; set; }

    [Required(ErrorMessage = MessageHelper.EmailRequireMessage)]
    [EmailAddress(ErrorMessage = MessageHelper.ValidEmailMessage)]
    [StringLength(255, ErrorMessage = MessageHelper.EmailLengthMessage)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,8}$", ErrorMessage = MessageHelper.ValidEmailMessage)]
    public string Email { get; set; }
    public int PartyTypId { get; set; }

    public EnumHelper.PartyType PartyTypeString { get; set; }

    public int BusinessId { get; set; }

    [Range(1, 10000, ErrorMessage = MessageHelper.TransactionAmountValidation)]
    public decimal? Amount { get; set; }
    public EnumHelper.TransactionType? TransactionType { get; set; }
    public bool IsEmailChaneged { get; set; } = false;
}
