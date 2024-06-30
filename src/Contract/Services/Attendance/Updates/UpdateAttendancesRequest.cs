namespace Contract.Services.Attendance.Update;

public record UpdateAttendancesRequest
(
    int SlotId,
    string Date,
    List<UpdateAttendanceWithoutSlotIdRequest> UpdateAttendances
    );
