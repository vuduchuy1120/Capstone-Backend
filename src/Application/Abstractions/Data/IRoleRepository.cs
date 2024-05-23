using Domain.Roles;

namespace Application.Abstractions.Data;

public interface IRoleRepository
{
    void AddRole(Role role);
    Task<bool> IsRoleExisted(string roleName);
}
