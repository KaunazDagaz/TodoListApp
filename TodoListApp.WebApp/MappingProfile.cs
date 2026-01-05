using AutoMapper;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.ViewModels;

namespace TodoListApp.WebApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDoList, ToDoListViewModel>().ReverseMap();
            CreateMap<Models.Task, TaskViewModel>()
                .ForMember(dest => dest.AssigneeDisplay, opt => opt.MapFrom(src => src.Assignee != null ? src.Assignee.UserAgent ?? src.Assignee.Id.ToString() : src.AssigneeId.ToString()))
                .ForMember(dest => dest.ToDoListTitle, opt => opt.MapFrom(src => src.ToDoList != null ? src.ToDoList.Title : null))
                .ReverseMap()
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Assignee, opt => opt.Ignore())
                .ForMember(dest => dest.ToDoList, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore());
        }
    }
}
