using AutoMapper;
using WebApi.Domain.DTOs;
using WebApi.Domain.Model;

namespace WebApi.Application.Mapping
{
    public class DomainToDTOMapping : Profile
    { 
        public DomainToDTOMapping()
        {
            CreateMap<EmployeeModel, EmployeeDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NameEmployee, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo));
        }
    }

 
}