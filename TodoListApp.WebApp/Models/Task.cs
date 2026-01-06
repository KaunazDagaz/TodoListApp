using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }

        public Guid AssigneeId { get; set; }
        public User? Assignee { get; set; }

        public int ToDoListId { get; set; }
        public ToDoList? ToDoList { get; set; }

        public ICollection<TaskTag>? TaskTags { get; set; }
    }

    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed
    }
}
