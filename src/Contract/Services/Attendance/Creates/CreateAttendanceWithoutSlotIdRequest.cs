namespace Contract.Services.Attendance.Create;

public record CreateAttendanceWithoutSlotIdRequest
(
    string UserId,
    double HourOverTime,
    bool IsAttendance,
    bool IsSalaryByProduct,
    bool IsManufacture
    );