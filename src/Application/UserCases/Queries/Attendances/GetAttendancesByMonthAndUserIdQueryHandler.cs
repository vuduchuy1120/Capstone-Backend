using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Queries;
using Contract.Services.Attendance.ShareDtos;
using Domain.Exceptions.Attendances;

namespace Application.UserCases.Queries.Attendances;

public sealed class GetAttendancesByMonthAndUserIdQueryHandler
    (IAttendanceRepository _attendanceRepository
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
                        .Select(dateGroup =>
                        {
                            var isSalaryByProduct = dateGroup.Count(a => a.IsSalaryByProduct) >= 1;
                            var isOverTime = dateGroup.Count(a => a.IsOverTime) >= 1;
                            var attendanceCount = dateGroup.Count(a => a.IsAttendance && a.SlotId != 3);

                            var isHalfWork = false;
                            var isOneWork = false;

                            if (attendanceCount == 2)
                            {
                                var salaryByProductCount = dateGroup.Count(a => a.IsSalaryByProduct && a.SlotId != 3);
                                if (salaryByProductCount == 1)
                                {
                                    isHalfWork = true;
                                }
                                else if (salaryByProductCount == 0)
                                {
                                    isOneWork = true;
                                }
                            }
                            else if (attendanceCount == 1 && !isSalaryByProduct)
                            {
                                isHalfWork = true;
                            }


                            return new AttendanceUserReportResponse(
                                Date: dateGroup.Key,
                                AttedanceDateReport: new AttedanceDateReport(
                                    IsHalfWork: isHalfWork,
                                    IsOneWork: isOneWork,
                                    IsSalaryByProduct: isSalaryByProduct,
                                    IsOverTime: isOverTime
                                )
                            );
                        })
                        .ToList()
                )).FirstOrDefault();

        return Result.Success<AttendanceUserResponse>.Get(attendanceResponse);

    }
}
