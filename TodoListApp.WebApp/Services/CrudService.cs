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

        protected static void EnsureHasGuidOwnerProperty()
        {
            var prop = typeof(T).GetProperty("OwnerId");
            if (prop == null || prop.PropertyType != typeof(Guid))
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have an OwnerId property of type Guid.");
        }

        public virtual async Task<List<T>> GetAllAsync(Guid ownerId)
        {
            EnsureHasGuidOwnerProperty();
            return await DbSet.Where(e => EF.Property<Guid>(e, "OwnerId") == ownerId).ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id, Guid ownerId)
        {
            EnsureHasGuidOwnerProperty();
            return await DbSet
                .Where(e => EF.Property<int>(e, "Id") == id && EF.Property<Guid>(e, "OwnerId") == ownerId)
                .FirstOrDefaultAsync();
        }

        public virtual async Task AddAsync(T entity, Guid ownerId)
        {
            EnsureHasGuidOwnerProperty();
            var ownerProp = typeof(T).GetProperty("OwnerId")!;
            ownerProp.SetValue(entity, ownerId);
            DbSet.Add(entity);
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity, Guid ownerId)
        {
            EnsureHasGuidOwnerProperty();

            var idProp = typeof(T).GetProperty("Id");
            if (idProp == null || idProp.PropertyType != typeof(int))
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have an Id property of type int.");

            var idValue = (int)idProp.GetValue(entity)!;
            var existing = await GetByIdAsync(idValue, ownerId);
            if (existing == null) return;

            context.Entry(existing).CurrentValues.SetValues(entity);
            context.Entry(existing).Property("OwnerId").CurrentValue = ownerId;

            await context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id, Guid ownerId)
        {
            EnsureHasGuidOwnerProperty();
            var entity = await GetByIdAsync(id, ownerId);
            if (entity == null) return;

            DbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
