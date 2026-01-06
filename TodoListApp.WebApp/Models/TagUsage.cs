namespace TodoListApp.WebApp.Models
{
    public class TagUsage
    {
        public Tag Tag { get; set; } = new Tag();
        public int TaskCount { get; set; }
    }
}
