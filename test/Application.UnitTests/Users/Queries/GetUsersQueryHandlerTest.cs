using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.UserCases.Queries.Users;
using AutoMapper;
using Contract.Services.SalaryHistory.Creates;
using Contract.Services.SalaryHistory.ShareDtos;
using Contract.Services.Set.GetSet;
using Contract.Services.User.CreateUser;
using Contract.Services.User.GetUsers;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Users;
using Moq;
using System.ComponentModel.Design;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.UnitTests.Users.Queries;

public class GetUsersQueryHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICloudStorage> _cloudStorageMock;
    private readonly Mock<IMapper> _mapperMock;

    public GetUsersQueryHandlerTest()
    {
        _mapperMock = new();
        _userRepositoryMock = new();
        _cloudStorageMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenReceivedUsersIsNull()
    {
        var getUsersQuery = new GetUsersQuery("search", 1, true);
        var getUsersQueryHandler = new GetUsersQueryHandler(_userRepositoryMock.Object, _mapperMock.Object, _cloudStorageMock.Object);

        _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(getUsersQuery)).ReturnsAsync(((List<User>)null, 0));
        _cloudStorageMock.Setup(cloud => cloud.GetSignedUrlAsync(It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await getUsersQueryHandler.Handle(getUsersQuery, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenReceivedUsersCountEqual0()
    {
        var getUsersQuery = new GetUsersQuery("search", 1, true);
        var getUsersQueryHandler = new GetUsersQueryHandler(_userRepositoryMock.Object, _mapperMock.Object, _cloudStorageMock.Object);

        _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(getUsersQuery)).ReturnsAsync((new List<User>(), 0));
        _cloudStorageMock.Setup(cloud => cloud.GetSignedUrlAsync(It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await getUsersQueryHandler.Handle(getUsersQuery, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenTotalPageEqual0()
    {
        var getUsersQuery = new GetUsersQuery("search", 1, true);
        var getUsersQueryHandler = new GetUsersQueryHandler(_userRepositoryMock.Object, _mapperMock.Object, _cloudStorageMock.Object);

        _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(getUsersQuery)).ReturnsAsync((new List<User>() { new User() }, 0));
        _cloudStorageMock.Setup(cloud => cloud.GetSignedUrlAsync(It.IsAny<string>())).ReturnsAsync(It.IsAny<string>());
        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await getUsersQueryHandler.Handle(getUsersQuery, default);
        });
    }

    //[Fact]
    //public async Task Handler_ShouldReturn_SuccessResult()
    //{
    //    var getUsersQuery = new GetUsersQuery("search", 1, true);
    //    var getUsersQueryHandler = new GetUsersQueryHandler(_userRepositoryMock.Object, _mapperMock.Object, _cloudStorageMock.Object);
    //    var userCreateRequest = new CreateUserRequest(
    //       "001201011091",
    //       "Son",
    //       "Nguyen",
    //       "iamge",
    //       "0976099351",
    //       "Ha Noi",
    //       "12345",
    //       "Male",
    //       "10/03/2001",
    //       SalaryByDayRequest: new SalaryByDayRequest(100000, "01/01/2024"),
    //       SalaryOverTimeRequest: new SalaryOverTimeRequest(150000, "01/01/2024"),
    //       Guid.NewGuid(),
    //       1
    //       );
    //    var users = new List<User> { User.Create(userCreateRequest, "hash", userCreateRequest.Id) };

    //    _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(getUsersQuery)).ReturnsAsync((users, 1));

    //    _cloudStorageMock.Setup(cloud => cloud.GetSignedUrlAsync(It.IsAny<string>())).ReturnsAsync("signed_url");

    //    _mapperMock.Setup(mapper => mapper.Map<UserResponse>(It.IsAny<User>())).Returns((User user) => new UserResponse(
    //        user.Id,
    //        user.FirstName,
    //        user.LastName,
    //        user.Phone,
    //        user.Address,
    //        "mapped_avatar_url",
    //        user.Gender,
    //        user.DOB,
    //        new SalaryHistoryResponse(new SalaryByDayResponse(100000, DateOnly.Parse("01/01/2024")), new SalaryByOverTimeResponse(150000, DateOnly.Parse("01/01/2024"))),
    //        user.IsActive,
    //        user.RoleId,
    //        user.Role.RoleName,
    //        user.Company.Name,
    //        user.CompanyId
    //    ));

    //    var result = await getUsersQueryHandler.Handle(getUsersQuery, default);

    //    Assert.True(result.isSuccess);
        
    //}
}
