using AutoMapper;
using Contract.Services.Company.Shared;
using Domain.Entities;

namespace Application.Mappers;

public class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        CreateMap<Company, CompanyResponse>();
    }
}
