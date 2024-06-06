using AutoMapper;

namespace Application.Mappers;

public sealed class MaterialMappingProfile : Profile
{
    public MaterialMappingProfile()
    {
        CreateMap<Domain.Entities.Material, Contract.Services.Material.ShareDto.MaterialResponse>();
    }
}
