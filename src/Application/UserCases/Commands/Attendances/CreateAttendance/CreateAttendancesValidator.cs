using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Create;
using FluentValidation;

namespace Application.UserCases.Commands.Attendances.CreateAttendance;

public sealed class CreateAttendancesValidator : AbstractValidator<CreateAttendanceDefaultRequest>
{
    public CreateAttendancesValidator(IUserRepository userRepository, ISlotRepository slotRepository, IAttendanceRepository attendanceRepository)
    {
        RuleFor(req => req.slotId)
            .NotEmpty().WithMessage("SlotId is required")
            .MustAsync(async (slotId, cancellationToken) =>
            {
                var slot = await slotRepository.IsSlotExisted(slotId);
                return slot;
            }).WithMessage("SlotId is invalid");
        RuleForEach(req => req.CreateAttendances)
            .NotEmpty().WithMessage("CreateAttendanceRequest is required")
            .MustAsync(async (createAttendanceRequest, cancellationToken) =>
            {
                var user = await userRepository.GetUserByIdAsync(createAttendanceRequest.UserId);
                return user != null;
            }).WithMessage("UserId is invalid")
             .Must(attendance =>
             {
                 return attendance.HourOverTime >= 0;
             }).WithMessage("HourOverTime must be greater than or equal to 0!")
            .Must(attendance =>
            {
                return attendance.HourOverTime <= 10;
            }).WithMessage("HourOverTime must be less than or equal to 10!")
            .Must(attendance =>
            {
                //if attendance = false then all ields must be false
                if (!attendance.IsAttendance)
                {
                    return attendance.IsOverTime == false && attendance.IsSalaryByProduct == false && attendance.HourOverTime == 0;
                }
                return true; // This condition is not applicable when IsAttendance is true
            }).WithMessage("IsAttendance must be true")
             .Must(attendance =>
             {
                 if (attendance.IsOverTime)
                 {
                     return attendance.HourOverTime > 0;
                 }
                 return true; // This condition is not applicable when IsOverTime is false
             }).WithMessage("HourOverTime must be greater than 0 when IsOverTime is true!")
            .Must(attendance =>
            {
                if (!attendance.IsOverTime)
                {
                    return attendance.HourOverTime == 0;
                }
                return true; // This condition is not applicable when IsOverTime is true
            }).WithMessage("HourOverTime must be 0 when IsOverTime is false!")
            .Must(attendance =>
            {
                return attendance.IsSalaryByProduct == false || attendance.IsSalaryByProduct == true;
            }).WithMessage("IsSalaryByProduct must be true or false!");
    }
}
