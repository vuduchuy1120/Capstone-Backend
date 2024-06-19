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
        RuleFor(req => req.CreateAttendances)
            .MustAsync(async (createAttendances, cancellationToken) =>
            {
                var userIds = createAttendances.Select(x => x.UserId).ToList();
                var check = await userRepository.IsAllUserActiveAsync(userIds);
                return check;
            }).WithMessage("One or more UserId is invalid");
        RuleFor(req => req.CreateAttendances)
            .MustAsync(async (request, CreateAttendance, _) =>
            {
                var userIds = CreateAttendance.Select(x => x.UserId).ToList();
                var currentDateTime = DateUtil.GetNow().ToString("dd/MM/yyyy");
                var formatedDate = DateUtil.ConvertStringToDateTimeOnly(currentDateTime);
                return !await attendanceRepository.IsAttendanceAlreadyExisted(userIds, request.slotId, formatedDate);
            }).WithMessage("One or more attendance is already existed in the system");
    }
}
