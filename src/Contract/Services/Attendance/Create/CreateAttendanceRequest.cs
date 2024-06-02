namespace Contract.Services.Attendance.Create;
public record CreateAttendanceRequest
(   
    int SlotId,
    CreateAttendanceWithoutSlotIdRequest CreateAttendance
    );
