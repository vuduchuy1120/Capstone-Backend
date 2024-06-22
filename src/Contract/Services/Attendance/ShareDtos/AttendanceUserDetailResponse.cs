using Contract.Services.EmployeeProduct.ShareDto;

namespace Contract.Services.Attendance.ShareDtos;

public record AttendanceUserDetailResponse
(
    int SlotId,
    string Date,
    bool IsAttendance,
    bool IsOvertime,
    double HourOverTime,
    bool IsSalaryByProduct,
    List<EmployeeProductResponse> EmployeeProductResponses
    );