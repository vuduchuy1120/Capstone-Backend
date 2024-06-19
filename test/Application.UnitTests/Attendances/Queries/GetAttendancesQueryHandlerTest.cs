using Application.Abstractions.Data;
using Application.UserCases.Queries.Attendances;
using AutoMapper;
using Contract.Services.Attendance.Query;
using Contract.Services.Attendance.ShareDto;
using Domain.Entities;
using Domain.Exceptions.Attendances;
using Moq;

namespace Application.UnitTests.Attendances.Query;

public class GetAttendancesQueryHandlerTest
{
    private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    public GetAttendancesQueryHandlerTest()
    {
        _mapperMock = new();
        _attendanceRepositoryMock = new();
    }

    // handler should return success
    [Fact]
    public async Task Handler_ShouldReturnSuccess_WhenReceivedAttendancesIsNotNull()
    {
        var getAttendancesQuery = new GetAttendancesQuery("", "01/06/2024", 1);
        var getAttendancesQueryHandler = new GetAttendancesQueryHandler(
            _attendanceRepositoryMock.Object,
            _mapperMock.Object);

        _attendanceRepositoryMock.Setup(repo => repo.SearchAttendancesAsync(getAttendancesQuery))
            .ReturnsAsync((new List<Attendance>() { new Attendance() }, 1));
        _mapperMock.Setup(mapper => mapper.Map<AttendanceResponse>(It.IsAny<Attendance>()))
            .Returns(It.IsAny<AttendanceResponse>);

        var result = await getAttendancesQueryHandler.Handle(getAttendancesQuery, default);

        Assert.NotNull(result);
    }

    // handler should throw AttendanceNotFoundException when received attendances is null
    [Fact]
    public async Task Handler_ShouldThrow_AttendanceNotFoundException_WhenReceivedAttendancesIsNull()
    {
        var getAttendancesQuery = new GetAttendancesQuery("", "01/06/2024", 1);
        var getAttendancesQueryHandler = new GetAttendancesQueryHandler(
            _attendanceRepositoryMock.Object,
            _mapperMock.Object);

        _attendanceRepositoryMock.Setup(repo => repo.SearchAttendancesAsync(getAttendancesQuery))
            .ReturnsAsync(((List<Attendance>)null, 0));

        await Assert.ThrowsAsync<AttendanceNotFoundException>(async () =>
        {
            await getAttendancesQueryHandler.Handle(getAttendancesQuery, default);
        });
    }

    // handler should throw AttendanceNotFoundException when received attendances count equal 0
    [Fact]
    public async Task Handler_ShouldThrow_AttendanceNotFoundException_WhenReceivedAttendancesCountEqual0()
    {
        var getAttendancesQuery = new GetAttendancesQuery("", "01/06/2024", 1);
        var getAttendancesQueryHandler = new GetAttendancesQueryHandler(
            _attendanceRepositoryMock.Object,
            _mapperMock.Object);

        _attendanceRepositoryMock.Setup(repo => repo.SearchAttendancesAsync(getAttendancesQuery))
            .ReturnsAsync((new List<Attendance>(), 0));

        await Assert.ThrowsAsync<AttendanceNotFoundException>(async () =>
        {
            await getAttendancesQueryHandler.Handle(getAttendancesQuery, default);
        });
    }

    // handler should throw AttendanceNotFoundException when total page equal 0
    [Fact]
    public async Task Handler_ShouldThrow_AttendanceNotFoundException_WhenTotalPageEqual0()
    {
        var getAttendancesQuery = new GetAttendancesQuery("", "01/06/2024", 1);
        var getAttendancesQueryHandler = new GetAttendancesQueryHandler(
            _attendanceRepositoryMock.Object,
            _mapperMock.Object);

        _attendanceRepositoryMock.Setup(repo => repo.SearchAttendancesAsync(getAttendancesQuery))
            .ReturnsAsync((new List<Attendance>() { new Attendance() }, 0));

        await Assert.ThrowsAsync<AttendanceNotFoundException>(async () =>
        {
            await getAttendancesQueryHandler.Handle(getAttendancesQuery, default);
        });
    }

}
