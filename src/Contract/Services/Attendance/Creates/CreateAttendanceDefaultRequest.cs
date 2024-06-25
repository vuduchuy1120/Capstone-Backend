namespace Contract.Services.Attendance.Create;

public record CreateAttendanceDefaultRequest
(
    int SlotId,
    string Date,
    Guid CompanyId,
    List<CreateAttendanceWithoutSlotIdRequest> CreateAttendances
    );

