using Contract.Abstractions.Messages;

namespace Contract.Services.Attendance.Update;

public record UpdateAttendancesCommand(UpdateAttendancesRequest UpdateAttendanceRequest, string UpdatedBy) : ICommand;
