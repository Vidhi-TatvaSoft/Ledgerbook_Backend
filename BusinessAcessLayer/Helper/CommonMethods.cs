using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using MailKit.Net.Smtp;
using System.Net.Mail;
using MimeKit;
using System;
using DataAccessLayer.Constant;

namespace BusinessAcessLayer.Helper;

public class CommonMethods
{
    private readonly IJWTTokenService _jwttokenService;
    private readonly LedgerBookDbContext _context;
    private readonly IGenericRepo _genericRepository;

    public CommonMethods(LedgerBookDbContext context,
    IJWTTokenService jWTTokenService,
    IGenericRepo genericRepository
    )
    {
        _context = context;
        _jwttokenService = jWTTokenService;
        _genericRepository = genericRepository;
    }

    public static string Base64Encode(string plainText)
    {
        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static string UploadImage(IFormFile file, string folderPath)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = Guid.NewGuid() + "_" + file.FileName;
        string filePath = Path.Combine(folderPath, fileName);

        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return fileName;
    }

    public static async Task<bool> SendEmail(string receiver, string subject, string body)
    {
        MimeMessage email = new MimeMessage();
        email.From.Add(new MailboxAddress(Constant.ConstantVariables.SenderName, Constant.ConstantVariables.SenderEmail));
        email.To.Add(new MailboxAddress(receiver, receiver));
        email.Subject = subject;
        BodyBuilder bodyBuilder = new() { HtmlBody = body };
        email.Body = bodyBuilder.ToMessageBody();

        using MailKit.Net.Smtp.SmtpClient smtp = new MailKit.Net.Smtp.SmtpClient();

        await smtp.ConnectAsync(Constant.ConstantVariables.SmtpServer, Constant.ConstantVariables.port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(Constant.ConstantVariables.SenderEmail, Constant.ConstantVariables.EmailServicePassword);
        await smtp.SendAsync(email);
        return true;
    }

    public static async Task<bool> RegisterEmail(string userName, string Email, string verificationLink, string loginLink)
    {
        string subject = Constant.ConstantVariables.RegisterSubject;
        string body = Constant.EmailTemplates.GenerateRegistrationSuccessEmail(userName, verificationLink, loginLink);
        return await SendEmail(Email, subject, body);
    }

    public static async Task<bool> ResetPasswordEmail(string Email, string userName, string resetLink, string loginLink)
    {
        string subject = Constant.ConstantVariables.ResetPasswordSubject;
        string body = Constant.EmailTemplates.GenerateResetPasswordEmail(userName, resetLink, loginLink);
        return await SendEmail(Email, subject, body);
    }

    public static async Task<bool> ChangePasswordEmail(string Email, string userName, string loginLink)
    {
        string subject = Constant.ConstantVariables.ChangePasswordSubject;
        string body = Constant.EmailTemplates.GenerateChangePasswordEmail(userName, loginLink);
        return await SendEmail(Email, subject, body);
    }

    public static async Task<bool> CreateUserAndRoleEmail(string Email, string Role, string Business, string loginLink)
    {
        string subject = Constant.ConstantVariables.EmailSubject;
        string body = Constant.EmailTemplates.GenerateBusinessAddedEmail(
            business: Business,
            role: Role,
            email: Email,
            loginLink: loginLink

        );
        return await SendEmail(Email, subject, body);
    }

    public static async Task<bool> CreateRoleEmail(string Email, string Role, string Business, string loginLink)
    {
        string subject = Constant.ConstantVariables.EmailSubject;
        string body = Constant.EmailTemplates.GetUserAddedEmailTemplate().Replace("{{UserName}}", "User")
                .Replace("{{UserEmail}}", Email)
                .Replace("{{BusinessName}}", Business)
                .Replace("{{InviterName}}", "Admin")
                .Replace("{{loginLink}}", loginLink);

        return await SendEmail(Email, subject, body);
    }

    public static async Task<bool> UpdateRoleEmail(string Email, string Role, string Business, string loginLink)
    {
        string subject = Constant.ConstantVariables.RoleUpdatedSubject;
        string body = Constant.EmailTemplates.GetRoleUpdatedEmailTemplate(
            userName: Email,
            role: Role,
            business: Business,
            loginlink: loginLink
        );


        return await SendEmail(Email, subject, body);
    }

    public static async Task<bool> DeleteUserEmail(string Email, string Business, string loginLink)
    {
        string subject = Constant.ConstantVariables.RoleDeletedSubject;
        string body = Constant.EmailTemplates.UserDeletedEmailTemplate(
            business: Business,
            loginlink: loginLink
        );
        return await SendEmail(Email, subject, body);
    }

    public static async Task<bool> VerifyParty(string userName, string Email, string verificationLink, string partyType, string businessName)
    {
        string subject = Constant.ConstantVariables.RegisterSubject;
        string body = Constant.EmailTemplates.PartyEmailVerificationEmail(userName, verificationLink, partyType, businessName);
        return await SendEmail(Email, subject, body);
    }

    public static async Task<bool> Sendreminder(string Email, string partyType, string businessName, decimal amount)
    {
        string subject = Constant.ConstantVariables.PaymentDueSubject;
        string body = Constant.EmailTemplates.PartyEmailVerificationEmail(partyType, businessName, amount);
        return await SendEmail(Email, subject, body);
    }

    public static void AddLog(string message)
    {
        string logMessage = $"[{DateTime.UtcNow}] : {message}";
        string filePath = "log.txt";
        try
        {
            File.AppendAllText(filePath, logMessage + Environment.NewLine);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
    }
}