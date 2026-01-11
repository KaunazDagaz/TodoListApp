using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services.IServices;
using TaskModel = TodoListApp.WebApp.Models.Task;

namespace TodoListApp.WebApp.Services
{
    public class CommentService : CrudService<Comment>, ICommentService
    {
        public CommentService(ToDoListDbContext context) : base(context)
        {
        }

        private async Task<TaskModel?> GetAccessibleTaskAsync(int taskId, Guid userId)
        {
            return await context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId && (t.OwnerId == userId || t.AssigneeId == userId));
        }

        public async Task<List<Comment>> GetForTaskAsync(int taskId, Guid requesterId)
        {
            TaskModel? task = await GetAccessibleTaskAsync(taskId, requesterId);
            if (task == null)
            {
                return new List<Comment>();
            }

            return await context.Comments
                .Include(c => c.Owner)
                .Where(c => c.TaskId == taskId)
                .ToListAsync();
        }

        public async Task<bool> AddToTaskAsync(int taskId, string text, Guid userId)
        {
            TaskModel? task = await GetAccessibleTaskAsync(taskId, userId);
            if (task == null)
            {
                return false;
            }

            string normalized = text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            Comment comment = new()
            {
                TaskId = taskId,
                Text = normalized,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.Comments.Add(comment);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Comment?> GetForEditAsync(int id, Guid userId)
        {
            return await context.Comments
                .Include(c => c.Task)
                .FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == userId);
        }

        public async Task<bool> UpdateTextAsync(int id, string text, Guid userId)
        {
            Comment? existing = await context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == userId);
            if (existing == null)
            {
                return false;
            }

            string normalized = text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            existing.Text = normalized;
            existing.UpdatedAt = DateTime.UtcNow;

            context.Comments.Update(existing);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOwnedAsync(int id, Guid userId)
        {
            Comment? existing = await context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == userId);
            if (existing == null)
            {
                return false;
            }

            context.Comments.Remove(existing);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
