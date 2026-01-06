using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services.IServices;
using TodoListApp.WebApp.ViewModels;
using TaskModel = TodoListApp.WebApp.Models.Task;

namespace TodoListApp.WebApp.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagService tagService;
        private readonly ITaskService taskService;
        private readonly IMapper mapper;

        public TagController(ITagService tagService, ITaskService taskService, IMapper mapper)
        {
            this.tagService = tagService;
            this.taskService = taskService;
            this.mapper = mapper;
        }

        private Guid GetCurrentUserId()
        {
            if (HttpContext?.Items["AppUserId"] is Guid idFromItems)
            {
                return idFromItems;
            }

            string? cookie = Request.Cookies["AppUserId"];
            return Guid.TryParse(cookie, out Guid idFromCookie) ? idFromCookie : Guid.Empty;
        }

        public async Task<IActionResult> Index(int listId)
        {
            Guid userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            List<TagUsage> usages = await tagService.GetTagUsageForListAsync(listId, userId);
            IEnumerable<TagViewModel> viewModel = usages.Select(u => new TagViewModel
            {
                Id = u.Tag.Id,
                Name = u.Tag.Name,
                TaskCount = u.TaskCount,
                ToDoListId = list.Id,
                ToDoListTitle = list.Title
            });

            ViewBag.ListId = listId;
            ViewBag.ListTitle = list.Title;

            return View(viewModel);
        }

        public async Task<IActionResult> Tasks(int listId, int tagId)
        {
            Guid userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            Tag? tag = await tagService.GetByIdAsync(tagId, userId);
            if (tag == null)
            {
                return NotFound();
            }

            List<TaskModel> tasks = await tagService.GetTasksByTagForListAsync(tagId, listId, userId);
            IEnumerable<TaskViewModel> viewModel = mapper.Map<IEnumerable<TaskViewModel>>(tasks);

            ViewBag.ListId = listId;
            ViewBag.ListTitle = list.Title;
            ViewBag.TagName = tag.Name;
            ViewBag.TagId = tagId;

            return View(viewModel);
        }

        public async Task<IActionResult> MyTasks(int tagId, int? listId)
        {
            Guid userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            Tag? tag = await tagService.GetByIdAsync(tagId);
            if (tag == null)
            {
                return NotFound();
            }

            List<TaskModel> tasks = await tagService.GetAssignedTasksByTagAsync(userId, tagId);
            if (listId.HasValue)
            {
                tasks = tasks.Where(t => t.ToDoListId == listId.Value).ToList();
            }

            IEnumerable<TaskViewModel> viewModel = mapper.Map<IEnumerable<TaskViewModel>>(tasks);

            ViewBag.TagName = tag.Name;
            ViewBag.TagId = tagId;
            ViewBag.ListId = listId;

            return View(viewModel);
        }
    }
}
