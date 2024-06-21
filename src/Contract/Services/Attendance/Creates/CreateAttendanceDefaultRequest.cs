namespace Contract.Services.Attendance.Create;

public record CreateAttendanceDefaultRequest
(
    int SlotId,
    string Date,
    List<CreateAttendanceWithoutSlotIdRequest> CreateAttendances
    );

