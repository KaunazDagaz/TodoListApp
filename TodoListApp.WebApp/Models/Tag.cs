using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }

        public ICollection<TaskTag>? TaskTags { get; set; }
    }
}
