using AutoMapper;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.ViewModels;
using System.Linq;

namespace TodoListApp.WebApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDoList, ToDoListViewModel>().ReverseMap();
            CreateMap<Models.Task, TaskViewModel>()
                .ForMember(dest => dest.AssigneeDisplay, opt => opt.MapFrom(src =>
                    src.Assignee != null
                        ? (!string.IsNullOrWhiteSpace(src.Assignee.Email)
                            ? src.Assignee.Email
                            : src.Assignee.Id.ToString())
                        : src.AssigneeId.ToString()))
                .ForMember(dest => dest.ToDoListTitle, opt => opt.MapFrom(src => src.ToDoList != null ? src.ToDoList.Title : null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.TaskTags != null ? src.TaskTags.Where(tt => tt.Tag != null).Select(tt => tt.Tag) : Enumerable.Empty<Tag>()))
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.CanComment, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Assignee, opt => opt.Ignore())
                .ForMember(dest => dest.ToDoList, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.TaskTags, opt => opt.Ignore());

            CreateMap<Tag, TagViewModel>().ReverseMap();

            CreateMap<Comment, CommentViewModel>()
                .ForMember(dest => dest.OwnerDisplay, opt => opt.MapFrom(src =>
                    src.Owner != null && !string.IsNullOrWhiteSpace(src.Owner.Email)
                        ? src.Owner.Email
                        : src.OwnerId.ToString()))
                .ForMember(dest => dest.IsMine, opt => opt.Ignore());
        }
    }
}
