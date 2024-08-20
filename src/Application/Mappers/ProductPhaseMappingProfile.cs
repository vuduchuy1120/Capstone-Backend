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

        //CreateMap<ProductPhase, SearchProductPhaseResponse>()
        //    .ForCtorParam("CompanyName", opt => opt.MapFrom(src => src.Company.Name))
        //    .ForCtorParam("ProductId", opt => opt.MapFrom(src => src.ProductId))
        //    .ForCtorParam("ProductName", opt => opt.MapFrom(src => src.Product.Name))
        //    .ForCtorParam("ProductCode", opt => opt.MapFrom(src => src.Product.Code))
        //    .ForCtorParam("ImageUrl", opt => opt.MapFrom(src => src.Product.Images))
        //    .ForCtorParam("PhaseId", opt => opt.MapFrom(src => src.PhaseId))
        //    .ForCtorParam("PhaseName", opt => opt.MapFrom(src => src.Phase.Name))
        //    .ForCtorParam("PhaseDescription", opt => opt.MapFrom(src => src.Phase.Description))
        //    .ForCtorParam("ErrorAvailableQuantity", opt => opt.MapFrom(src => src.ErrorAvailableQuantity))
        //    .ForCtorParam("AvailableQuantity", opt => opt.MapFrom(src => src.AvailableQuantity))
        //    .ForCtorParam("BrokenAvailableQuantity", opt => opt.MapFrom(src => src.BrokenAvailableQuantity))
        //    .ForCtorParam("FailAvailableQuantity", opt => opt.MapFrom(src => src.FailAvailableQuantity))

    }
}
