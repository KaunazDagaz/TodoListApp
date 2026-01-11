using System.Threading.Tasks;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services.IServices
{
    public interface ICommentService : ICrudService<Comment>
    {
        Task<List<Comment>> GetForTaskAsync(int taskId, Guid requesterId);
        Task<bool> AddToTaskAsync(int taskId, string text, Guid userId);
        Task<Comment?> GetForEditAsync(int id, Guid userId);
        Task<bool> UpdateTextAsync(int id, string text, Guid userId);
        Task<bool> DeleteOwnedAsync(int id, Guid userId);
    }
}
