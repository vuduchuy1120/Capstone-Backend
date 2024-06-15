using Application.Abstractions.Data;
using Application.Utils;
using Domain.Abstractions.Exceptions;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Update;
using Domain.Exceptions.Attendances;
using FluentValidation;

namespace Application.UserCases.Commands.Attendances.UpdateAttendance;

internal sealed class UpdateAttendancesCommandHandler(
    IAttendanceRepository _attendanceRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateAttendancesRequest> _validator)
    : ICommandHandler<UpdateAttendancesCommand>
{
    public async Task<Result.Success> Handle(UpdateAttendancesCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.UpdateAttendanceRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var attendances = request.UpdateAttendanceRequest.UpdateAttendances;

        foreach (var attendance in attendances)
        {
            var formattedDate = DateUtil.ConvertStringToDateTimeOnly(attendance.Date);
            var attendanceEntity = await _attendanceRepository
                .GetAttendanceByUserIdSlotIdAndDateAsync(
                 attendance.UserId,
                 request.UpdateAttendanceRequest.SlotId,
                 formattedDate)
                ?? throw new AttendanceNotFoundException();
            // check is can update
            var isCanUpdateAttendance = await _attendanceRepository.IsCanUpdateAttendance(attendance.UserId, request.UpdateAttendanceRequest.SlotId, formattedDate);
            if (!isCanUpdateAttendance)
            {
                throw new MyValidationException("Can not update because over 2 days!");
            }
            attendanceEntity.Update(attendance, request.UpdatedBy);

            _attendanceRepository.UpdateAttendance(attendanceEntity);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Update();
    }
}
