namespace Contract.Services.Attendance.Queries;

public record GetAttendanceRequest
(   string? SearchTerm,
    string Date,
    Guid CompanyId,
    int SlotId,
    int PageIndex = 1,
    int PageSize = 10
    );
