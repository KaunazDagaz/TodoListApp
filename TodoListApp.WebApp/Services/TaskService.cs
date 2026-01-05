using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services.IServices;
using TaskModel = TodoListApp.WebApp.Models.Task;

namespace TodoListApp.WebApp.Services
{
    public class TaskService : CrudService<TaskModel>, ITaskService
    {
        private readonly ICrudService<ToDoList> listService;

        public TaskService(ToDoListDbContext context, ICrudService<ToDoList> listService) : base(context)
        {
            this.listService = listService;
        }

        private static DateTime EnsureUtc(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private static DateTime? EnsureUtc(DateTime? value)
        {
            return value.HasValue ? EnsureUtc(value.Value) : null;
        }

        public async Task<List<TaskModel>> GetByListIdAsync(int listId, Guid ownerId)
        {
            List<TaskModel> tasks = await base.GetAllAsync(ownerId);
            return tasks.Where(t => t.ToDoListId == listId).ToList();
        }

        public async Task<TaskModel?> GetDetailedByIdAsync(int id, Guid ownerId)
        {
            TaskModel? task = await base.GetByIdAsync(id, ownerId);
            return task == null
                ? null
                : await context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.ToDoList)
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == ownerId);
        }

        public override async Task<TaskModel?> GetByIdAsync(int id, Guid ownerId)
        {
            return await GetDetailedByIdAsync(id, ownerId);
        }

        public async Task<ToDoList?> GetListForUserAsync(int listId, Guid ownerId)
        {
            return await listService.GetByIdAsync(listId, ownerId);
        }

        public async Task<List<TaskModel>> GetAssignedToAsync(Guid assigneeId)
        {
            return await context.Tasks
                .Include(t => t.ToDoList)
                .Where(t => t.AssigneeId == assigneeId)
                .ToListAsync();
        }

        public async Task<TaskModel?> GetAssignedTaskAsync(int id, Guid assigneeId)
        {
            return await context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.ToDoList)
                .FirstOrDefaultAsync(t => t.Id == id && t.AssigneeId == assigneeId);
        }

        public override async System.Threading.Tasks.Task AddAsync(TaskModel entity, Guid ownerId)
        {
            entity.CreatedDate = EnsureUtc(entity.CreatedDate == default ? DateTime.UtcNow : entity.CreatedDate);
            entity.DueDate = EnsureUtc(entity.DueDate);
            await base.AddAsync(entity, ownerId);
        }

        public override async System.Threading.Tasks.Task UpdateAsync(TaskModel entity, Guid ownerId)
        {
            entity.CreatedDate = EnsureUtc(entity.CreatedDate);
            entity.DueDate = EnsureUtc(entity.DueDate);
            await base.UpdateAsync(entity, ownerId);
        }
    }
}
