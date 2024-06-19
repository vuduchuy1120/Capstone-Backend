
namespace Contract.Services.Attendance.ShareDto;

public record AttendanceResponse
(
    string UserId,
    //string avatar,
    DateOnly Date,
    string FullName,
    double HourOverTime,
    bool IsAttendance,
    bool IsOverTime,
    bool IsSalaryByProduct,
    bool IsManufacture
    );