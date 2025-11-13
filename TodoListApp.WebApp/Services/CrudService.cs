using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services.IServices;

namespace TodoListApp.WebApp.Services
{
    public class CrudService<T> : ICrudService<T> where T : class
    {
        protected readonly ToDoListDbContext context;
        protected readonly DbSet<T> DbSet;

        public CrudService(ToDoListDbContext context)
        {
            this.context = context;
            DbSet = this.context.Set<T>();
        }

        public virtual async Task<List<T>> GetAllAsync() => await DbSet.ToListAsync();

        public virtual async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

        public virtual async Task AddAsync(T entity)
        {
            DbSet.Add(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            DbSet.Update(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity != null)
            {
                DbSet.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}
