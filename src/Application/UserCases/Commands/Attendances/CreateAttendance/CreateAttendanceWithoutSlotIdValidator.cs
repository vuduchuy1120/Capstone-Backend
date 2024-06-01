using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Create;
using FluentValidation;

namespace Application.UserCases.Commands.Attendances.CreateAttendance;

public sealed class CreateAttendanceWithoutSlotIdValidator : AbstractValidator<CreateAttendanceDefaultRequest>
{
    public CreateAttendanceWithoutSlotIdValidator(IUserRepository userRepository, ISlotRepository slotRepository, IAttendanceRepository attendanceRepository)
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
             .Must(createAttendanceRequest =>
             {
                 return createAttendanceRequest.HourOverTime >= 0;
             }).WithMessage("HourOverTime must be greater than or equal to 0")
            .Must(createAttendanceRequest =>
            {
                return createAttendanceRequest.HourOverTime <= 24;
            }).WithMessage("HourOverTime must be less than or equal to 24")
            .Must(createAttendanceRequest =>
            {
                return createAttendanceRequest.IsAttendance == true || createAttendanceRequest.IsAttendance == false;
            }).WithMessage("IsAttendance must be true or false")
            .Must(createAttendanceRequest =>
            {
                return createAttendanceRequest.IsOverTime == true || createAttendanceRequest.IsOverTime == false;
            }).WithMessage("IsOverTime must be true or false")
            .Must(createAttendanceRequest =>
            {
                return createAttendanceRequest.IsSalaryByProduct == true || createAttendanceRequest.IsSalaryByProduct == false;
            }).WithMessage("IsSalaryByProduct must be true or false");
    }
}
