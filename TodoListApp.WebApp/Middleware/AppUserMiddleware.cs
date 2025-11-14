using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Middleware
{
    public class AppUserMiddleware
    {
        private readonly RequestDelegate next;

        public AppUserMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            const string cookieName = "AppUserId";
            var cookieVal = context.Request.Cookies[cookieName];
            if (!Guid.TryParse(cookieVal, out var userId))
            {
                userId = Guid.NewGuid();
                context.Response.Cookies.Append(cookieName, userId.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = true,
                    Secure = context.Request.IsHttps,
                    SameSite = SameSiteMode.Lax
                });
            }

            var db = context.RequestServices.GetRequiredService<ToDoListDbContext>();
            var user = await db.Users.FindAsync(userId);
            var now = DateTime.UtcNow;
            var ua = context.Request.Headers["User-Agent"].ToString();

            if (user == null)
            {
                user = new User
                {
                    Id = userId,
                    CreatedAt = now,
                    UserAgent = ua,
                    LastSeen = now
                };
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
            }
            else
            {
                user.LastSeen = now;
                user.UserAgent = ua;
                db.Users.Update(user);
                await db.SaveChangesAsync();
            }

            context.Items["AppUserId"] = userId;
            await next(context);
        }
    }
}
