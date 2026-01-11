using System.Collections.Generic;

namespace TodoListApp.WebApp.ViewModels
{
    public class CommentSectionViewModel
    {
        public int TaskId { get; set; }
        public int ListId { get; set; }
        public string Origin { get; set; } = "details";
        public bool CanComment { get; set; }
        public List<CommentViewModel> Comments { get; set; } = new();
    }
}
