using System;

namespace TodoListApp.WebApp.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerDisplay { get; set; } = string.Empty;
        public bool IsMine { get; set; }
    }
}
