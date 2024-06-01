using Contract.Abstractions.Messages;

namespace Contract.Services.Attendance.Create;

public record CreateAttendanceDefaultCommand(CreateAttendanceDefaultRequest CreateAttendanceDefaultRequest, string CreatedBy) : ICommand;

