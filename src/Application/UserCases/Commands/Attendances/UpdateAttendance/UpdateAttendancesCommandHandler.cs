using Application.Abstractions.Data;
using Application.Utils;
using Domain.Abstractions.Exceptions;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Update;
using Domain.Exceptions.Attendances;
using FluentValidation;
using Domain.Exceptions.Users;
using Contract.Services.Attendance.Create;

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

        await CheckPermissionAsync(request, formattedDate, DateOnly.FromDateTime(DateTime.Now));
        await CheckSalaryCalculatedAsync(formattedDate);
        
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


    private bool IsOverTwoDays(DateOnly DateRequest, DateOnly DateNow)
    {
        var daysDifference = DateNow.ToDateTime(TimeOnly.MinValue) - DateRequest.ToDateTime(TimeOnly.MinValue);

        return daysDifference.TotalDays > 2;
    }
    private async Task CheckPermissionAsync(UpdateAttendancesCommand request, DateOnly formattedDate, DateOnly dateNow)
    {
        var userIds = request.UpdateAttendanceRequest.UpdateAttendances.Select(x => x.UserId).ToList();
        var roleName = request.RoleNameClaim;
        var companyId = request.CompanyIdClaim;
        if (roleName != "MAIN_ADMIN")
        {
            var check = await _userRepository.IsAllUserActiveByCompanyId(userIds, companyId);
            if (!check)
            {
                throw new UserNotPermissionException("You don't have permission to create attendance for users of this company.");
            }

            if (IsOverTwoDays(formattedDate, dateNow))
            {
                throw new UserNotPermissionException("You do not have permission to update this record as it is over 2 days old.");
            }
        }
    }
    private async Task CheckSalaryCalculatedAsync(DateOnly formattedDate)
    {
        var isSalaryCalculated = await _attendanceRepository.IsSalaryCalculatedForMonth(formattedDate.Month, formattedDate.Year);
        if (isSalaryCalculated)
        {
            throw new MyValidationException($"Cannot create attendance records for {formattedDate.Month}/{formattedDate.Year} because salary has already been calculated.");
        }
    }

}
