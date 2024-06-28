using AutoMapper;

namespace Application.Mappers;

public class EmployeeProductMappingProfile : Profile
{
    public EmployeeProductMappingProfile()
    {
        CreateMap<Domain.Entities.EmployeeProduct, Contract.Services.EmployeeProduct.ShareDto.EmployeeProductResponse>()
            .ForCtorParam("ImageUrl", opt => opt.MapFrom(src => src.Product.Images.FirstOrDefault(x => x.IsMainImage).ImageUrl ?? "no image"))
            .ForCtorParam("ProductName", opt => opt.MapFrom(src => src.Product.Name))
            .ForCtorParam("PhaseName", opt => opt.MapFrom(src => src.Phase.Name))
            .ForCtorParam("PhaseId", opt => opt.MapFrom(src => src.Phase.Id))
            .ForCtorParam("ProductId", opt => opt.MapFrom(src => src.Product.Id))
            .ForCtorParam("Quantity", opt => opt.MapFrom(src => src.Quantity));

    }
}
