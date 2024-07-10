using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Update;
using Domain.Entities;
using FluentValidation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.UserCases.Commands.Attendances.UpdateAttendance;

public sealed class UpdateAttendancesRequestValidator : AbstractValidator<UpdateAttendancesRequest>
{
    public UpdateAttendancesRequestValidator(IUserRepository userRepository, ISlotRepository slotRepository, IAttendanceRepository attendanceRepository)
    {
        RuleFor(x => x.SlotId)
        .NotEmpty().WithMessage("SlotID is required!")
        .MustAsync(async (SlotId, _) =>
        {
            var slot = await slotRepository.IsSlotExisted(SlotId);
            return slot;
        }).WithMessage("SlotID invalad or notfound!");


        RuleFor(x => x.UpdateAttendances).NotEmpty();
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

        RuleFor(x => x.UpdateAttendances)
            .MustAsync(async (updateAttendances, cancellationToken) =>
            {
                var userIds = updateAttendances.Select(x => x.UserId).ToList();
                return await userRepository.IsAllUserActiveAsync(userIds);
            }).WithMessage("One or more UserId is invalid or doesn't exist!");

        //IsAllAttendanceExist
        RuleFor(x => x.UpdateAttendances)
            .MustAsync(async (request, updateAttendances, _) =>
            {
                var formattedDate = DateUtil.ConvertStringToDateTimeOnly(request.Date);
                var userIds = updateAttendances.Select(x => x.UserId).ToList();

                return await attendanceRepository.IsAllAttendancesExist(request.SlotId, formattedDate, userIds);
            }).WithMessage("One or more Attendance is invalid or doesn't exist");

        // validate each attendance with user, date, hourOverTIme
        RuleForEach(x => x.UpdateAttendances)
            .NotEmpty().WithMessage("Attendance is required!")
            .Must(attendance =>
            {
                return attendance.HourOverTime >= 0;
            }).WithMessage("HourOverTime must be greater than or equal to 0!")
            .Must(attendance =>
            {
                return attendance.HourOverTime <= 5;
            }).WithMessage("HourOverTime must be less than or equal to 5!")
            .Must((request, attendance) =>
             {
                 return attendance.HourOverTime <= 3 && (request.SlotId == 1 || request.SlotId == 2);
             }).WithMessage("HourOverTime must be less than or equal to 3 for slot is morning and after!")
            .Must(attendance =>
            {
                return attendance.HourOverTime % 0.5 == 0;
            }).WithMessage("HourOverTime must be a multiple of 0.5!")
            .Must(attendance =>
            {
                if (!attendance.IsAttendance)
                {
                    return attendance.IsOverTime == false && attendance.HourOverTime == 0;
                }
                return true;
            }).WithMessage("IsAttendance must be true when hour over time has value!");

    }
}


