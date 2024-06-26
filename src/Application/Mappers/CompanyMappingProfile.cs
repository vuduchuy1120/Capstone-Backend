﻿using AutoMapper;
using Contract.Services.Company.Shared;
using Contract.Services.Company.ShareDtos;
using Domain.Entities;

namespace Application.Mappers;

public class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        CreateMap<Company, CompanyResponse>()
            .ForCtorParam("CompanyTypeDescription",
                       opt => opt.MapFrom(src => src.CompanyType.GetDescription()));
    }

}
