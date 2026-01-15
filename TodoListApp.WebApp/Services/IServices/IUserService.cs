using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services.IServices
{
    public interface IUserService
    {
        Task<User?> RegisterAsync(string email, string password, string? displayName = null);
        Task<User?> AuthenticateAsync(string email, string password);
        Guid GetCurrentUserId();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        System.Threading.Tasks.Task SetAuthCookieAsync(User user);
        System.Threading.Tasks.Task ClearAuthCookieAsync();
        Task<string?> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(Guid userId, string token, string newPassword);
        Task<bool> IsValidPasswordResetTokenAsync(Guid userId, string token);
    }
}

