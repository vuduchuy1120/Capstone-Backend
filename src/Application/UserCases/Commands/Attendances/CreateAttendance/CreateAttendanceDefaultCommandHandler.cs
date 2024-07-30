using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Create;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;

namespace Application.UserCases.Commands.Attendances.CreateAttendance;

internal sealed class CreateAttendanceDefaultCommandHandler(
    IAttendanceRepository _attendanceRepository,
    IUserRepository _userRepository,
    IValidator<CreateAttendanceDefaultRequest> _validator,
    IUnitOfWork _unitOfWork) : ICommandHandler<CreateAttendanceDefaultCommand>
{
    public async Task<Result.Success> Handle(CreateAttendanceDefaultCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.CreateAttendanceDefaultRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var formattedDate = DateUtil.ConvertStringToDateTimeOnly(request.CreateAttendanceDefaultRequest.Date);


        var isSalaryCalculated = await _attendanceRepository.IsSalaryCalculatedForMonth(formattedDate.Month, formattedDate.Year);
        if (isSalaryCalculated)
        {
            throw new MyValidationException($"Cannot create attendance records for {formattedDate.Month}/{formattedDate.Year} because salary has already been calculated.");
        }

        var userIds = request.CreateAttendanceDefaultRequest.CreateAttendances.Select(x => x.UserId).ToList();
        var roleName = request.RoleName;
        var companyId = request.CompanyId;
        if(roleName != "MAIN_ADMIN")
        {
            var check = await _userRepository.IsAllUserActiveByCompanyId(userIds, companyId);
            if (!check)
            {
                throw new UserNotPermissionException("You dont have permission create attendance of other user companyID");
            }
        }

       
        var emps = request.CreateAttendanceDefaultRequest.CreateAttendances;
        var attendances = emps
                        .Select(emp => Attendance
                        .Create(
                                    emp,
                                    request.CreateAttendanceDefaultRequest.Date,
                                    request.CreateAttendanceDefaultRequest.SlotId,
                                    request.CreatedBy))
                        .ToList();

        await _attendanceRepository.AddRangeAsync(attendances);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success.Create();
    }
}