using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models
{
    public class ToDoList
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }
    }
}
