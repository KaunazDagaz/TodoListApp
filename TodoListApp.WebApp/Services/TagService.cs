using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services.IServices;
using TaskModel = TodoListApp.WebApp.Models.Task;

namespace TodoListApp.WebApp.Services
{
    public class TagService : CrudService<Tag>, ITagService
    {
        public TagService(ToDoListDbContext context) : base(context)
        {
        }

        public async Task<Tag?> GetByNameAsync(string name, Guid ownerId)
        {
            string normalized = name.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return null;
            }

            return await context.Tags.FirstOrDefaultAsync(t => t.OwnerId == ownerId && t.Name == normalized);
        }

        private static string NormalizeTagName(string tagName) => tagName.Trim();

        public async Task<bool> AddTagToTaskAsync(int taskId, string tagName, Guid ownerId)
        {
            string normalized = NormalizeTagName(tagName);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            TaskModel? task = await context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.OwnerId == ownerId);

            if (task == null)
            {
                return false;
            }

            Tag? tag = await context.Tags.FirstOrDefaultAsync(t => t.OwnerId == ownerId && t.Name == normalized);
            if (tag == null)
            {
                tag = new Tag
                {
                    Name = normalized,
                    OwnerId = ownerId
                };

                context.Tags.Add(tag);
                await context.SaveChangesAsync();
            }

            bool alreadyLinked = task.TaskTags?.Any(tt => tt.TagId == tag.Id) ?? false;
            if (alreadyLinked)
            {
                return true;
            }

            TaskTag link = new TaskTag
            {
                TaskId = task.Id,
                TagId = tag.Id
            };

            context.TaskTags.Add(link);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTagFromTaskAsync(int taskId, int tagId, Guid ownerId)
        {
            TaskModel? task = await context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.OwnerId == ownerId);

            if (task == null)
            {
                return false;
            }

            TaskTag? link = task.TaskTags?.FirstOrDefault(tt => tt.TagId == tagId);
            if (link == null)
            {
                return false;
            }

            Tag? tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == tagId && t.OwnerId == ownerId);
            if (tag == null)
            {
                return false;
            }

            context.TaskTags.Remove(link);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TagUsage>> GetTagUsageForListAsync(int listId, Guid ownerId)
        {
            List<TagUsage> usages = await context.TaskTags
                .Include(tt => tt.Tag)
                .Include(tt => tt.Task)
                .Where(tt => tt.Tag != null &&
                             tt.Task != null &&
                             tt.Tag.OwnerId == ownerId &&
                             tt.Task.OwnerId == ownerId &&
                             tt.Task.ToDoListId == listId)
                .GroupBy(tt => tt.Tag!)
                .Select(g => new TagUsage
                {
                    Tag = g.Key,
                    TaskCount = g.Select(tt => tt.TaskId).Distinct().Count()
                })
                .OrderBy(tu => tu.Tag.Name)
                .ToListAsync();

            return usages;
        }

        public async Task<List<TaskModel>> GetTasksByTagForListAsync(int tagId, int listId, Guid ownerId)
        {
            return await context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.ToDoList)
                .Include(t => t.TaskTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => t.OwnerId == ownerId &&
                            t.ToDoListId == listId &&
                            t.TaskTags.Any(tt => tt.TagId == tagId))
                .ToListAsync();
        }

        public async Task<List<TaskModel>> GetAssignedTasksByTagAsync(Guid assigneeId, int tagId)
        {
            return await context.Tasks
                .Include(t => t.ToDoList)
                .Include(t => t.TaskTags)
                    .ThenInclude(tt => tt.Tag)
                .Where(t => t.AssigneeId == assigneeId &&
                            t.TaskTags.Any(tt => tt.TagId == tagId))
                .ToListAsync();
        }
    }
}
