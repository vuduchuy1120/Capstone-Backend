using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IRoleRepository
{
    void AddRole(Role role);
    Task<bool> IsRoleExisted(string roleName);
    Task<List<Role>> GetAllRolesAsync();
}
