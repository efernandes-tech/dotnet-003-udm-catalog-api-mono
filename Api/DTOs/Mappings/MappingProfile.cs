using Api.Models;
using AutoMapper;

namespace Api.DTOs.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductModel, ProductDTO>().ReverseMap();
        CreateMap<CategoryModel, CategoryDTO>().ReverseMap();
    }
}
