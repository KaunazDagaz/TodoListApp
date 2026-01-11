using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public required string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Date)]
        [Display(Name = "Due date")]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Status")]
        public Models.TaskStatus Status { get; set; } = Models.TaskStatus.NotStarted;

        [Display(Name = "Assignee")]
        public Guid AssigneeId { get; set; }

        public Guid OwnerId { get; set; }

        public string? AssigneeDisplay { get; set; }

        public int ToDoListId { get; set; }

        public string? ToDoListTitle { get; set; }

        public List<TagViewModel> Tags { get; set; } = new();

        public List<CommentViewModel> Comments { get; set; } = new();

        public bool CanComment { get; set; }

        public bool IsOverdue => DueDate.HasValue && DueDate.Value.Date < DateTime.UtcNow.Date && Status != Models.TaskStatus.Completed;
    }
}
