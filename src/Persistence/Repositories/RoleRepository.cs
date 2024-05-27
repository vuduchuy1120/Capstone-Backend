using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddRole(Role role)
    {
        _context.Roles.Add(role);
    }

    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _context.Roles
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> IsRoleExisted(string roleName)
    {
        return await _context.Roles
            .AnyAsync(role => role.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase));
    }
}
