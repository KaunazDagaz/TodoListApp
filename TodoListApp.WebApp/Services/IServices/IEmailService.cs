namespace TodoListApp.WebApp.Services.IServices
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}

