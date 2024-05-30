using Contract.Services.Slot.GetSlots;
using Domain.Entities;

namespace Application.Mappers;
public class SlotMapperProfile : AutoMapper.Profile
{
    public SlotMapperProfile()
    {
        CreateMap<Slot, SlotResponse>();
    }
}
