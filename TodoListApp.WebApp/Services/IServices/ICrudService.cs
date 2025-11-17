namespace TodoListApp.WebApp.Services.IServices
{
    public interface ICrudService<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        System.Threading.Tasks.Task AddAsync(T entity);
        System.Threading.Tasks.Task UpdateAsync(T entity);
        System.Threading.Tasks.Task DeleteAsync(int id);

        //I added this methods for everything that have user (lists mostly)
        Task<List<T>> GetAllAsync(Guid ownerId);
        Task<T?> GetByIdAsync(int id, Guid ownerId);
        System.Threading.Tasks.Task AddAsync(T entity, Guid ownerId);
        System.Threading.Tasks.Task UpdateAsync(T entity, Guid ownerId);
        System.Threading.Tasks.Task DeleteAsync(int id, Guid ownerId);
    }
}
