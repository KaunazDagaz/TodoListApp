using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.ViewModels
{
    public class CommentInputModel
    {
        [Required]
        [StringLength(2000)]
        public string Text { get; set; } = string.Empty;

        public int TaskId { get; set; }

        public int ListId { get; set; }

        public string Origin { get; set; } = "details";
    }
}
