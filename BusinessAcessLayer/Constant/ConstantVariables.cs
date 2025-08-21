using Microsoft.Extensions.Configuration;

namespace BusinessAcessLayer.Constant;

public class ConstantVariables
{
    public static string BasicPassword = "Vvv@1234";
    public static string SmtpServer = "mail.etatvasoft.com";
    public static int port = 587;
    public static string SenderEmail = "test.dotnet@etatvasoft.com";
    public static string SenderName = "Ledgerbook";
    public static string EmailServicePassword = "P}N^{z-]7Ilp";
    public static string EmailHost = "mail.etatvasoft.com";
    public static string OwnerRole = "Owner/Admin";
    public static string PurchaseManagerRole = "Purchase Manager";
    public static string SalesManagerRole = "Sales Manager";
    public static string PartyType = "PartyType";
    public static string RegisterSubject = "Verify it's you";
    public static string EmailSubject = "Added you to Business";
    public static string RoleUpdatedSubject = "Role has been updated";
    public static string RoleDeletedSubject = "Removed from business";
    public static string ResetPasswordSubject = "Reset your password";
    public static string ChangePasswordSubject = "Password updated";
    public static string PaymentDueSubject = "Due Payment";
}

public class TokenKey
{
    public static string BusinessToken = "BusinessToken";
    public static string UserToken = "UserToken";
    public static string Authorization = "Authorization";
}

public class PartyType
{
    public static string Customer = "Customer";
    public static string Supplier = "Supplier";
}