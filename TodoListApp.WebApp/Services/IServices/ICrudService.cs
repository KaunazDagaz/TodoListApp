namespace TodoListApp.WebApp.Services.IServices
{
    public interface ICrudService<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

        //I added this methods for everything that have user (lists mostly)
        Task<List<T>> GetAllAsync(Guid ownerId);
        Task<T?> GetByIdAsync(int id, Guid ownerId);
        Task AddAsync(T entity, Guid ownerId);
        Task UpdateAsync(T entity, Guid ownerId);
        Task DeleteAsync(int id, Guid ownerId);
    }
}
