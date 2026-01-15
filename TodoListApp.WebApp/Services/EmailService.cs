using SendGrid;
using SendGrid.Helpers.Mail;
using TodoListApp.WebApp.Services.IServices;

namespace TodoListApp.WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;
        private readonly IConfiguration configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            string? apiKey = configuration["SendGrid:ApiKey"];
            string? fromEmail = configuration["SendGrid:FromEmail"];
            string? fromName = configuration["SendGrid:FromName"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("SendGrid:ApiKey is not configured. See SendGrid setup guidance.");
            }

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException("SendGrid:FromEmail is not configured.");
            }

            fromName ??= "TodoList App";

            SendGridClient client = new(apiKey);
            EmailAddress from = new(fromEmail, fromName);
            EmailAddress to = new(toEmail);
            const string subject = "Reset your TodoList password";

            string plainTextContent = $"You requested to reset your password.\n\n" +
                                      $"Please use the following link to set a new password:\n{resetLink}\n\n" +
                                      "If you did not request this, you can safely ignore this email.";

            string htmlContent =
                $"<p>You requested to reset your password.</p>" +
                $"<p><a href=\"{resetLink}\">Click here to set a new password</a></p>" +
                $"<p>If the button does not work, copy and paste this link into your browser:</p>" +
                $"<p><code>{System.Net.WebUtility.HtmlEncode(resetLink)}</code></p>" +
                "<p>If you did not request this, you can safely ignore this email.</p>";

            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            msg.SetClickTracking(false, false);

            Response response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Password reset email to {Email} queued successfully via SendGrid.", toEmail);
            }
            else
            {
                string body = await response.Body.ReadAsStringAsync();
                logger.LogError("Failed to send password reset email to {Email}. Status: {StatusCode}, Body: {Body}", toEmail, response.StatusCode, body);
            }
        }
    }
}

