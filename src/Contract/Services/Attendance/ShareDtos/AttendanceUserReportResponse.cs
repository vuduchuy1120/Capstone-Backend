namespace Contract.Services.Attendance.ShareDtos;

public record AttendanceUserReportResponse
(
    string Date,
    List<AttedanceDateReport> AttedanceDateReports
    );