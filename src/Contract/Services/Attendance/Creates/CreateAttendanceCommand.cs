using Contract.Abstractions.Messages;

namespace Contract.Services.Attendance.Create;
public record CreateAttendanceCommand(CreateAttendanceRequest CreateAttendanceRequest, string CreatedBy) : ICommand;

