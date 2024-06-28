namespace Contract.Services.Attendance.Update;

public record UpdateAttendancesRequest
(
    int SlotId,
    string Date,
    Guid CompanyId,
    List<UpdateAttendanceWithoutSlotIdRequest> UpdateAttendances
    );
