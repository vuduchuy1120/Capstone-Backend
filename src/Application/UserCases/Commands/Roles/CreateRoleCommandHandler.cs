using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Role.Create;
using Domain.Entities;
using Domain.Exceptions.Roles;

namespace Application.UserCases.Commands.Roles;

internal sealed class CreateRoleCommandHandler(IRoleRepository _roleRepository, IUnitOfWork _unitOfWork)
    : ICommandHandler<CreateRoleCommand>
{
    public async Task<Result.Success> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var isRoleExisted = await _roleRepository.IsRoleExisted(request.RoleName);

        if (isRoleExisted)
        {
            throw new RoleAlreadyExistedException(request.RoleName);
        }

        var role = Role.Create(request);

        _roleRepository.AddRole(role);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Create();
    }
}
