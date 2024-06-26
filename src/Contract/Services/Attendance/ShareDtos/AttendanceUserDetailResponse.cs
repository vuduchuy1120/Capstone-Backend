
namespace Contract.Services.Attendance.ShareDtos;

public record AttendanceUserDetailResponse
(
    string Date,
    string UserId,
    List<AttendanceSlotReport> AttendanceSlotReports
    );