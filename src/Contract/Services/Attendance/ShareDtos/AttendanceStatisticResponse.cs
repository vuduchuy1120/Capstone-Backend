namespace Contract.Services.Attendance.ShareDto;

public record AttendanceStatisticResponse
(
    int SlotId,
    int TotalAttendance,
    int TotalManufacture,
    int TotalSalaryByProduct,
    double TotalHourOverTime,
    int NumberOfPresent
    );
