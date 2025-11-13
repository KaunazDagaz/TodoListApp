using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services.IServices;

namespace TodoListApp.WebApp.Controllers
{
    public class ToDoListController : Controller
    {
        private readonly ICrudService<ToDoList> service;

        public ToDoListController(ICrudService<ToDoList> service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index() => View(await service.GetAllAsync());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ToDoList list)
        {
            if (!ModelState.IsValid)
                return View(list);

            await service.AddAsync(list);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var list = await service.GetByIdAsync(id);
            return list == null ? NotFound() : View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ToDoList list)
        {
            if (id != list.Id || !ModelState.IsValid)
                return View(list);

            await service.UpdateAsync(list);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var list = await service.GetByIdAsync(id);
            return list == null ? NotFound() : View(list);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
