using Application.Abstractions.Data;
using Contract.Services.Role.Create;
using Contract.Services.SalaryHistory.Creates;
using Contract.Services.User.BanUser;
using Contract.Services.User.CreateUser;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Users;

public class IsUserActiveTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;

    public IsUserActiveTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public async Task UserActive_ShouldReturnTrue()
    {
        await InitDb();

        var isUserActive = await _userRepository.IsUserActiveAsync("001201011091");

        Assert.True(isUserActive);
    }

    [Fact]
    public async Task UserNotActive_ShouldReturnFalse()
    {
        await InitDb();

        var isUserActive = await _userRepository.IsUserActiveAsync("001201011092");

        Assert.False(isUserActive);
    }

    [Fact]
    public async Task UserNotFound_ShouldReturnFalse()
    {
        await InitDb();

        var isUserActive = await _userRepository.IsUserActiveAsync("001201011555");

        Assert.False(isUserActive);
    }

    private async Task InitDb()
    {
        var role = Role.Create(new CreateRoleCommand("ADMIN", "Admin"));
        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Avatar: "image",
            Phone: "123-456-7890",
            Address: "123 Main St, Anytown, USA",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDayRequest: new SalaryByDayRequest(150, "10/03/2001"),
            SalaryOverTimeRequest: new SalaryOverTimeRequest(200, "10/03/2001"),
            Guid.NewGuid(),
            RoleId: 1
        );
        var user = User.Create(createUserRequest, "SecurePassword123", createUserRequest.Id);

        var createUserRequest_2 = new CreateUserRequest(
            Id: "001201011092",
            FirstName: "John",
            LastName: "Doe",
            Avatar: "image",
            Phone: "123-456-7890",
            Address: "123 Main St, Anytown, USA",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDayRequest: new SalaryByDayRequest(150, "10/03/2001"),
            SalaryOverTimeRequest: new SalaryOverTimeRequest(200, "10/03/2001"),
            Guid.NewGuid(),
            RoleId: 1
        );
        var user_2 = User.Create(createUserRequest_2, "SecurePassword123", createUserRequest.Id);
        user_2.UpdateStatus(new ChangeUserStatusCommand(user.Id, user_2.Id, false));

        _context.Roles.Add(role);
        _context.Users.Add(user);
        _context.Users.Add(user_2);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
