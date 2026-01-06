using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.ViewModels
{
    public class TagViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public int TaskCount { get; set; }
        public int ToDoListId { get; set; }
        public string? ToDoListTitle { get; set; }
    }
}
