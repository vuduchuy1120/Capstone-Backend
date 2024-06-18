using AutoMapper;
using Contract.Services.ProductPhase.ShareDto;
using Domain.Entities;

namespace Application.Mappers;

public class ProductPhaseMappingProfile : Profile
{
    public ProductPhaseMappingProfile()
    {
        CreateMap<ProductPhase, ProductPhaseResponse>()
            .ForCtorParam("ProductName", opt => opt.MapFrom(src => src.Product.Name))
            .ForCtorParam("PhaseName", opt => opt.MapFrom(src => src.Phase.Name))
            ;
    }
}
