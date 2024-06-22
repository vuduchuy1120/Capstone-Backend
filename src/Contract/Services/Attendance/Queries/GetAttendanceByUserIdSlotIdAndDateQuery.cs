using Contract.Abstractions.Messages;
using Contract.Services.Attendance.ShareDtos;

namespace Contract.Services.Attendance.Queries;

public record GetAttendanceByUserIdSlotIdAndDateQuery
    (string UserId, int SlotId, string Date)
    : IQueryHandler<AttendanceUserDetailResponse>;