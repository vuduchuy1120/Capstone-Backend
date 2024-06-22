using Contract.Abstractions.Messages;
using Contract.Services.Attendance.ShareDtos;

namespace Contract.Services.Attendance.Queries;

public record GetAttendanceByUserIdAndDateQuery
    (string UserId, string Date)
    : IQueryHandler<AttendanceUserDetailResponse>;