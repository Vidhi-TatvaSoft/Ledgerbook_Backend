namespace BusinessAcessLayer.Constant;

public class EmailTemplates
{
    public static string GetUserAddedEmailTemplate(string userEmail, string businessName, string loginLink)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset=""UTF-8"" />
        <title>You've Been Added to a Business on Ledger Book</title>
        </head>
        <body style=""font-family: 'Segoe UI', sans-serif; background-color: #f0f9f8; margin: 0; padding: 0; color: #333;"">
        <div style=""max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);"">
        <!-- Header -->
        <div style=""background-color: #126773; color: white; padding: 40px 30px; text-align: center;"">
        <h1 style=""margin: 0; font-size: 24px;"">Welcome to Ledger Book</h1>
        </div>
        
        <!-- Content -->
        <div style=""padding: 30px;"">
        <h2 style=""color: #126773; margin-top: 0;"">You've been added to a business</h2>
        <p style=""font-size: 16px; line-height: 1.6;"">
            Hello,<br><br>
            You’ve just been added to the business <strong>{{BusinessName}}</strong> in <strong>Ledger Book</strong> by <strong>Admin</strong>.
        </p>
        
            <!-- Info Box -->
        <div style=""background-color: #f4fdfd; border-left: 4px solid #126773; padding: 15px; margin-top: 20px; font-size: 15px;"">
        <strong>Your Email:</strong> {userEmail}<br />
        <strong>Business Name:</strong> {businessName}<br />
        <strong>Added By:</strong> Admin
        </div>
        
            <p style=""font-size: 16px; line-height: 1.6; margin-top: 25px;"">
            You can now access the business dashboard, manage records, and track expenses.
        </p>
        
            <!-- Button -->
        <a href={loginLink} style=""display: inline-block; margin-top: 25px; padding: 14px 28px; background-color: #126773; color: white; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 16px;"">
            Log In to Ledger Book
        </a>
        
            <!-- Closing -->
        <p style=""margin-top: 30px; text-align: center; font-size: 16px;"">
            Happy budgeting!<br />
        <strong>The Ledger Book Team</strong>
        </p>
        </div>
        
        <!-- Footer -->
        <div style=""text-align: center; padding: 25px; font-size: 13px; color: #666; background-color: #f8f8f8;"">
        &copy; 2025 Ledger Book &nbsp;|&nbsp;
        <a href={loginLink} style=""color: #126773; text-decoration: none;"">Visit our site</a>
        </div>
        </div>
        </body>
        </html>";
    }

    public static string GetRoleUpdatedEmailTemplate(string role, string business, string loginlink)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
            <meta charset=""UTF-8"" />
            <title>Role Updated – Ledger Book</title>
            </head>
            <body style=""font-family: 'Segoe UI', sans-serif; background-color: #f0f9f8; margin: 0; padding: 0; color: #333;"">
            <div style=""max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);"">
            <!-- Header -->
            <div style=""background-color: #126773; color: white; padding: 40px 30px; text-align: center;"">
            <h1 style=""margin: 0; font-size: 24px;"">Role Updated</h1>
            </div>
            
                <!-- Content -->
            <div style=""padding: 30px;"">
            <h2 style=""color: #126773; margin-top: 0;"">Hello,</h2>
            <p style=""font-size: 16px; line-height: 1.6;"">
                    Your role has been updated to <strong>{role}</strong> in the <strong>{business}</strong> business on <strong>Ledger Book</strong>.
            </p>
            
                    <div style=""background-color: #f4fdfd; border-left: 4px solid #126773; padding: 15px; margin-top: 20px; font-size: 15px;"">
            <strong>New Role:</strong> {role}<br />
            <strong>Business:</strong> {business}
            </div>
            
                    <p style=""font-size: 16px; line-height: 1.6; margin-top: 25px;"">
                    You can now log in using your credentials to access your updated permissions.
            </p>
            
                    <a href={loginlink} style=""display: inline-block; margin-top: 25px; padding: 14px 28px; background-color: #126773; color: white; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 16px;"">Log In to Ledger Book</a>
            
                    <p style=""margin-top: 30px; text-align: center; font-size: 16px;"">
                    Happy budgeting!<br />
            <strong>The Ledger Book Team</strong>
            </p>
            </div>
            
                <!-- Footer -->
            <div style=""text-align: center; padding: 25px; font-size: 13px; color: #666; background-color: #f8f8f8;"">
            &copy; 2025 Ledger Book &nbsp;|&nbsp;
            <a href={loginlink} style=""color: #126773; text-decoration: none;"">Visit our site</a>
            </div>
            </div>
            </body>
            </html>";
    }

    public static string UserDeletedEmailTemplate(string business, string loginlink)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
            <meta charset=""UTF-8"" />
            <title>Delete user - Ledger Book</title>
            </head>
            <body style=""font-family: 'Segoe UI', sans-serif; background-color: #f0f9f8; margin: 0; padding: 0; color: #333;"">
            <div style=""max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);"">
            <!-- Header -->
            <div style=""background-color: #126773; color: white; padding: 40px 30px; text-align: center;"">
            <h1 style=""margin: 0; font-size: 24px;"">Removed from business</h1>
            </div>
            
                <!-- Content -->
            <div style=""padding: 30px;"">
            <h2 style=""color: #126773; margin-top: 0;"">Hello, </h2>
            <p style=""font-size: 16px; line-height: 1.6;"">
                    Sorry to inform you that you have been removed from <strong>{business}</strong> business on <strong>Ledger Book</strong>.
            </p>
            
                    <div style=""background-color: #f4fdfd; border-left: 4px solid #126773; padding: 15px; margin-top: 20px; font-size: 15px;"">
            <strong>Business:</strong> {business}
            </div>
            
                    <p style=""font-size: 16px; line-height: 1.6; margin-top: 25px;"">
                    Thank you for using our services.
            </p>
            
                    <a href={loginlink} style=""display: inline-block; margin-top: 25px; padding: 14px 28px; background-color: #126773; color: white; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 16px;"">Log In to Ledger Book</a>
            
                    <p style=""margin-top: 30px; text-align: center; font-size: 16px;"">
                    Happy budgeting!<br />
            <strong>The Ledger Book Team</strong>
            </p>
            </div>
            
                <!-- Footer -->
            <div style=""text-align: center; padding: 25px; font-size: 13px; color: #666; background-color: #f8f8f8;"">
            &copy; 2025 Ledger Book &nbsp;|&nbsp;
            <a href={loginlink} style=""color: #126773; text-decoration: none;"">Visit our site</a>
            </div>
            </div>
            </body>
            </html>";
    }

    public static string GenerateBusinessAddedEmail(string business, string role, string email, string loginLink)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset=""UTF-8"" />
        <title>Welcome to Ledger Book</title>
        </head>
        <body style=""font-family: 'Segoe UI', sans-serif; background-color: #f0f9f8; padding: 30px; color: #333;"">
        <div style=""max-width: 600px; margin: auto; background-color: #ffffff; padding: 25px; border-radius: 10px; box-shadow: 0 0 15px rgba(0,0,0,0.1);"">
        <div style=""background-color: #126773; color: white; padding: 40px 30px; text-align: center;"">
        <h1 style=""margin: 0; font-size: 24px;"">You've been added to a business</h1>
        </div>
        <p style=""font-size: 16px; line-height: 1.5;"">
            You have been added to <strong>{business}</strong> as <strong>{role}</strong>.
        </p>
        
            <p style=""font-size: 16px; line-height: 1.5;"">
            You can log in using your credentials:
        </p>
        
            <div style=""background-color: #f4fdfd; border-left: 4px solid #126773; padding: 15px; font-size: 15px; margin: 20px 0;"">
        <strong>Email:</strong> {email}<br />
        </div>
        
            <a href={loginLink} style=""display: inline-block; margin-top: 20px; padding: 12px 24px; background-color: #126773; color: white; text-decoration: none; border-radius: 6px; font-weight: bold;"">
            Log In to Ledger Book
        </a>
        
            <p style=""margin-top: 30px; font-size: 15px; text-align: center;"">
            Happy budgeting!<br />
        <strong>The Ledger Book Team</strong>
        </p>
        </div>
        </body>
        </html>";
    }

    public static string GenerateRegistrationSuccessEmail(string userName, string verificationLink, string loginLink)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset=""UTF-8"" />
        <title>Registration Successful – Ledger Book</title>
        </head>
        <body style=""font-family: 'Segoe UI', sans-serif; background-color: #f0f9f8; padding: 0; margin: 0; color: #333;"">
        <div style=""max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);"">
        
            <!-- Header -->
        <div style=""background-color: #126773; color: white; padding: 40px 30px; text-align: center;"">
        <h1 style=""margin: 0; font-size: 24px;"">Welcome to Ledger Book</h1>
        </div>
        
            <!-- Body -->
        <div style=""padding: 30px;"">
        <h2 style=""color: #126773; margin-top: 0;"">Hello, {userName}</h2>
        
            <p style=""font-size: 16px; line-height: 1.6;"">
                You’ve successfully registered with <strong>Ledger Book</strong>.
        </p>
        
            <p style=""font-size: 16px; line-height: 1.6;"">
                Start managing your business records, expenses, and finances all in one place.
        </p>
        <p style=""font-size: 16px; line-height: 1.6;"">
               Before Login to your account verify your Email by clicking on this button
        </p>
        
            <a href={verificationLink} style=""display: inline-block; margin-top: 25px; padding: 14px 28px; background-color: #126773; color: white; text-decoration: none; border-radius: 6px; font-weight: bold;"">
               Verify Email
        </a>
        
            <p style=""margin-top: 30px; text-align: center; font-size: 16px;"">
                Happy budgeting!<br />
        <strong>The Ledger Book Team</strong>
        </p>
        </div>
        
            <!-- Footer -->
        <div style=""text-align: center; padding: 25px; font-size: 13px; color: #666; background-color: #f8f8f8;"">
        &copy; 2025 Ledger Book &nbsp;|&nbsp;
        <a href={loginLink} style=""color: #126773; text-decoration: none;"">Visit our site</a>
        </div>
        </div>
        </body>
        </html>";
    }

    public static string PartyEmailVerificationEmail(string userName, string verificationLink, string partyType, string businessName)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset=""UTF-8"" />
        <title>Party Added – Ledger Book</title>
        </head>
        <body style=""font-family: 'Segoe UI', sans-serif; background-color: #f0f9f8; padding: 0; margin: 0; color: #333;"">
        <div style=""max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);"">
        
            <!-- Header -->
        <div style=""background-color: #126773; color: white; padding: 40px 30px; text-align: center;"">
        <h1 style=""margin: 0; font-size: 24px;"">Welcome to Ledger Book</h1>
        </div>
        
            <!-- Body -->
        <div style=""padding: 30px;"">
        <h2 style=""color: #126773; margin-top: 0;"">Hello, {userName}</h2>
        
            <p style=""font-size: 16px; line-height: 1.6;"">
                You've been added to <strong>{businessName}</strong> business as <strong>{partyType}</strong>.
        </p>
        
        <p style=""font-size: 16px; line-height: 1.6;"">
               Before Login to your account verify your Email by clicking on this button
        </p>
        
            <a href={verificationLink} style=""display: inline-block; margin-top: 25px; padding: 14px 28px; background-color: #126773; color: white; text-decoration: none; border-radius: 6px; font-weight: bold;"">
               Verify Email
        </a>
        
            <p style=""margin-top: 30px; text-align: center; font-size: 16px;"">
                Happy budgeting!<br />
        <strong>The Ledger Book Team</strong>
        </p>
        </div>
        
            <!-- Footer -->
        <div style=""text-align: center; padding: 25px; font-size: 13px; color: #666; background-color: #f8f8f8;"">
        &copy; 2025 Ledger Book &nbsp;|&nbsp;
        </div>
        </div>
        </body>
        </html>";
    }

    public static string GenerateResetPasswordEmail(string userName, string resetPasswordLink, string loginLink)
    {
        return $@"
       <!DOCTYPE html>
        <html>
        <head>
        <meta charset=""UTF-8"" />
        <title>Reset password - Ledger Book</title>
        </head>
        <body style=""font-family: 'Segoe UI', sans-serif; background-color: #f0f9f8; padding: 0; margin: 0; color: #333;"">
        <div style=""max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);"">
        
            <!-- Header -->
        <div style=""background-color: #126773; color: white; padding: 40px 30px; text-align: center;"">
        <h1 style=""margin: 0; font-size: 24px;"">Welcome to Ledger Book</h1>
        </div>
        
            <!-- Body -->
        <div style=""padding: 30px;"">
        <h2 style=""color: #126773; margin-top: 0;"">Hello, {userName}</h2>
        
            <p style=""font-size: 16px; line-height: 1.6;"">
                Forgot your password?
        </p>
        
            <p style=""font-size: 16px; line-height: 1.6;"">
                We have received a request to reset the password of your account.
        </p>
        <p style=""font-size: 16px; line-height: 1.6;"">
                To reset your password click on the link below:
        </p>
        
            <a href={resetPasswordLink} style=""display: inline-block; margin-top: 25px; padding: 14px 28px; background-color: #126773; color: white; text-decoration: none; border-radius: 6px; font-weight: bold;"">
               Reset Password
        </a>
        
            <p style=""margin-top: 30px; text-align: center; font-size: 16px;"">
                Happy budgeting!<br />
        <strong>The Ledger Book Team</strong>
        </p>
        </div>
        
            <!-- Footer -->
        <div style=""text-align: center; padding: 25px; font-size: 13px; color: #666; background-color: #f8f8f8;"">
        &copy; 2025 Ledger Book &nbsp;|&nbsp;
        <a href={loginLink} style=""color: #126773; text-decoration: none;"">Visit our site</a>
        </div>
        </div>
        </body>
        </html>";
    }

    public static string GenerateChangePasswordEmail(string userName, string loginLink)
    {
        return $@"
       <!DOCTYPE html>
        <html>
        <head>
        <meta charset=""UTF-8"" />
        <title>Change password - Ledger Book</title>
        </head>
        <body style=""font-family: 'Segoe UI', sans-serif; background-color: #f0f9f8; padding: 0; margin: 0; color: #333;"">
        <div style=""max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);"">
        
            <!-- Header -->
        <div style=""background-color: #126773; color: white; padding: 40px 30px; text-align: center;"">
        <h1 style=""margin: 0; font-size: 24px;"">Welcome to Ledger Book</h1>
        </div>
        
            <!-- Body -->
        <div style=""padding: 30px;"">
        <h2 style=""color: #126773; margin-top: 0;"">Hello, {userName}</h2>
        
            <p style=""font-size: 16px; line-height: 1.6;"">
               This is to confirm that the password for your account has been successfully changed. Your account is now secured with the new password that you have set.
        </p>
        
            <p style=""font-size: 16px; line-height: 1.6;"">
                If you did not change your password, please contact us immediately to report any unauthorized access to your account.
        </p>
        <p style=""font-size: 16px; line-height: 1.6;"">
            Thank you for using our service.
        </p>
        
            <p style=""margin-top: 30px; text-align: center; font-size: 16px;"">
                Happy budgeting!<br />
        <strong>The Ledger Book Team</strong>
        </p>
        </div>
        
            <!-- Footer -->
        <div style=""text-align: center; padding: 25px; font-size: 13px; color: #666; background-color: #f8f8f8;"">
        &copy; 2025 Ledger Book &nbsp;|&nbsp;
        <a href={loginLink} style=""color: #126773; text-decoration: none;"">Visit our site</a>
        </div>
        </div>
        </body>
        </html>";
    }

    public static string PartyEmailVerificationEmail(string partyType, string businessName, decimal amount)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset=""UTF-8"" />
        <title>Due Payment - Ledger Book</title>
        </head>
        <body style=""font-family: 'Segoe UI', sans-serif; background-color: #f0f9f8; padding: 0; margin: 0; color: #333;"">
        <div style=""max-width: 600px; margin: 30px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);"">
        
            <!-- Header -->
        <div style=""background-color: #126773; color: white; padding: 40px 30px; text-align: center;"">
        <h1 style=""margin: 0; font-size: 24px;"">Welcome to Ledger Book</h1>
        </div>
        
            <!-- Body -->
        <div style=""padding: 30px;"">
        <h2 style=""color: #126773; margin-top: 0;"">Hello, {partyType}</h2>
        
            <p style=""font-size: 16px; line-height: 1.6;"">
                your payment of <strong>Rs. {amount}</strong> is pending with <strong>{businessName}</strong>.
        </p>
        
        <p style=""font-size: 16px; line-height: 1.6;"">
               For more details contact owner of {businessName}.
        </p>
        
            <p style=""margin-top: 30px; text-align: center; font-size: 16px;"">
                Happy budgeting!<br />
        <strong>The Ledger Book Team</strong>
        </p>
        </div>
        
            <!-- Footer -->
        <div style=""text-align: center; padding: 25px; font-size: 13px; color: #666; background-color: #f8f8f8;"">
        &copy; 2025 Ledger Book;
        </div>
        </div>
        </body>
        </html>";
    }

}
