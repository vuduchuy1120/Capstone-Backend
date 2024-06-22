using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.ShareDtos;
using Domain.Exceptions.Attendances;

namespace Application.UserCases.Queries.Attendances;

public sealed class GetAttendancesByMonthAndUserIdQueryHandler
    (IAttendanceRepository _attendanceRepository, IMapper _mapper
    ) : IQueryHandler<GetAttendancesByMonthAndUserIdQuery, AttendanceUserResponse>
{
    public async Task<Result.Success<AttendanceUserResponse>> Handle(GetAttendancesByMonthAndUserIdQuery request, CancellationToken cancellationToken)
    {
        var attendances = await _attendanceRepository.GetAttendanceByMonthAndUserIdAsync(request.Month, request.Year, request.UserId);
        if (attendances is null || attendances.Count <= 0)
        {
            throw new AttendanceNotFoundException();
        }
        var attendanceResponse = attendances
                .OrderBy(a => a.Date) 
                .GroupBy(a => new { a.Date.Month, a.Date.Year, a.UserId })
                .Select(group => new AttendanceUserResponse(
                    Month: group.Key.Month,
                    Year: group.Key.Year,
                    UserId: group.Key.UserId,
                    Attendances: group
                        .GroupBy(a => a.Date.ToString("dd/MM/yyyy"))
                        .Select(dateGroup => new AttendanceUserReportResponse(
                            Date: dateGroup.Key,
                            AttedanceDateReport: new AttedanceDateReport(
                                IsPresentSlot1: dateGroup.Any(a => a.SlotId == 1 && a.IsAttendance),
                                IsPresentSlot2: dateGroup.Any(a => a.SlotId == 2 && a.IsAttendance),
                                IsPresentSlot3: dateGroup.Any(a => a.SlotId == 3 && a.IsAttendance),
                                IsSalaryByProduct: dateGroup.Any(a => a.IsSalaryByProduct),
                                IsOverTime: dateGroup.Any(a => a.IsOverTime)
                            )
                        )).ToList()
                )).FirstOrDefault();

        return Result.Success<AttendanceUserResponse>.Get(attendanceResponse);

    }
}
