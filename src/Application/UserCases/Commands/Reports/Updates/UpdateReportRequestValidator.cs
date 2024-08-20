using Application.Abstractions.Data;
using Contract.Services.Report.ShareDtos;
using Contract.Services.Report.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.Reports.Updates;

public class UpdateReportRequestValidator : AbstractValidator<UpdateReportRequest>
{
    public UpdateReportRequestValidator(IReportRepository _reportRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Yêu cầu nhập id.")
            .MustAsync(async (Id, _) =>
            {
                return await _reportRepository.IsReportExisted(Id);
            }).WithMessage("Id không tồn tại hoặc không hợp lệ.");

        RuleFor(x => x.ReplyMessage)
            .NotEmpty().WithMessage("Bạn cần nhập nội dung phản hồi.")
            .MaximumLength(500).WithMessage("Nội dung không được vượt quá 500 kí tự.");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Trạng thái báo cáo cần phải được sửa đổi.")
            .Must(status =>
            {
                return status == StatusReport.Rejected || status == StatusReport.Accepted;
            }).WithMessage("Chỉ có thể thay đổi trạng thái thành chấp nhận hoặc từ chối.");

    }
}
