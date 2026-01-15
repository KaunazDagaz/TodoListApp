using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.ViewModels
{
    public class RestorePasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

