namespace TodoListApp.WebApp.Models
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
        public Guid AssigneeId { get; set; }
        public User? Assignee { get; set; }
        public int ToDoListId { get; set; }
        public ToDoList? ToDoList { get; set; }
    }

    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed
    }
}
