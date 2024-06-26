//using Application.Abstractions.Data;
//using Application.UserCases.Queries.Attendances;
//using AutoMapper;
//using Contract.Services.Attendance.Queries;
//using Contract.Services.Attendance.Query;
//using Contract.Services.Attendance.ShareDto;
//using Contract.Services.EmployeeProduct.ShareDto;
//using Contract.Services.User.CreateUser;
//using Domain.Entities;
//using Domain.Exceptions.Attendances;
//using Moq;
//using System;
//using System.ComponentModel.Design;

//namespace Application.UnitTests.Attendances.Query;

//public class GetAttendancesQueryHandlerTest
//{
//    private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
//    private readonly Mock<IMapper> _mapperMock;

//    public GetAttendancesQueryHandlerTest()
//    {
//        _mapperMock = new Mock<IMapper>();
//        _attendanceRepositoryMock = new Mock<IAttendanceRepository>();
//    }

//    private Attendance CreateMockAttendance()
//    {
//        var createUserRequest = new CreateUserRequest(
//            "user-123",
//            "John",
//            "Doe",
//            "123456789",
//            "123 Street",
//            "hashedPassword",
//            "Male",
//            "01/01/1990",
//            100,
//            Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286"),
//            1
//        );
//        var user = User.Create(createUserRequest, "hashedPassword", "createdBy");

//        return new Attendance
//        {
//            UserId = "user-123",
//            User = user,
//            Date = DateOnly.Parse("2024-06-01"),
//            HourOverTime = 2,
//            IsAttendance = true,
//            IsOverTime = false,
//            IsSalaryByProduct = true,
//            IsManufacture = false,
//            SlotId = 1
//        };
//    }
//    // handler should return success
//    [Fact]
//    public async Task Handler_ShouldReturnSuccess_WhenReceivedAttendancesIsNotNull()
//    {
//        var getAttendancesRequest = new GetAttendanceRequest(
//            "",
//            "01/06/2024",
//            Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286"), 1);
//        var getAttendancesQuery = new GetAttendancesQuery(
//            getAttendancesRequest,
//            Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286"), "MAIN_ADMIN");

//        var getAttendancesQueryHandler = new GetAttendancesQueryHandler(
//            _attendanceRepositoryMock.Object,
//            _mapperMock.Object);

//        var mockAttendance = CreateMockAttendance();
//        _attendanceRepositoryMock.Setup(repo => repo.SearchAttendancesAsync(getAttendancesRequest))
//            .ReturnsAsync((new List<Attendance> { mockAttendance }, 1));
//        _mapperMock.Setup(mapper => mapper.Map<AttendanceResponse>(It.IsAny<Attendance>()))
//            .Returns(new AttendanceResponse(
//                mockAttendance.UserId,
//                mockAttendance.Date,
//                $"{mockAttendance.User.FirstName} {mockAttendance.User.LastName}",
//                mockAttendance.HourOverTime,
//                mockAttendance.IsAttendance,
//                mockAttendance.IsOverTime,
//                mockAttendance.IsSalaryByProduct,
//                mockAttendance.IsManufacture,
//                new List<EmployeeProductResponse>()));

//        var result = await getAttendancesQueryHandler.Handle(getAttendancesQuery, default);

//        Assert.NotNull(result);
//    }

//    // handler should throw AttendanceNotFoundException when received attendances is null
//    [Fact]
//    public async Task Handler_ShouldThrow_AttendanceNotFoundException_WhenReceivedAttendancesIsNull()
//    {
//        var getAttendancesRequest =
//               new GetAttendanceRequest(
//                   "",
//                   "01/06/2024",
//                   Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286"), 1);
//        var getAttendancesQuery =
//            new GetAttendancesQuery(
//               getAttendancesRequest,
//                Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286"), "MAIN_ADMIN");

//        var getAttendancesQueryHandler = new GetAttendancesQueryHandler(
//            _attendanceRepositoryMock.Object,
//            _mapperMock.Object);

//        _attendanceRepositoryMock.Setup(repo => repo.SearchAttendancesAsync(getAttendancesRequest))
//            .ReturnsAsync(((List<Attendance>)null, 0));

//        await Assert.ThrowsAsync<AttendanceNotFoundException>(async () =>
//        {
//            await getAttendancesQueryHandler.Handle(getAttendancesQuery, default);
//        });
//    }

//    // handler should throw AttendanceNotFoundException when received attendances count equal 0
//    [Fact]
//    public async Task Handler_ShouldThrow_AttendanceNotFoundException_WhenReceivedAttendancesCountEqual0()
//    {
//        var getAttendancesRequest =
//                       new GetAttendanceRequest(
//                           "",
//                           "01/06/2024",
//                           Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286"), 1);
//        var getAttendancesQuery =
//            new GetAttendancesQuery(
//               getAttendancesRequest,
//                Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286"), "MAIN_ADMIN");
//        var getAttendancesQueryHandler = new GetAttendancesQueryHandler(
//            _attendanceRepositoryMock.Object,
//            _mapperMock.Object);

//        _attendanceRepositoryMock.Setup(repo => repo.SearchAttendancesAsync(getAttendancesRequest))
//            .ReturnsAsync((new List<Attendance>(), 0));

//        await Assert.ThrowsAsync<AttendanceNotFoundException>(async () =>
//        {
//            await getAttendancesQueryHandler.Handle(getAttendancesQuery, default);
//        });
//    }

//    // handler should throw AttendanceNotFoundException when total page equal 0
//    [Fact]
//    public async Task Handler_ShouldThrow_AttendanceNotFoundException_WhenTotalPageEqual0()
//    {
//        var getAttendancesRequest =
//       new GetAttendanceRequest(
//           "",
//           "01/06/2024",
//           Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286"), 1);
//        var getAttendancesQuery =
//            new GetAttendancesQuery(
//               getAttendancesRequest,
//                Guid.Parse("b9fb1c8d-b84d-42db-8f5f-cb8583de4286"), "MAIN_ADMIN");
//        var getAttendancesQueryHandler = new GetAttendancesQueryHandler(
//            _attendanceRepositoryMock.Object,
//            _mapperMock.Object);

//        _attendanceRepositoryMock.Setup(repo => repo.SearchAttendancesAsync(getAttendancesRequest))
//            .ReturnsAsync((new List<Attendance>() { new Attendance() }, 0));

//        await Assert.ThrowsAsync<AttendanceNotFoundException>(async () =>
//        {
//            await getAttendancesQueryHandler.Handle(getAttendancesQuery, default);
//        });
//    }

//}
