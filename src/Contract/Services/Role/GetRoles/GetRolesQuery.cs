using Contract.Abstractions.Messages;

namespace Contract.Services.Role.GetRoles;

public record GetRolesQuery : IQuery<List<RoleResponse>>;
