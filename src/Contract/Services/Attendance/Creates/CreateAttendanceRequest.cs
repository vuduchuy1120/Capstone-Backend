namespace Contract.Services.Attendance.Create;
public record CreateAttendanceRequest
(
    int SlotId,
    string Date,
    CreateAttendanceWithoutSlotIdRequest CreateAttendance
    );
