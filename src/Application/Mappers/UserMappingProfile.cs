using AutoMapper;
using Contract.Services.User.Command;
using Contract.Services.User.CreateUser;
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
               src.Gender,
               src.DOB,
               src.SalaryByDay,
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
