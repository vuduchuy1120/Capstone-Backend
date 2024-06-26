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
    IUserRepository _userRepository,
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

        var formattedDate = DateUtil.ConvertStringToDateTimeOnly(request.UpdateAttendanceRequest.Date);
        var userIds = request.UpdateAttendanceRequest.UpdateAttendances.Select(x => x.UserId).ToList();
        var roleName = request.RoleNameClaim;
        var companyId = request.CompanyIdClaim;

        if(roleName != "MAIN_ADMIN")
        {
            var isCanUpdateAttendance = await _attendanceRepository.IsAllCanUpdateAttendance(userIds, request.UpdateAttendanceRequest.SlotId, formattedDate);
            if (!isCanUpdateAttendance)
            {
                throw new MyValidationException("Can not update because over 2 days!");
            }
            var isUpdate = await _userRepository.IsAllUserActiveByCompanyId(userIds, companyId);
            if (!isUpdate)
            {
                throw new MyValidationException("You dont have permission update attendance of other user companyID");
            }
        }
        var attendances = await _attendanceRepository.GetAttendancesByKeys(request.UpdateAttendanceRequest.SlotId, formattedDate, userIds);

        var updateRequests = request.UpdateAttendanceRequest.UpdateAttendances;

        foreach (var updateRequest in updateRequests)
        {
            var attendance = attendances.FirstOrDefault(x => x.UserId == updateRequest.UserId);
            if (attendance is null)
            {
                throw new AttendanceNotFoundException();
            }
            attendance.Update(updateRequest, request.UpdatedBy);
        }

        _attendanceRepository.UpdateRange(attendances);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Update();
    }
}
