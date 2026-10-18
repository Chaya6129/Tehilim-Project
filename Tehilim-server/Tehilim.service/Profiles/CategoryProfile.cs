using AutoMapper;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;

namespace Tehilim.Service.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>();
        }
    }
}