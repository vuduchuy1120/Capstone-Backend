using AutoMapper;
using Contract.Services.Company.ShareDtos;
using Domain.Entities;

namespace Application.Mappers;

public class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        CreateMap<Company, CompanyResponse>();
    }

}
