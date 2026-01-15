using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Middleware
{
    public class AppUserMiddleware
    {
        private readonly RequestDelegate next;
        private const string CookieName = "UserId";

        public AppUserMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
        {
            string? cookieVal = context.Request.Cookies[CookieName];
            if (!Guid.TryParse(cookieVal, out Guid userId))
            {
                await next(context);
                return;
            }

            ToDoListDbContext db = context.RequestServices.GetRequiredService<ToDoListDbContext>();
            User? user = await db.Users.FindAsync(userId);
            if (user != null)
            {
                DateTime now = DateTime.UtcNow;
                string ua = context.Request.Headers["User-Agent"].ToString();

                user.LastSeen = now;
                user.UserAgent = ua;
                db.Users.Update(user);
                await db.SaveChangesAsync();

                context.Items["AppUserId"] = userId;
            }
            else
            {
                context.Response.Cookies.Delete(CookieName);
            }

            await next(context);
        }
    }
}
