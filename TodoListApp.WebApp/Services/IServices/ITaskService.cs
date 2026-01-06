using TodoListApp.WebApp.Models;
using TaskModel = TodoListApp.WebApp.Models.Task;

namespace TodoListApp.WebApp.Services.IServices
{
    public interface ITaskService : ICrudService<TaskModel>
    {
        Task<List<TaskModel>> GetByListIdAsync(int listId, Guid ownerId);
        Task<TaskModel?> GetDetailedByIdAsync(int id, Guid ownerId);
        Task<ToDoList?> GetListForUserAsync(int listId, Guid ownerId);
        Task<TaskModel?> GetAssignedTaskAsync(int id, Guid assigneeId);
        Task<List<TaskModel>> GetAssignedToAsync(
            Guid assigneeId, 
            IEnumerable<Models.TaskStatus>? statuses = null, 
            string sortBy = "due", 
            bool ascending = true,
            string? titleQuery = null,
            DateTime? createdDate = null,
            DateTime? dueDate = null);
        Task<bool> UpdateStatusAsAssigneeAsync(int id, Guid assigneeId, Models.TaskStatus status);
    }
}
