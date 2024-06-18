﻿using Application.Abstractions.Data;
using Contract.Services.Role.Create;
using Contract.Services.User.BanUser;
using Contract.Services.User.CreateUser;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Users;

public class GetUserByIdTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;

    public GetUserByIdTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public async Task UserActive_Success_ShouldReturnUser()
    {
        await InitDb();

        var retrievedUser = await _userRepository.GetUserByIdAsync("001201011091");

        Assert.NotNull(retrievedUser);
        Assert.Equal("John", retrievedUser.FirstName);
        Assert.Equal("001201011091", retrievedUser.Id);
        Assert.True(retrievedUser.IsActive);
    }

    [Fact]
    public async Task UserNotActive_Success_ShouldReturnUser()
    {
        await InitDb();

        var retrievedUser = await _userRepository.GetUserByIdAsync("001201011092");

        Assert.NotNull(retrievedUser);
        Assert.Equal("John", retrievedUser.FirstName);
        Assert.Equal("001201011092", retrievedUser.Id);
        Assert.False(retrievedUser.IsActive);
    }

    [Fact]
    public async Task UserNotFound_Fail_ShouldReturnNull()
    {
        await InitDb();

        var retrievedUser = await _userRepository.GetUserByIdAsync("00120101103432");

        Assert.Null(retrievedUser);
    }

    private async Task InitDb()
    {
        var role = Role.Create(new CreateRoleCommand("ADMIN", "Admin"));
        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Phone: "123-456-7890",
            Address: "123 Main St, Anytown, USA",
            Password: "SecurePassword123",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDay: 150,
            Guid.NewGuid(),
            RoleId: 1
        );
        var user = User.Create(createUserRequest, createUserRequest.Password, createUserRequest.Id);

        var createUserRequest_2 = new CreateUserRequest(
            Id: "001201011092",
            FirstName: "John",
            LastName: "Doe",
            Phone: "123-456-7890",
            Address: "123 Main St, Anytown, USA",
            Password: "SecurePassword123",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDay: 150,
            Guid.NewGuid(),
            RoleId: 1
        );
        var user_2 = User.Create(createUserRequest_2, createUserRequest_2.Password, createUserRequest.Id);
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
