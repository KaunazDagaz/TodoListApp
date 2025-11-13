using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services
{
    public class TodoListService : CrudService<ToDoList>
    {
        public TodoListService(ToDoListDbContext context)
            : base(context)
        {
        }
    }
}
