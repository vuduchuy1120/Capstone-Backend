using Application.Shared.Dtos.Roles;
using Application.Shared.Dtos.Users;
using AutoMapper;
using Domain.Roles;
using Domain.Users;

namespace Application.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponse>();
        CreateMap<Role, RoleResponse>();
    }
}
