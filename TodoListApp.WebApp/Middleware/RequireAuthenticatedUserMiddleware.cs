using TodoListApp.WebApp.Services.IServices;

namespace TodoListApp.WebApp.Middleware
{
    public class RequireAuthenticatedUserMiddleware
    {
        private readonly RequestDelegate next;

        public RequireAuthenticatedUserMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value ?? string.Empty;

            if (IsAllowedPath(path))
            {
                await next(context);
                return;
            }

            IUserService userService = context.RequestServices.GetRequiredService<IUserService>();
            Guid userId = userService.GetCurrentUserId();

            if (userId == Guid.Empty)
            {
                context.Response.Redirect("/Account/Login");
                return;
            }

            await next(context);
        }

        private static bool IsAllowedPath(string path)
        {
            path = path.ToLowerInvariant();

            return path.StartsWith("/css/") ||
                path.StartsWith("/js/") ||
                path.StartsWith("/lib/") ||
                path.StartsWith("/favicon") ||
                path.EndsWith(".ico") ||
                path.EndsWith(".png") ||
                path.EndsWith(".jpg") ||
                path.EndsWith(".jpeg") ||
                path.EndsWith(".svg") ||
                path.EndsWith(".map") ||
                path.EndsWith(".css") ||
                path.EndsWith(".js") || path.StartsWith("/account/login") ||
                path.StartsWith("/account/register") ||
                path.StartsWith("/account/restorepassword") ||
                path.StartsWith("/account/sendrestorepasswordemail") ||
                path.StartsWith("/account/resetpassword");
        }
    }
}

