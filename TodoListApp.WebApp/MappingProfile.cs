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
        }
    }
}
