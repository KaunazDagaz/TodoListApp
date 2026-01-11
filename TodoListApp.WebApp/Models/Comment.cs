using System;
using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(2000)]
        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int TaskId { get; set; }
        public Task? Task { get; set; }

        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }
    }
}
