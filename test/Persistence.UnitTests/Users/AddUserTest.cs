using Application.Abstractions.Data;
using Contract.Services.SalaryHistory.Creates;
using Contract.Services.User.CreateUser;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Users;

public class AddUserTest : IDisposable
{

    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;

    public AddUserTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddUser_Success_ShouldAddNewUserToDb()
    {
        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Avatar: "image",
            Phone: "123-456-7890",
            Address: "123 Main St, Anytown, USA",
            Password: "SecurePassword123",
            Gender: "Male",
            DOB:"10/03/2001",
            SalaryByDayRequest: new SalaryByDayRequest(150, "10/03/2001"),
            SalaryOverTimeRequest: new SalaryOverTimeRequest(200, "10/03/2001"),
            Guid.NewGuid(),
            RoleId: 2
        );

        var user = User.Create(createUserRequest, createUserRequest.Password, "001201011091");
        _userRepository.AddUser(user);
        await _context.SaveChangesAsync();

        Assert.Single(_context.Users);
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

        Assert.NotNull(savedUser);
        Assert.Equal("001201011091", savedUser.Id);
    }

    [Fact]
    public async Task AddUser_IdExisted_Error_ShouldNotAddNewUserToDb()
    {
        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Avatar: "image",
            Phone: "123-456-7890",
            Address: "123 Main St, Anytown, USA",
            Password: "SecurePassword123",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDayRequest: new SalaryByDayRequest(150, "10/03/2001"),
            SalaryOverTimeRequest: new SalaryOverTimeRequest(200, "10/03/2001"),
            Guid.NewGuid(),
            RoleId: 2
        );

        var user = User.Create(createUserRequest, createUserRequest.Password, "001201011091");
        _userRepository.AddUser(user);
        await _context.SaveChangesAsync();

        var duplicateUser = User.Create(createUserRequest, createUserRequest.Password, createUserRequest.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            _userRepository.AddUser(duplicateUser);
            await _context.SaveChangesAsync();
        });

        Assert.Single(_context.Users);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
