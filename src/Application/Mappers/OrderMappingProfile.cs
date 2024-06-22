using AutoMapper;

namespace Application.Mappers;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Domain.Entities.Order, Contract.Services.Order.ShareDtos.OrderResponse>();
    }
}
