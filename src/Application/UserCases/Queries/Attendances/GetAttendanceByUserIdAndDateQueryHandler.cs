using Application.Abstractions.Data;
using Application.Utils;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.ShareDto;
using Contract.Services.Attendance.ShareDtos;
using Contract.Services.EmployeeProduct.ShareDto;
using Domain.Entities;
using Domain.Exceptions.Attendances;

namespace Application.UserCases.Queries.Attendances;

public sealed class GetAttendanceByUserIdAndDateQueryHandler
    (IAttendanceRepository _attendanceRepository,
    IMapper _mapper
    ) : IQueryHandler<GetAttendanceByUserIdAndDateQuery, AttendanceUserDetailResponse>
{
    public async Task<Result.Success<AttendanceUserDetailResponse>> Handle(GetAttendanceByUserIdAndDateQuery request, CancellationToken cancellationToken)
    {
        var formatedDate = DateUtil.ConvertStringToDateTimeOnly(request.Date);
        var attendances = await _attendanceRepository.GetAttendanceByUserIdAndDateAsync(request.UserId, formatedDate);
        if (attendances == null)
        {
            throw new AttendanceNotFoundException();
        }

        var attendanceSlotReports = new List<AttendanceSlotReport>();

        foreach (var attendance in attendances)
        {
            var employeeProductResponses = attendance.User.EmployeeProducts
                .Where(ep => ep.Date == attendance.Date && ep.SlotId == attendance.SlotId && ep.UserId == attendance.UserId)
                .Select(ep => new EmployeeProductResponse
                (
                    ImageUrl: ep.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty,
                    ProductName: ep.Product.Name,
                    ProductId: ep.Product.Id,
                    PhaseId: ep.Phase.Id,
                    PhaseName: ep.Phase.Name,
                    Quantity: ep.Quantity
                ))
                .ToList();

            var report = new AttendanceSlotReport
            (
                SlotId: attendance.SlotId,
                IsAttendance: attendance.IsAttendance,
                IsOvertime: attendance.IsOverTime,
                HourOverTime: attendance.HourOverTime,
                IsSalaryByProduct: attendance.IsSalaryByProduct,
                EmployeeProductResponses: employeeProductResponses
            );

            attendanceSlotReports.Add(report);
        }

        var response = new AttendanceUserDetailResponse
        (
            Date: request.Date,
            UserId: request.UserId,
            AttendanceSlotReports: attendanceSlotReports
        );

        return Result.Success<AttendanceUserDetailResponse>.Get(response);
    }
}
