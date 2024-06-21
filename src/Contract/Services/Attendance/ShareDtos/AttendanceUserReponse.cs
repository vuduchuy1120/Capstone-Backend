namespace Contract.Services.Attendance.ShareDtos;

public record AttendanceUserReponse
(
    int Month,
    int Year,
    string UserId,
    List<AttendanceUserReportResponse> Attendances
    );
