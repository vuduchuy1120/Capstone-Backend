namespace Contract.Services.Attendance.ShareDtos;

public record AttedanceDateReport
(
    bool IsHalfWork,
    bool IsOneWork,
    bool IsSalaryByProduct,
    bool IsOverTime
    );

