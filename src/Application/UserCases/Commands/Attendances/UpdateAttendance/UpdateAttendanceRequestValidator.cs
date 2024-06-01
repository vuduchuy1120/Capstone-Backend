using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Update;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.Attendances.UpdateAttendance;

public sealed class UpdateAttendanceRequestValidator : AbstractValidator<UpdateAttendanceRequest>
{
    public UpdateAttendanceRequestValidator(IUserRepository userRepository, ISlotRepository slotRepository, IAttendanceRepository attendanceRepository)
    {
        RuleFor(x => x.SlotId)
        .NotEmpty().WithMessage("SlotID is required!")
        .MustAsync(async (SlotId, _) =>
        {
            var slot = await slotRepository.IsSlotExisted(SlotId);
            return slot;
        }).WithMessage("SlotID invalad or notfound!");

        RuleFor(x => x.UpdateAttendances).NotEmpty();
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
            }).WithMessage("IsSalaryByProduct must be true or false!")
            .Must(attendance =>
            {
                var date = attendance.Date;
                return DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _);
            }).WithMessage("Date must be a valid date in the format dd/MM/yyyy");

    }

}


