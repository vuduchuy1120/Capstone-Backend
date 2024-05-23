using Application.Abstractions.Data;
using Application.Shared.Dtos;
using Application.Shared.Dtos.Roles;
using Domain.Roles;
using MediatR;
using System.Net;

namespace Application.Roles.Create;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, SuccessResponse<RoleResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _roleRepository;

    public CreateRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository roleRepository)
    {
        _unitOfWork = unitOfWork;
        _roleRepository = roleRepository;
    }

    public async Task<SuccessResponse<RoleResponse>> Handle(
        CreateRoleCommand request, 
        CancellationToken cancellationToken)
    {
        var isRoleExisted = await _roleRepository.IsRoleExisted(request.RoleName);

        if(isRoleExisted)
        {
            throw new RoleAlreadyExistedException(request.RoleName);
        }

        var role = Role.Create(request.RoleName, request.Decription);

        _roleRepository.AddRole(role);
        await _unitOfWork.SaveChangesAsync();

        var roleRespone = new RoleResponse
        {
            Id = role.Id,
            RoleName = role.RoleName,
            Decription = role.Decription,
        };

        return new()
        {
            Status = (int) HttpStatusCode.Created,
            Message = "Create new role success",
            Data = roleRespone
        };
    }
}
