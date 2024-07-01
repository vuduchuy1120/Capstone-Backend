using Application.Abstractions.Data;
using Contract.Services.Company.Create;
using Contract.Services.Company.Shared;
using Contract.Services.Role.Create;
using Contract.Services.SalaryHistory.Creates;
using Contract.Services.User.BanUser;
using Contract.Services.User.CreateUser;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Users;

public class GetUserByPhoneNumberOrIdAsync : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;

    public GetUserByPhoneNumberOrIdAsync()
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

    [Fact]
    public async Task FoundUserByPhone_Success_ShouldReturnUser()
    {
        await InitDb();

        var retrievedUser = await _userRepository.GetUserByPhoneNumberOrIdAsync("0976099351");

        Assert.NotNull(retrievedUser);
        Assert.Equal("John", retrievedUser.FirstName);
        Assert.Equal("001201011091", retrievedUser.Id);
        Assert.True(retrievedUser.IsActive);
    }

    [Fact]
    public async Task FoundUserById_Success_ShouldReturnUser()
    {
        await InitDb();

        var retrievedUser = await _userRepository.GetUserByPhoneNumberOrIdAsync("001201011091");

        Assert.NotNull(retrievedUser);
        Assert.Equal("John", retrievedUser.FirstName);
        Assert.Equal("001201011091", retrievedUser.Id);
        Assert.True(retrievedUser.IsActive);
    }

    [Fact]
    public async Task UserNotActive_Fail_ShouldReturnNull()
    {
        await InitDb();

        var retrievedUser = await _userRepository.GetUserByPhoneNumberOrIdAsync("001201011092");

        Assert.Null(retrievedUser);
    }

    [Fact]
    public async Task FoundUserByPhone_Fail_ShouldReturnNull()
    {
        await InitDb();

        var retrievedUser = await _userRepository.GetUserByPhoneNumberOrIdAsync("0976099355");

        Assert.Null(retrievedUser);
    }

    [Fact]
    public async Task FoundUserById_Fail_ShouldReturnNull()
    {
        await InitDb();

        var retrievedUser = await _userRepository.GetUserByPhoneNumberOrIdAsync("001201011077");

        Assert.Null(retrievedUser);
    }

    private async Task InitDb()
    {
        var company = Company.Create(new CreateCompanyRequest(new Contract.Services.Company.ShareDto.CompanyRequest("Cơ sở chính", "Hà Nội", "Vũ Đức Huy",
           "0976099789", "admin@admin.com", CompanyType.FACTORY)));

        var role = Role.Create(new CreateRoleCommand("ADMIN", "Admin"));
        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Avatar: "image",
            Phone: "0976099351",
            Address: "123 Main St, Anytown, USA",
            Password: "SecurePassword123",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDayRequest: new SalaryByDayRequest(150, "10/03/2001"),
            SalaryOverTimeRequest: new SalaryOverTimeRequest(200, "10/03/2001"),
            company.Id,
            RoleId: 1
        );
        var user = User.Create(createUserRequest, createUserRequest.Password, createUserRequest.Id);

        var createUserRequest_2 = new CreateUserRequest(
            Id: "001201011092",
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
            company.Id,
            RoleId: 1
        );
        var user_2 = User.Create(createUserRequest_2, createUserRequest_2.Password, createUserRequest.Id);
        user_2.UpdateStatus(new ChangeUserStatusCommand(user.Id, user_2.Id, false));

        _context.Companies.Add(company);
        _context.Roles.Add(role);
        _context.Users.Add(user);
        _context.Users.Add(user_2);
        await _context.SaveChangesAsync();
    }
}
