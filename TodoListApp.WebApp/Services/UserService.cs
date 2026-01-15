using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services.IServices;

namespace TodoListApp.WebApp.Services
{
    public class UserService : IUserService
    {
        private readonly ToDoListDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly PasswordHasher<User> passwordHasher = new();
        private const string AuthCookieName = "UserId";

        public UserService(ToDoListDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentUserId()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return Guid.Empty;
            }

            if (httpContext.Items["AppUserId"] is Guid idFromItems)
            {
                return idFromItems;
            }

            var cookie = httpContext.Request.Cookies[AuthCookieName];
            return Guid.TryParse(cookie, out var idFromCookie) ? idFromCookie : Guid.Empty;
        }

        public async System.Threading.Tasks.Task<User?> GetByIdAsync(Guid id)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async System.Threading.Tasks.Task<User?> GetByEmailAsync(string email)
        {
            var normalized = email.Trim().ToLowerInvariant();
            return await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalized);
        }

        public async System.Threading.Tasks.Task<User?> RegisterAsync(string email, string password, string? displayName = null)
        {
            var normalized = email.Trim().ToLowerInvariant();

            if (await context.Users.AnyAsync(u => u.Email.ToLower() == normalized))
            {
                return null;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = normalized,
                DisplayName = displayName,
                CreatedAt = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow
            };

            user.PasswordHash = passwordHasher.HashPassword(user, password);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async System.Threading.Tasks.Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Failed ? null : user;
        }

        public async System.Threading.Tasks.Task SetAuthCookieAsync(User user)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            httpContext.Response.Cookies.Append(AuthCookieName, user.Id.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                Secure = httpContext.Request.IsHttps,
                SameSite = SameSiteMode.Lax
            });

            httpContext.Items["AppUserId"] = user.Id;
        }

        public async System.Threading.Tasks.Task ClearAuthCookieAsync()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            httpContext.Response.Cookies.Delete(AuthCookieName);
            await System.Threading.Tasks.Task.CompletedTask;
        }

        public async System.Threading.Tasks.Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", string.Empty)
                .Replace("/", string.Empty)
                .Replace("=", string.Empty);

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return token;
        }

        public async System.Threading.Tasks.Task<bool> IsValidPasswordResetTokenAsync(Guid userId, string token)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (user.PasswordResetToken == null || user.PasswordResetTokenExpiresAt == null)
            {
                return false;
            }

            if (!string.Equals(user.PasswordResetToken, token, StringComparison.Ordinal) ||
                user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        public async System.Threading.Tasks.Task<bool> ResetPasswordAsync(Guid userId, string token, string newPassword)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (!await IsValidPasswordResetTokenAsync(userId, token))
            {
                return false;
            }

            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiresAt = null;

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return true;
        }
    }
}

