using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Users;

internal class SearchUsersTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;

    public SearchUsersTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _userRepository = new UserRepository(_context);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
