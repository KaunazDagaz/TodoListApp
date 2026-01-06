namespace TodoListApp.WebApp.Models
{
    public class TaskTag
    {
        public int TaskId { get; set; }
        public Task? Task { get; set; }

        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
