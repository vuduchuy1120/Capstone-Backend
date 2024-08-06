using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Create;
using FluentValidation;

namespace Application.UserCases.Commands.Attendances.CreateAttendance;

public sealed class CreateAttendancesValidator : AbstractValidator<CreateAttendanceDefaultRequest>
{
    public CreateAttendancesValidator(
        IUserRepository userRepository, 
        ISlotRepository slotRepository, 
        IAttendanceRepository attendanceRepository)
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
        }).WithMessage("Ngày phải là ngày hợp lệ ở định dạng dd/MM/yyyy")
         .Must(Date =>
         {
             var formatedDate = DateUtil.ConvertStringToDateTimeOnly(Date);
             var dateNow = DateOnly.FromDateTime(DateTime.Now);
             return formatedDate <= dateNow;
         }).WithMessage("Ngày phải nhỏ hơn hoặc bằng hôm nay");

        RuleFor(req => req.CreateAttendances)
            .NotEmpty().WithMessage("Danh sách tham dự không được để trống")
            .Must(x => x.Count > 0).WithMessage("Danh sách tham dự không được để trống");

        RuleFor(req => req.SlotId)
            .NotEmpty().WithMessage("SlotId không được để trống")
            .MustAsync(async (slotId, cancellationToken) =>
            {
                var slot = await slotRepository.IsSlotExisted(slotId);
                return slot;
            }).WithMessage("SlotId không hợp lệ");

        RuleFor(req => req.CreateAttendances)
            .MustAsync(async (createAttendances, cancellationToken) =>
            {
                var userIds = createAttendances.Select(x => x.UserId).ToList();
                var check = await userRepository.IsAllUserActiveAsync(userIds);
                return check;
            }).WithMessage("Một hoặc nhiều UserId không hợp lệ");

        RuleFor(req => req.CreateAttendances)
            .MustAsync(async (request, CreateAttendance, _) =>
            {
                var userIds = CreateAttendance.Select(x => x.UserId).ToList();
                var formatedDate = DateUtil.ConvertStringToDateTimeOnly(request.Date.ToString());
                return !await attendanceRepository.IsAttendanceAlreadyExisted(userIds, request.SlotId, formatedDate);
            }).WithMessage("Một hoặc nhiều lượt tham dự đã tồn tại trong hệ thống");

        RuleForEach(x => x.CreateAttendances)
            .NotEmpty().WithMessage("Tham dự không được để trống!")
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
                    return attendance.HourOverTime == 0;
                }
                return true;
            }).WithMessage("Phải tham dự khi giờ làm thêm > 0");

    }
}
