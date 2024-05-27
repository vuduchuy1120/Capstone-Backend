using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Role.GetRoles;
using Domain.Exceptions.Roles;

namespace Application.UserCases.Queries.Roles;

internal sealed class GetRolesQueryHandler(IRoleRepository _roleRepository, IMapper _mapper)
    : IQueryHandler<GetRolesQuery, List<RoleResponse>>
{
    public async Task<Result.Success<List<RoleResponse>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllRolesAsync() ?? throw new RoleNotFoundException();

        var rolesResponse = roles.ConvertAll(role => _mapper.Map<RoleResponse>(role));

        return Result.Success<List<RoleResponse>>.Get(rolesResponse);
    }
}