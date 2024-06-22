namespace Contract.Services.Attendance.ShareDtos;

public record AttendanceRecordResponse
(
    List<AttendanceSlotReport> AttendanceSlotReports
    );
