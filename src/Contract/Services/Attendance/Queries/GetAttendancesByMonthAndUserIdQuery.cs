using Contract.Abstractions.Messages;
using Contract.Services.Attendance.ShareDtos;

namespace Contract.Services.Attendance.Queries;

public record GetAttendancesByMonthAndUserIdQuery(int Month, int Year, string UserId) : IQuery<AttendanceUserResponse>;
