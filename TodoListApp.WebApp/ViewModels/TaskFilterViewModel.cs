namespace TodoListApp.WebApp.ViewModels
{
    public class TaskFilterViewModel
    {
        public string? StatusFilter { get; set; } = "active";
        public string? SortBy { get; set; } = "due";
        public string? Direction { get; set; } = "asc";
        public string? TitleQuery { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
