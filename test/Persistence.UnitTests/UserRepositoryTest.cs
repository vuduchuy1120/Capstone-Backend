using Application.Abstractions.Data;
using Contract.Services.User.Command;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using Xunit;

namespace Persistence.UnitTests
{
    public class UserRepositoryTest : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;

        public UserRepositoryTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _userRepository = new UserRepository(_context); 
        }

        //[Fact]
        //public async Task AddUser_ShouldAddUserToDatabase()
        //{
        //    var user = User.Create("0012", "Dinh son", "0976099351", "Ha noi", "password", 1, "001232");
        //    _userRepository.AddUser(user);
        //    await _context.SaveChangesAsync();

        //    Assert.Single(_context.Users);
        //    var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        //    Assert.NotNull(savedUser);
        //    Assert.Equal("Dinh son", savedUser.Fullname);
        //}

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

