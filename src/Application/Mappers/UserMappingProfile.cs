using AutoMapper;
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
