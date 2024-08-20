using AutoMapper;
using Contract.Services.MaterialHistory.ShareDto;
using Domain.Entities;

namespace Application.Mappers;

public class MaterialHistoryMappingProfile : Profile
{
    public MaterialHistoryMappingProfile()
    {
        CreateMap<MaterialHistory, MaterialHistoryResponse>()
            .ForCtorParam("Image", opt => opt.MapFrom(src => src.Material.Image ?? "No Image"))
            .ForCtorParam("MaterialName", opt => opt.MapFrom(src => src.Material.Name))
            .ForCtorParam("MaterialUnit", opt => opt.MapFrom(src => src.Material.Unit));
    }
}
