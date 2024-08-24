using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Update;
using FluentValidation;

namespace Application.UserCases.Commands.Attendances.UpdateAttendance;

public sealed class UpdateAttendancesRequestValidator : AbstractValidator<UpdateAttendancesRequest>
{
    public UpdateAttendancesRequestValidator(IUserRepository userRepository, ISlotRepository slotRepository, IAttendanceRepository attendanceRepository)
    {
        RuleFor(x => x.SlotId)
        .NotEmpty().WithMessage("SlotID không được để trống!")
        .MustAsync(async (SlotId, _) =>
        {
            var slot = await slotRepository.IsSlotExisted(SlotId);
            return slot;
        }).WithMessage("SlotID không hợp lệ hoặc không tồn tại!");

        RuleFor(x => x.UpdateAttendances).NotEmpty().WithMessage("Danh sách chấm công là bắt buộc!");

        RuleFor(x => x.Date)
        .Must(Date =>
        {
            if (string.IsNullOrEmpty(Date) || !System.Text.RegularExpressions.Regex.IsMatch(Date, @"^\d{2}/\d{2}/\d{4}$") ||
                !DateTime.TryParseExact(Date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
            {
                return false;
            }
            return true;
        }).WithMessage("Ngày phải là ngày hợp lệ ở định dạng dd/MM/yyyy");

        //RuleFor(x => x.UpdateAttendances)
        //    .MustAsync(async (updateAttendances, cancellationToken) =>
        //    {
        //        var userIds = updateAttendances.Select(x => x.UserId).ToList();
        //        return await userRepository.IsAllUserActiveAsync(userIds);
        //    }).WithMessage("Một hoặc nhiều UserId không hợp lệ hoặc không tồn tại!");

        RuleFor(x => x.UpdateAttendances)
            .MustAsync(async (request, updateAttendances, _) =>
            {
                var formattedDate = DateUtil.ConvertStringToDateTimeOnly(request.Date);
                var userIds = updateAttendances.Select(x => x.UserId).ToList();
                return await attendanceRepository.IsAllAttendancesExist(request.SlotId, formattedDate, userIds);
            }).WithMessage("Một hoặc nhiều lượt tham dự không hợp lệ hoặc không tồn tại!");

        RuleForEach(x => x.UpdateAttendances)
            .NotEmpty().WithMessage("Điểm danh không được để trống!")
            .Must(attendance =>
            {
                return attendance.HourOverTime >= 0;
            }).WithMessage("Giờ làm thêm phải lớn hơn hoặc bằng 0!")
            .Must(attendance =>
            {
                return attendance.HourOverTime <= 5;
            }).WithMessage("Giờ làm thêm phải nhỏ hơn hoặc bằng 5!")
            .Must((request, attendance) =>
            {
                if (request.SlotId == 1 || request.SlotId == 2)
                {
                    return attendance.HourOverTime <= 3;
                }
                return true;
            }).WithMessage("Giờ làm thêm phải nhỏ hơn hoặc bằng 3 cho ca sáng và chiều!")
            .Must(attendance =>
            {
                return attendance.HourOverTime % 0.5 == 0;
            }).WithMessage("Giờ làm thêm phải là bội số của 0.5!")
            .Must(attendance =>
            {
                if (!attendance.IsAttendance)
                {
                    return attendance.IsOverTime == false && attendance.HourOverTime == 0;
                }
                return true;
            }).WithMessage("Nếu không điểm danh thì không được tăng ca");

    }
}
