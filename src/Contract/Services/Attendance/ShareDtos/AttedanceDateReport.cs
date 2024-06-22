namespace Contract.Services.Attendance.ShareDtos;

public record AttedanceDateReport
(
    bool IsPresentSlot1,
    bool IsPresentSlot2,
    bool IsPresentSlot3,
    bool IsSalaryByProduct,
    bool IsOverTime
    );

