using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services.IServices;
using TodoListApp.WebApp.ViewModels;

namespace TodoListApp.WebApp.Controllers
{
    public class ToDoListController : Controller
    {
        private readonly ICrudService<ToDoList> service;
        private readonly IMapper mapper;

        public ToDoListController(ICrudService<ToDoList> service, IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var lists = await service.GetAllAsync();
            var listViewModels = mapper.Map<IEnumerable<ToDoListViewModel>>(lists);
            return View(listViewModels);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ToDoListViewModel listViewModel)
        {
            if (!ModelState.IsValid)
                return View(listViewModel);

            var list = mapper.Map<ToDoList>(listViewModel);
            await service.AddAsync(list);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var list = await service.GetByIdAsync(id);
            if (list == null)
                return NotFound();

            var listViewModel = mapper.Map<ToDoListViewModel>(list);
            return View(listViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ToDoListViewModel listViewModel)
        {
            if (id != listViewModel.Id || !ModelState.IsValid)
                return View(listViewModel);

            var list = mapper.Map<ToDoList>(listViewModel);
            await service.UpdateAsync(list);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var list = await service.GetByIdAsync(id);
            if (list == null)
                return NotFound();

            var listViewModel = mapper.Map<ToDoListViewModel>(list);
            return View(listViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
