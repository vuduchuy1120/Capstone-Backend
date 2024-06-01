using Contract.Abstractions.Messages;

namespace Contract.Services.Attendance.Update;

public record UpdateAttendanceCommand(UpdateAttendanceRequest UpdateAttendanceRequest, string UpdatedBy) : ICommand;
