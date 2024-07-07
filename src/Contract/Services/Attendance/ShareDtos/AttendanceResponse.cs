
using Contract.Services.EmployeeProduct.ShareDto;

namespace Contract.Services.Attendance.ShareDto;

public record AttendanceResponse
(
    string UserId,
    string Avatar,
    DateOnly Date,
    string FullName,
    double HourOverTime,
    bool IsAttendance,
    bool IsOverTime,
    bool IsSalaryByProduct,
    bool IsManufacture,
    List<EmployeeProductResponse> EmployeeProductResponses
    );