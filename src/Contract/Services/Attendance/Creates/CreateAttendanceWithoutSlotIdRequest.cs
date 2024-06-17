namespace Contract.Services.Attendance.Create;

public record CreateAttendanceWithoutSlotIdRequest
(
    string UserId,
    bool IsManufacture,
    bool IsSalaryByProduct
    );