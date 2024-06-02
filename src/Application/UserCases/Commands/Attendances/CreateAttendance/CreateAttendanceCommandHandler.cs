using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Create;
using Domain.Entities;

namespace Application.UserCases.Commands.Attendances.CreateAttendance;

internal sealed class CreateAttendanceCommandHandler(
    IAttendanceRepository _attendanceRepository, 
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateAttendanceCommand>
{
    public async Task<Result.Success> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
    {
        var createAttendanceRequest = request.CreateAttendanceRequest;

        var attendance = Attendance.Create(createAttendanceRequest.CreateAttendance, createAttendanceRequest.SlotId, request.CreatedBy);

        _attendanceRepository.AddAttendance(attendance);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }
}
