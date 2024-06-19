using AutoMapper;
using Contract.Services.Phase.ShareDto;
using Domain.Entities;

namespace Application.Mappers;

public class PhaseMappingProfile : Profile
{
    public PhaseMappingProfile()
    {
        CreateMap<Phase, PhaseResponse>();
    }
}
