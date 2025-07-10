using AutoMapper;
using EmployeeManagementSystem.Api.Models;
using EmployeeManagementSystem.Api.DTOs;

namespace EmployeeManagementSystem.Api.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
        }
    }
}
