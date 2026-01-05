using TodoListApp.WebApp.Models;
using TaskModel = TodoListApp.WebApp.Models.Task;

namespace TodoListApp.WebApp.Services.IServices
{
    public interface ITaskService : ICrudService<TaskModel>
    {
        Task<List<TaskModel>> GetByListIdAsync(int listId, Guid ownerId);
        Task<TaskModel?> GetDetailedByIdAsync(int id, Guid ownerId);
        Task<ToDoList?> GetListForUserAsync(int listId, Guid ownerId);
        Task<List<TaskModel>> GetAssignedToAsync(Guid assigneeId);
        Task<TaskModel?> GetAssignedTaskAsync(int id, Guid assigneeId);
    }
}
