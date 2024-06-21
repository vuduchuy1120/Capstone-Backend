namespace Contract.Services.Attendance.ShareDtos;

public record AttedanceDateReport
(
    int SlotId,
    bool IsPresent,
    bool isSalaryByProduct,
    bool isOverTime
    );

