using Contract.Abstractions.Messages;
using Contract.Services.Attendance.ShareDtos;

namespace Contract.Services.Attendance.Queries;

public record GetAttendanceByUserIdAndDateQuery
    (GetAttendancesByDateRequest getRequest, string UserId)
    : IQueryHandler<AttendanceUserDetailResponse>;