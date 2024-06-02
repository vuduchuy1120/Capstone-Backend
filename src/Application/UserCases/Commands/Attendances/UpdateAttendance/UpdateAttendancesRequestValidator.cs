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
        RuleFor(x => x.UpdateAttendances).Must(attendances =>
        {
            foreach (var attendance in attendances)
            {
                var date = attendance.Date;
                if (string.IsNullOrEmpty(date) || !System.Text.RegularExpressions.Regex.IsMatch(date, @"^\d{2}/\d{2}/\d{4}$") ||
                    !DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    return false;
                }
            }
            return true;
        }).WithMessage("Date must be a valid date in the format dd/MM/yyyy");
        // validate each attendance with user, date, hourOverTIme
        RuleForEach(x => x.UpdateAttendances)
            .NotEmpty().WithMessage("Attendance is required!")
            .MustAsync(async (attendance, _) =>
             {
                 var user = await userRepository.IsUserExistAsync(attendance.UserId);
                 return user;
             }).WithMessage("User not found!")
        .MustAsync(async (request, attendance, _) =>
        {
            var formattedDate = DateUtil.ConvertStringToDateTimeOnly(attendance.Date);
            var attendanceEntity = await attendanceRepository.GetAttendanceByUserIdSlotIdAndDateAsync(attendance.UserId, request.SlotId, formattedDate);
            return attendanceEntity != null;
        }).WithMessage("Attendance not found!")
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

        //RuleFor Date must be a valid date in the format dd/MM/yyyy in each attendance

    }
}


