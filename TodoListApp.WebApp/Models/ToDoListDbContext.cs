using Microsoft.EntityFrameworkCore;

namespace TodoListApp.WebApp.Models
{
    public class ToDoListDbContext : DbContext
    {
        public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options)
            : base(options)
        {
        }

        public DbSet<ToDoList> ToDoLists { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ToDoList>()
                .HasOne(t => t.Owner)
                .WithMany()
                .HasForeignKey(t => t.OwnerId)
                .IsRequired();

            modelBuilder.Entity<Task>()
                .HasOne(t => t.Owner)
                .WithMany()
                .HasForeignKey(t => t.OwnerId)
                .IsRequired();
            modelBuilder.Entity<Task>()
                .HasOne(t => t.Assignee)
                .WithMany()
                .HasForeignKey(t => t.AssigneeId)
                .IsRequired();
            modelBuilder.Entity<Task>()
                .HasOne(t => t.ToDoList)
                .WithMany(l => l.Tasks)
                .HasForeignKey(t => t.ToDoListId)
                .IsRequired();
        }
    }
}
