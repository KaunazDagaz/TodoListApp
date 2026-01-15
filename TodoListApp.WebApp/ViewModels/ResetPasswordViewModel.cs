using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

