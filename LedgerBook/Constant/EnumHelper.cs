namespace LedgerBook.Constant;

public class EnumHelper
{
    public enum SourceType
    {
        User,
        Business,
        Party
    }

    public enum EntityType
    {
        BusinessCategory,
        BusinessType,
    }

    public enum TransactionType
    {
        GOT = 1,
        GAVE = 2
    }

    public enum ActivityEntityType
    {
        User = 1,
        Business = 2,
        Party = 3,
        Transaction = 4,
        Role = 5
    }

    public enum Actiontype
    {
        Add = 1,
        Update = 2,
        Delete = 3
    }
}
