using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using ExpenseTracker.Dtos.Models;

namespace ExpenseTracker.Service.Services.Auth
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(User user);
        Task SendPasswordResetAsync(User user, string token);
    }

    public class ConsoleEmailService : IEmailService
    {
        public Task SendWelcomeEmailAsync(User user)
        {
            System.Console.WriteLine($"[Email] Welcome {user.Email}");
            return Task.CompletedTask;
        }

        public Task SendPasswordResetAsync(User user, string token)
        {
            System.Console.WriteLine($"[Email] Password reset for {user.Email}: token={token}");
            return Task.CompletedTask;
        }
    }

    public class GmailEmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly string _senderName;
        private readonly string _frontendBaseUrl;

        public GmailEmailService(IConfiguration configuration)
        {
            // Read email configuration from appsettings.json
            _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            _senderEmail = configuration["Email:SenderEmail"] ?? throw new InvalidOperationException("Email:SenderEmail is required in configuration");
            var rawPassword = configuration["Email:SenderPassword"] ?? throw new InvalidOperationException("Email:SenderPassword is required in configuration");
            _senderPassword = rawPassword.Replace(" ", "").Trim(); // Remove all spaces and trim
            _senderName = configuration["Email:SenderName"] ?? "Expense Tracker";
            
            // Read frontend base URL for email links
            _frontendBaseUrl = configuration["Frontend:BaseUrl"] ?? throw new InvalidOperationException("Frontend:BaseUrl is required in configuration");
            
            // Log the password for debugging (first 4 and last 4 chars only for security)
            var passwordPreview = _senderPassword.Length > 8 
                ? $"{_senderPassword.Substring(0, 4)}...{_senderPassword.Substring(_senderPassword.Length - 4)}"
                : "****";
            
            System.Console.WriteLine($"[Email] Configuration loaded:");
            System.Console.WriteLine($"[Email]   SMTP Host: {_smtpHost}:{_smtpPort}");
            System.Console.WriteLine($"[Email]   Sender: {_senderEmail}");
            System.Console.WriteLine($"[Email]   Sender Name: {_senderName}");
            System.Console.WriteLine($"[Email]   App Password: {passwordPreview} (length: {_senderPassword.Length})");
            System.Console.WriteLine($"[Email]   Frontend Base URL: {_frontendBaseUrl}");
            
            // Validate password length (should be 16 characters for Gmail App Password)
            if (_senderPassword.Length != 16 && !string.IsNullOrEmpty(rawPassword))
            {
                System.Console.WriteLine($"[Email WARNING] App Password length is {_senderPassword.Length}, expected 16 characters!");
                System.Console.WriteLine($"[Email WARNING] Please verify you copied the ENTIRE App Password from Google.");
            }
        }

        public async Task SendWelcomeEmailAsync(User user)
        {
            var subject = "Welcome to Expense Tracker!";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #4CAF50;'>Welcome to Expense Tracker!</h2>
                    <p>Hello {user.FullName ?? user.Email},</p>
                    <p>Thank you for registering with Expense Tracker. Your account has been successfully created.</p>
                    <p>You can now start tracking your expenses and managing your budget.</p>
                    <br/>
                    <p>Best regards,<br/>Expense Tracker Team</p>
                </body>
                </html>";

            await SendEmailAsync(user.Email, subject, body);
        }

        public async Task SendPasswordResetAsync(User user, string token)
        {
            var subject = "Password Reset Request";
            var resetLink = $"{_frontendBaseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; padding: 20px;'>
                    <h2 style='color: #2196F3;'>Password Reset Request</h2>
                    <p>Hello {user.FullName ?? user.Email},</p>
                    <p>You have requested to reset your password for your Expense Tracker account.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{resetLink}' style='background-color: #2196F3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Reset Password</a></p>
                    <p>Or copy and paste this link into your browser:</p>
                    <p style='word-break: break-all; color: #666;'>{resetLink}</p>
                    <p>This link will expire in 1 hour for security reasons.</p>
                    <br/>
                    <p>If you did not request this password reset, please ignore this email.</p>
                    <p>Best regards,<br/>Expense Tracker Team</p>
                </body>
                </html>";

            await SendEmailAsync(user.Email, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Use MailKit for better Gmail SMTP support
            await SendEmailWithMailKitAsync(toEmail, subject, body);
        }

        private async Task SendEmailWithMailKitAsync(string toEmail, string subject, string body)
        {
            // Diagnostic: Show password info
            var passwordPreview = _senderPassword.Length > 8 
                ? $"{_senderPassword.Substring(0, 4)}...{_senderPassword.Substring(_senderPassword.Length - 4)}"
                : "****";
            
            System.Console.WriteLine($"[Email] Attempting to send email using MailKit");
            System.Console.WriteLine($"[Email] To: {toEmail}");
            System.Console.WriteLine($"[Email] Sender: {_senderEmail}");
            System.Console.WriteLine($"[Email] Password length: {_senderPassword.Length} characters");
            System.Console.WriteLine($"[Email] Password preview: {passwordPreview}");
            System.Console.WriteLine($"[Email] Password (no spaces): {_senderPassword}");
            
            try
            {
                // Ensure password is clean (no spaces)
                var cleanPassword = _senderPassword.Replace(" ", "").Trim();
                System.Console.WriteLine($"[Email] Cleaned password: {cleanPassword} (length: {cleanPassword.Length})");
                
                // Create MIME message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_senderName, _senderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;
                
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                // Connect and authenticate using MailKit
                System.Console.WriteLine($"[Email] Connecting to {_smtpHost}:587 using MailKit...");
                System.Console.WriteLine($"[Email] Using password: {cleanPassword}");
                System.Console.WriteLine($"[Email] Password characters: {cleanPassword.Length}");
                
                using (var client = new SmtpClient())
                {
                    // Set timeout
                    client.Timeout = 30000;
                    
                    try
                    {
                        // Connect with STARTTLS (port 587) - Auto will negotiate the best option
                        System.Console.WriteLine($"[Email] Step 1: Connecting to SMTP server...");
                        await client.ConnectAsync(_smtpHost, 587, SecureSocketOptions.StartTls);
                        System.Console.WriteLine($"[Email] Step 2: Connected successfully. Server: {client.Capabilities}");
                        
                        // Authenticate with username and password
                        System.Console.WriteLine($"[Email] Step 3: Authenticating with email: {_senderEmail}");
                        System.Console.WriteLine($"[Email] Step 3: Using password length: {cleanPassword.Length}");
                        await client.AuthenticateAsync(_senderEmail, cleanPassword);
                        System.Console.WriteLine($"[Email] Step 4: Authentication successful!");
                        
                        // Send the email
                        System.Console.WriteLine($"[Email] Step 5: Sending email...");
                        await client.SendAsync(message);
                        System.Console.WriteLine($"[Email] ✓ Successfully sent email to {toEmail}");
                        
                        // Disconnect
                        await client.DisconnectAsync(true);
                        System.Console.WriteLine($"[Email] Disconnected from server.");
                    }
                    catch (Exception connectEx)
                    {
                        System.Console.WriteLine($"[Email] Error during connection/auth: {connectEx.GetType().Name} - {connectEx.Message}");
                        if (client.IsConnected)
                        {
                            await client.DisconnectAsync(true);
                        }
                        throw;
                    }
                }
            }
            catch (MailKit.Security.AuthenticationException authEx)
            {
                System.Console.WriteLine($"[Email Error] ==========================================");
                System.Console.WriteLine($"[Email Error] AUTHENTICATION FAILED (MailKit)!");
                System.Console.WriteLine($"[Email Error] Error: {authEx.Message}");
                System.Console.WriteLine($"[Email Error] Exception Type: {authEx.GetType().FullName}");
                System.Console.WriteLine($"[Email Error] ");
                System.Console.WriteLine($"[Email Error] Password being used: {_senderPassword}");
                System.Console.WriteLine($"[Email Error] Cleaned password: {_senderPassword.Replace(" ", "").Trim()}");
                System.Console.WriteLine($"[Email Error] Password length: {_senderPassword.Length}");
                System.Console.WriteLine($"[Email Error] ");
                System.Console.WriteLine($"[Email Error] This usually means:");
                System.Console.WriteLine($"[Email Error] 1. App Password is INCORRECT - verify it matches exactly");
                System.Console.WriteLine($"[Email Error] 2. 2-Step Verification is NOT enabled");
                System.Console.WriteLine($"[Email Error] 3. App Password was revoked or expired");
                System.Console.WriteLine($"[Email Error] ");
                System.Console.WriteLine($"[Email Error] Verify your App Password at:");
                System.Console.WriteLine($"[Email Error] https://myaccount.google.com/apppasswords");
                System.Console.WriteLine($"[Email Error] ==========================================");
                throw;
            }
            catch (MailKit.Net.Smtp.SmtpCommandException smtpCmdEx)
            {
                System.Console.WriteLine($"[Email Error] ==========================================");
                System.Console.WriteLine($"[Email Error] SMTP COMMAND ERROR (MailKit)!");
                System.Console.WriteLine($"[Email Error] Status Code: {smtpCmdEx.StatusCode}");
                System.Console.WriteLine($"[Email Error] Error: {smtpCmdEx.Message}");
                System.Console.WriteLine($"[Email Error] Exception Type: {smtpCmdEx.GetType().FullName}");
                System.Console.WriteLine($"[Email Error] ");
                System.Console.WriteLine($"[Email Error] Password being used: {_senderPassword}");
                System.Console.WriteLine($"[Email Error] Cleaned password: {_senderPassword.Replace(" ", "").Trim()}");
                System.Console.WriteLine($"[Email Error] ==========================================");
                throw;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[Email Error] ==========================================");
                System.Console.WriteLine($"[Email Error] UNEXPECTED ERROR!");
                System.Console.WriteLine($"[Email Error] Exception Type: {ex.GetType().FullName}");
                System.Console.WriteLine($"[Email Error] Error: {ex.Message}");
                System.Console.WriteLine($"[Email Error] Stack Trace: {ex.StackTrace}");
                System.Console.WriteLine($"[Email Error] ");
                System.Console.WriteLine($"[Email Error] Password being used: {_senderPassword}");
                System.Console.WriteLine($"[Email Error] Cleaned password: {_senderPassword.Replace(" ", "").Trim()}");
                
                if (ex.Message.Contains("Authentication") || ex.Message.Contains("5.7.0") || ex.Message.Contains("secure connection"))
                {
                    System.Console.WriteLine($"[Email Error] ");
                    System.Console.WriteLine($"[Email Error] AUTHENTICATION ISSUE DETECTED!");
                    System.Console.WriteLine($"[Email Error] Verify your App Password matches exactly!");
                    System.Console.WriteLine($"[Email Error] Generate new App Password at:");
                    System.Console.WriteLine($"[Email Error] https://myaccount.google.com/apppasswords");
                }
                System.Console.WriteLine($"[Email Error] ==========================================");
                throw;
            }
        }
    }
}
