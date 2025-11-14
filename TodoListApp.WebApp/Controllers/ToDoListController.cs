// ...existing code...
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        private Guid GetCurrentUserId()
        {
            if (HttpContext?.Items["AppUserId"] is Guid idFromItems)
                return idFromItems;

            var cookie = Request.Cookies["AppUserId"];
            if (Guid.TryParse(cookie, out var idFromCookie))
                return idFromCookie;

            return Guid.Empty;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return View(new List<ToDoListViewModel>());

            var lists = await service.GetAllAsync(userId);
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
            await service.AddAsync(list, GetCurrentUserId());

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var list = await service.GetByIdAsync(id, GetCurrentUserId());
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
            await service.UpdateAsync(list, GetCurrentUserId());

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var list = await service.GetByIdAsync(id, GetCurrentUserId());
            if (list == null)
                return NotFound();

            var listViewModel = mapper.Map<ToDoListViewModel>(list);
            return View(listViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await service.DeleteAsync(id, GetCurrentUserId());
            return RedirectToAction(nameof(Index));
        }
    }
}