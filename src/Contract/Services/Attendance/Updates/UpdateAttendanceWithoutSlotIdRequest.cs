namespace Contract.Services.Attendance.Update;

public record UpdateAttendanceWithoutSlotIdRequest
(
    string UserId,
    double HourOverTime,
    bool IsAttendance,
    bool IsOverTime,
    bool IsSalaryByProduct,
    bool IsManufacture
    );