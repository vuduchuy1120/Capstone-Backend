using Application.Abstractions.Data;
using Application.Users.GetUsers;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddUser(User user)
    {
        _context.Users.Add(user);
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id.Equals(id));
    }

    public async Task<bool> IsUserExistAsync(string id)
    {
        return await _context.Users.AnyAsync(user => user.Id.Equals(id));
    }

    public async Task<(List<User>?, int)> SearchUsersAsync(GetUsersQuery request)
    {
        var query = _context.Users.AsQueryable();
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = _context.Users.Where(user => user.Id.Contains(request.SearchTerm));
        }

        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var users = await query
            .OrderBy(user => user.CreatedDate)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return (users, totalPages);
    }
}
