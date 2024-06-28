using AutoMapper;
using Contract.Services.Company.Shared;
using Contract.Services.Company.ShareDtos;
using Contract.Services.Shipment.Share;
using Domain.Entities;

namespace Application.Mappers;

public class ShipmentMappingProfile : Profile
{
    public ShipmentMappingProfile()
    {
        CreateMap<Shipment, ShipmentResponse>()
            .ConstructUsing((src, context) =>
                new ShipmentResponse(
                    context.Mapper.Map<CompanyResponse>(src.FromCompany),
                    context.Mapper.Map<CompanyResponse>(src.ToCompany),
                    src.ShipDate,
                    src.Status.GetDescription(),
                    src.Status
                ));
    }
}
