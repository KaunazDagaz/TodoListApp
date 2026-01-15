using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Services.IServices;
using TodoListApp.WebApp.ViewModels;

namespace TodoListApp.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IEmailService emailService;

        public AccountController(IUserService userService, IEmailService emailService)
        {
            this.userService = userService;
            this.emailService = emailService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Models.User? user = await userService.RegisterAsync(model.Email, model.Password, model.DisplayName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User with this email already exists.");
                return View(model);
            }

            await userService.SetAuthCookieAsync(user);
            return RedirectToAction("Index", "ToDoList");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Models.User? user = await userService.AuthenticateAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            await userService.SetAuthCookieAsync(user);
            return RedirectToAction("Index", "ToDoList");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await userService.ClearAuthCookieAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult RestorePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RestorePassword(RestorePasswordViewModel model)
        {
            return !ModelState.IsValid ? View(model) : View("RestorePasswordConfirmation", model);
        }

        [HttpPost]
        public async Task<IActionResult> SendRestorePasswordEmail(RestorePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("RestorePassword", model);
            }

            string? token = await userService.GeneratePasswordResetTokenAsync(model.Email);

            if (token != null)
            {
                Models.User? user = await userService.GetByEmailAsync(model.Email);
                if (user != null)
                {
                    string resetLink = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { userId = user.Id, token },
                        Request.Scheme) ?? string.Empty;

                    await emailService.SendPasswordResetEmailAsync(model.Email, resetLink);
                }
            }

            return View("RestorePasswordEmailSent");
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(Guid userId, string token)
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(token))
            {
                return View("ResetPasswordInvalid");
            }

            bool isValid = await userService.IsValidPasswordResetTokenAsync(userId, token);
            if (!isValid)
            {
                return View("ResetPasswordInvalid");
            }

            ResetPasswordViewModel model = new()
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool success = await userService.ResetPasswordAsync(model.UserId, model.Token, model.NewPassword);
            return !success ? View("ResetPasswordInvalid") : View("ResetPasswordSuccess");
        }
    }
}

