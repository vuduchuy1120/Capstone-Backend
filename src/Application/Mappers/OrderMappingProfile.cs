using AutoMapper;
using Contract.Services.Order.ShareDtos;
using Domain.Entities;

namespace Application.Mappers;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderResponse>()
            .ForCtorParam("Company", opt => opt.MapFrom(src => src.Company))
            .ForCtorParam("StatusType", opt => opt.MapFrom(src => src.Status.ToString()))
            .ForCtorParam("StatusDescription", opt => opt.MapFrom(src => src.Status.GetDescription()));
    }
}
