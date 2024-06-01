
using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Update;
using Domain.Exceptions.Attendances;
using Domain.Exceptions.Users;

namespace Application.UserCases.Commands.Attendances.UpdateAttendance;

internal sealed class UpdateAttendanceCommandHandler(
    IAttendanceRepository _attendanceRepository,
    IUnitOfWork _unitOfWork)
    : ICommandHandler<UpdateAttendanceCommand>
{
    public async Task<Result.Success> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
    {
        var updateAttendanceRequest = request.UpdateAttendanceRequest;

        var formattedDate = DateUtil.ConvertStringToDateTimeOnly(updateAttendanceRequest.Date);
        var attendance = await _attendanceRepository
            .GetAttendanceByUserIdSlotIdAndDateAsync(
            updateAttendanceRequest.UserId,
            updateAttendanceRequest.SlotId,
            formattedDate)
            ?? throw new AttendanceNotFoundException();

        attendance.Update(updateAttendanceRequest, request.UpdatedBy);

        _attendanceRepository.UpdateAttendance(attendance);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
}
