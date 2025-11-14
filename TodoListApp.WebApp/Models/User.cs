namespace TodoListApp.WebApp.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeen { get; set; }

    }
}
