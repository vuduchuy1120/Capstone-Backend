using Application.Utils;
using AutoMapper;
using Contract.Abstractions.Shared.Utils;
using Contract.Services.SalaryHistory.ShareDtos;

using Contract.Services.User.SharedDto;
using Domain.Entities;

namespace Application.Helpers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponse>()
           .ConstructUsing(src => new UserResponse(
               src.Id,
               src.FirstName,
               src.LastName,
               src.AccountBalance,
               src.Phone,
               src.Address,
               src.Avatar,
               src.Gender,
               src.DOB,
               new SalaryHistoryResponse(
                       src.SalaryHistories
                           .Where(sh => sh.SalaryType == SalaryType.SALARY_BY_DAY)
                           .Select(sh => new SalaryByDayResponse(sh.Salary, sh.StartDate))
                           .OrderByDescending(sh => sh.StartDate)
                           .FirstOrDefault(),
                       src.SalaryHistories
                           .Where(sh => sh.SalaryType == SalaryType.SALARY_OVER_TIME)
                           .Select(sh => new SalaryByOverTimeResponse(sh.Salary, sh.StartDate))
                           .OrderByDescending(sh => sh.StartDate)
                           .FirstOrDefault()
                   ),
               DateUtil.ConvertStringToDateTimeOnly(src.PaidSalaries
                   .OrderByDescending(ps => ps.CreatedDate)
                   .Select(ps => ps.CreatedDate)
                   .FirstOrDefault().ToString("dd/MM/yyyy")),
               src.IsActive,
               src.RoleId,
               src.Role.RoleName,
               src.Company.Name,
               src.CompanyId
               ))
           .ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.Role.RoleName))
           .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));

        CreateMap<User, UserResponseWithoutSalary>()
           .ConstructUsing(src => new UserResponseWithoutSalary(
               src.Id,
               src.FirstName,
               src.LastName,
               src.Phone,
               src.Address,
               src.Avatar,
               src.Gender,
               src.DOB,
               src.IsActive,
               src.RoleId,
               src.Role.RoleName,
               src.Company.Name,
               src.CompanyId
               ))
           .ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.Role.RoleName))
           .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));
    }
}
