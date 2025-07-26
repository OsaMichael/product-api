using AutoMapper;
using BMO_Assessment.Data;
using BMO_Assessment.Models;

namespace BMO_Assessment.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductRequest>()
                .ForMember(s => s.ProductName, b => b.MapFrom(b => b.Name))
                .ForMember(s => s.Category, b => b.MapFrom(b => b.Category))
                .ForMember(s => s.Description, b => b.MapFrom(b => b.Description))
                .ForMember(s => s.Price, b => b.MapFrom(b => b.Price))
                .ReverseMap();

            CreateMap<ProductResponse, Product>()
                .ForMember(s => s.Name, b => b.MapFrom(b => b.ProductName))
                .ForMember(s => s.Category, b => b.MapFrom(b => b.Category))
                .ForMember(s => s.Description, b => b.MapFrom(b => b.Description))
                .ForMember(s => s.Price, b => b.MapFrom(b => b.Price))
               // .ForMember(s => s.CreatedDate, b => b.MapFrom(b => b.CreatedDate))
               .ReverseMap();

        }
    }
}
