using Application.Shared.Dtos;
using Application.Shared.Dtos.Roles;
using MediatR;

namespace Application.Roles.Create;

public class CreateRoleCommand : IRequest<SuccessResponse<RoleResponse>>
{
    public string RoleName { get; set; }
    public string Decription { get; set; }
}
