using AutoMapper;
using Contract.Services.Role.GetRoles;
using Domain.Entities;

namespace Application.Mappers;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<Role, RoleResponse>();
    }
}
