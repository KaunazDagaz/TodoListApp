using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Services.IServices;
using TodoListApp.WebApp.ViewModels;

namespace TodoListApp.WebApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
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

        private IActionResult RedirectToOrigin(string origin, int taskId, int listId)
        {
            return origin?.ToLowerInvariant() switch
            {
                "edit" => RedirectToAction("Edit", "Task", new { id = taskId, listId }),
                "assigned" => RedirectToAction("AssignedDetails", "Task", new { id = taskId }),
                _ => RedirectToAction("Details", "Task", new { id = taskId, listId })
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentInputModel input)
        {
            Guid userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            if (!ModelState.IsValid)
            {
                TempData["CommentError"] = "Comment text is required.";
                return RedirectToOrigin(input.Origin, input.TaskId, input.ListId);
            }

            bool added = await commentService.AddToTaskAsync(input.TaskId, input.Text, userId);
            TempData[added ? "CommentMessage" : "CommentError"] = added ? "Comment added." : "Unable to add comment.";

            return RedirectToOrigin(input.Origin, input.TaskId, input.ListId);
        }

        public async Task<IActionResult> Edit(int id, int taskId, int listId, string origin = "details")
        {
            Guid userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            CommentEditViewModel? viewModel = null;
            var comment = await commentService.GetForEditAsync(id, userId);
            if (comment != null)
            {
                int commentListId = comment.Task?.ToDoListId ?? listId;
                viewModel = new CommentEditViewModel
                {
                    Id = comment.Id,
                    TaskId = comment.TaskId,
                    ListId = commentListId,
                    Origin = origin,
                    Text = comment.Text
                };
            }

            return viewModel == null ? NotFound() : View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CommentEditViewModel viewModel)
        {
            Guid userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            bool updated = await commentService.UpdateTextAsync(viewModel.Id, viewModel.Text, userId);
            TempData[updated ? "CommentMessage" : "CommentError"] = updated ? "Comment updated." : "Unable to update comment.";

            return RedirectToOrigin(viewModel.Origin, viewModel.TaskId, viewModel.ListId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int taskId, int listId, string origin = "details")
        {
            Guid userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToAction("Index", "ToDoList");
            }

            bool deleted = await commentService.DeleteOwnedAsync(id, userId);
            TempData[deleted ? "CommentMessage" : "CommentError"] = deleted ? "Comment deleted." : "Unable to delete comment.";

            return RedirectToOrigin(origin, taskId, listId);
        }
    }
}
