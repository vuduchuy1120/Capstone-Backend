using Contract.Services.EmployeeProduct.ShareDto;

namespace Contract.Services.Attendance.ShareDtos;

public record AttendanceSlotReport
(
    int SlotId,
    bool IsAttendance,
    bool IsOvertime,
    double HourOverTime,
    bool IsSalaryByProduct,
    List<EmployeeProductResponse> EmployeeProductResponses
    );