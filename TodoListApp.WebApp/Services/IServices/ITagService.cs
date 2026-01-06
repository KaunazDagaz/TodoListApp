using TodoListApp.WebApp.Models;
using TaskModel = TodoListApp.WebApp.Models.Task;

namespace TodoListApp.WebApp.Services.IServices
{
    public interface ITagService : ICrudService<Tag>
    {
        Task<Tag?> GetByNameAsync(string name, Guid ownerId);
        Task<bool> AddTagToTaskAsync(int taskId, string tagName, Guid ownerId);
        Task<bool> RemoveTagFromTaskAsync(int taskId, int tagId, Guid ownerId);
        Task<List<TagUsage>> GetTagUsageForListAsync(int listId, Guid ownerId);
        Task<List<TaskModel>> GetTasksByTagForListAsync(int tagId, int listId, Guid ownerId);
        Task<List<TaskModel>> GetAssignedTasksByTagAsync(Guid assigneeId, int tagId);
    }
}
