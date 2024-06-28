namespace Contract.Services.Attendance.ShareDtos;

public record AttendanceUserResponse
(
    int Month,
    int Year,
    string UserId,
    List<AttendanceUserReportResponse> Attendances
    );
