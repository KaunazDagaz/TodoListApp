using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services.IServices;
using TodoListApp.WebApp.ViewModels;
using TaskModel = TodoListApp.WebApp.Models.Task;

namespace TodoListApp.WebApp.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly ToDoListDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ITagService tagService;
        private readonly IUserService userService;
        private readonly ICommentService commentService;

        public TaskController(
            ITaskService taskService,
            ToDoListDbContext dbContext,
            IMapper mapper,
            ITagService tagService,
            IUserService userService,
            ICommentService commentService)
        {
            this.taskService = taskService;
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.tagService = tagService;
            this.commentService = commentService;
            this.userService = userService;
        }

        private async Task<CommentSectionViewModel> BuildCommentsAsync(TaskModel task, Guid userId, string origin)
        {
            List<Comment> comments = await commentService.GetForTaskAsync(task.Id, userId);

            List<CommentViewModel> ordered = comments
                .Select(c =>
                {
                    CommentViewModel viewModel = mapper.Map<CommentViewModel>(c);
                    viewModel.IsMine = c.OwnerId == userId;
                    return viewModel;
                })
                .OrderByDescending(c => c.IsMine)
                .ThenByDescending(c => c.CreatedAt)
                .ToList();

            return new CommentSectionViewModel
            {
                TaskId = task.Id,
                ListId = task.ToDoListId,
                Origin = origin,
                CanComment = task.OwnerId == userId || task.AssigneeId == userId,
                Comments = ordered
            };
        }

        private IEnumerable<SelectListItem> BuildUserSelectList(Guid selectedUserId)
        {
            List<User> users = dbContext.Users
                .OrderByDescending(u => u.LastSeen)
                .ToList();

            return users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = string.IsNullOrWhiteSpace(u.Email) ? u.Id.ToString() : u.Email,
                Selected = u.Id == selectedUserId
            });
        }

        public async Task<IActionResult> Index(int listId)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            List<TaskModel> tasks = await taskService.GetByListIdAsync(listId, userId);
            IEnumerable<TaskViewModel> viewModel = mapper.Map<IEnumerable<TaskViewModel>>(tasks);

            ViewBag.ListId = listId;
            ViewBag.ListTitle = list.Title;

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id, int listId)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            TaskModel? task = await taskService.GetDetailedByIdAsync(id, userId);
            if (task == null || task.ToDoListId != listId)
            {
                return NotFound();
            }

            TaskViewModel viewModel = mapper.Map<TaskViewModel>(task);
            viewModel.ToDoListTitle = list.Title;
            viewModel.CanComment = task.OwnerId == userId || task.AssigneeId == userId;

            CommentSectionViewModel comments = await BuildCommentsAsync(task, userId, "details");
            viewModel.Comments = comments.Comments;
            ViewBag.CommentsSection = comments;

            return View(viewModel);
        }

        public async Task<IActionResult> Create(int listId)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            ViewBag.Users = BuildUserSelectList(userId);
            ViewBag.ListId = listId;
            ViewBag.ListTitle = list.Title;

            TaskViewModel viewModel = new()
            {
                Title = string.Empty,
                ToDoListId = listId,
                CreatedDate = DateTime.UtcNow,
                AssigneeId = userId,
                ToDoListTitle = list.Title
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int listId, TaskViewModel viewModel)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Users = BuildUserSelectList(viewModel.AssigneeId);
                ViewBag.ListId = listId;
                ViewBag.ListTitle = list.Title;
                return View(viewModel);
            }

            TaskModel task = mapper.Map<TaskModel>(viewModel);
            task.ToDoListId = listId;
            task.CreatedDate = DateTime.UtcNow;
            task.AssigneeId = viewModel.AssigneeId == Guid.Empty ? userId : viewModel.AssigneeId;

            await taskService.AddAsync(task, userId);

            return RedirectToAction(nameof(Index), new { listId });
        }

        public async Task<IActionResult> Edit(int id, int listId)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            TaskModel? task = await taskService.GetDetailedByIdAsync(id, userId);
            if (task == null || task.ToDoListId != listId)
            {
                return NotFound();
            }

            TaskViewModel viewModel = mapper.Map<TaskViewModel>(task);
            viewModel.ToDoListTitle = list.Title;
            viewModel.CanComment = task.OwnerId == userId || task.AssigneeId == userId;

            CommentSectionViewModel comments = await BuildCommentsAsync(task, userId, "edit");
            viewModel.Comments = comments.Comments;
            ViewBag.CommentsSection = comments;

            ViewBag.Users = BuildUserSelectList(viewModel.AssigneeId);
            ViewBag.ListId = listId;
            ViewBag.ListTitle = list.Title;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, int listId, TaskViewModel viewModel)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            TaskModel? existingTask = await taskService.GetDetailedByIdAsync(id, userId);
            if (existingTask == null || existingTask.ToDoListId != listId)
            {
                return NotFound();
            }

            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                viewModel.CanComment = existingTask.OwnerId == userId || existingTask.AssigneeId == userId;
                CommentSectionViewModel comments = await BuildCommentsAsync(existingTask, userId, "edit");
                viewModel.Comments = comments.Comments;
                ViewBag.CommentsSection = comments;

                ViewBag.Users = BuildUserSelectList(viewModel.AssigneeId);
                ViewBag.ListId = listId;
                ViewBag.ListTitle = list.Title;
                return View(viewModel);
            }

            TaskModel task = mapper.Map<TaskModel>(viewModel);
            task.ToDoListId = listId;
            task.AssigneeId = viewModel.AssigneeId == Guid.Empty ? userId : viewModel.AssigneeId;

            await taskService.UpdateAsync(task, userId);

            return RedirectToAction(nameof(Index), new { listId });
        }

        public async Task<IActionResult> Delete(int id, int listId)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            TaskModel? task = await taskService.GetDetailedByIdAsync(id, userId);
            if (task == null || task.ToDoListId != listId)
            {
                return NotFound();
            }

            TaskViewModel viewModel = mapper.Map<TaskViewModel>(task);
            viewModel.ToDoListTitle = list.Title;

            ViewBag.ListId = listId;
            ViewBag.ListTitle = list.Title;

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int listId)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            TaskModel? task = await taskService.GetDetailedByIdAsync(id, userId);
            if (task == null || task.ToDoListId != listId)
            {
                return NotFound();
            }

            await taskService.DeleteAsync(id, userId);
            return RedirectToAction(nameof(Index), new { listId });
        }

        public async Task<IActionResult> Assigned([FromQuery] TaskFilterViewModel filter)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            (IEnumerable<Models.TaskStatus> statuses, string appliedFilter) = BuildStatusFilter(filter.StatusFilter);
            bool ascending = string.Equals(filter.Direction, "asc", StringComparison.OrdinalIgnoreCase);

            List<TaskModel> tasks = await taskService.GetAssignedToAsync(
                userId,
                statuses,
                filter.SortBy ?? "due",
                ascending,
                filter.TitleQuery,
                filter.CreatedDate,
                filter.DueDate);

            IEnumerable<TaskViewModel> viewModel = mapper.Map<IEnumerable<TaskViewModel>>(tasks);

            ViewData["Title"] = "My Tasks";
            ViewBag.StatusFilter = appliedFilter;
            ViewBag.SortBy = filter.SortBy;
            ViewBag.Direction = ascending ? "asc" : "desc";
            ViewBag.TitleQuery = filter.TitleQuery;
            ViewBag.CreatedDate = filter.CreatedDate?.ToString("yyyy-MM-dd");
            ViewBag.DueDate = filter.DueDate?.ToString("yyyy-MM-dd");

            return View(viewModel);
        }

        private static (IEnumerable<Models.TaskStatus> statuses, string applied) BuildStatusFilter(string? statusFilter)
        {
            return (statusFilter?.ToLowerInvariant()) switch
            {
                "notstarted" => ((IEnumerable<Models.TaskStatus> statuses, string applied))(new[] { Models.TaskStatus.NotStarted }, "notstarted"),
                "inprogress" => ((IEnumerable<Models.TaskStatus> statuses, string applied))(new[] { Models.TaskStatus.InProgress }, "inprogress"),
                "completed" => ((IEnumerable<Models.TaskStatus> statuses, string applied))(new[] { Models.TaskStatus.Completed }, "completed"),
                "all" => ((IEnumerable<Models.TaskStatus> statuses, string applied))(Array.Empty<Models.TaskStatus>(), "all"),
                _ => ((IEnumerable<Models.TaskStatus> statuses, string applied))(new[] { Models.TaskStatus.NotStarted, Models.TaskStatus.InProgress }, "active"),
            };
        }

        public async Task<IActionResult> AssignedDetails(int id)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            TaskModel? task = await taskService.GetAssignedTaskAsync(id, userId);
            if (task == null)
            {
                return NotFound();
            }

            TaskViewModel viewModel = mapper.Map<TaskViewModel>(task);
            viewModel.CanComment = task.OwnerId == userId || task.AssigneeId == userId;

            CommentSectionViewModel comments = await BuildCommentsAsync(task, userId, "assigned");
            viewModel.Comments = comments.Comments;
            ViewBag.CommentsSection = comments;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTag(int id, int listId, string tagName)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            bool added = await tagService.AddTagToTaskAsync(id, tagName, userId);
            TempData[added ? "TagMessage" : "TagError"] = added ? "Tag added to task." : "Unable to add tag.";

            return RedirectToAction(nameof(Edit), new { id, listId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTag(int id, int listId, int tagId)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            ToDoList? list = await taskService.GetListForUserAsync(listId, userId);
            if (list == null)
            {
                return NotFound();
            }

            bool removed = await tagService.RemoveTagFromTaskAsync(id, tagId, userId);
            TempData[removed ? "TagMessage" : "TagError"] = removed ? "Tag removed from task." : "Unable to remove tag.";

            return RedirectToAction(nameof(Edit), new { id, listId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignedUpdateStatus(int id, Models.TaskStatus status, TaskFilterViewModel filter)
        {
            Guid userId = userService.GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            bool updated = await taskService.UpdateStatusAsAssigneeAsync(id, userId, status);
            return !updated 
                ? NotFound() 
                : RedirectToAction(nameof(Assigned), new 
                { 
                    statusFilter = filter.StatusFilter, 
                    sortBy = filter.SortBy, 
                    direction = filter.Direction, 
                    titleQuery = filter.TitleQuery, 
                    createdDate = filter.CreatedDate, 
                    dueDate = filter.DueDate 
                });
        }
    }
}
