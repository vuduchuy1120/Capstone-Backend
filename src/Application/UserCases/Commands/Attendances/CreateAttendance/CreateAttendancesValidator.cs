using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Create;
using FluentValidation;

namespace Application.UserCases.Commands.Attendances.CreateAttendance;

public sealed class CreateAttendancesValidator : AbstractValidator<CreateAttendanceDefaultRequest>
{
    public CreateAttendancesValidator(IUserRepository userRepository, ISlotRepository slotRepository, IAttendanceRepository attendanceRepository)
    {
        RuleFor(x => x.Date)
       .Must(Date =>
       {
           if (string.IsNullOrEmpty(Date) || !System.Text.RegularExpressions.Regex.IsMatch(Date, @"^\d{2}/\d{2}/\d{4}$") ||
               !DateTime.TryParseExact(Date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
           {
               return false;
           }

           return true;
       }).WithMessage("Date must be a valid date in the format dd/MM/yyyy");
        RuleFor(req => req.CreateAttendances)
            .NotEmpty().WithMessage("CreateAttendances is required")
            .Must(x => x.Count > 0).WithMessage("CreateAttendances is required");
        RuleFor(req => req.SlotId)
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
                var formatedDate = DateUtil.ConvertStringToDateTimeOnly(request.Date.ToString());
                return !await attendanceRepository.IsAttendanceAlreadyExisted(userIds, request.SlotId, formatedDate);
            }).WithMessage("One or more attendance is already existed in the system");
        RuleForEach(x => x.CreateAttendances)
            .NotEmpty().WithMessage("Attendance is required!")
            .Must(attendance =>
            {
                return attendance.HourOverTime >= 0;
            }).WithMessage("HourOverTime must be greater than or equal to 0!")
            .Must(attendance =>
            {
                return attendance.HourOverTime <= 6;
            }).WithMessage("HourOverTime must be less than or equal to 5!")
            .Must(attendance =>
            {
                return attendance.HourOverTime % 0.5 == 0;
            }).WithMessage("HourOverTime must be a multiple of 0.5!")
            .Must(attendance =>
            {
                if (!attendance.IsAttendance)
                {
                    return attendance.HourOverTime == 0;
                }
                return true;
            }).WithMessage("IsAttendance must be true when hourOverTime > 0");
    }
}
