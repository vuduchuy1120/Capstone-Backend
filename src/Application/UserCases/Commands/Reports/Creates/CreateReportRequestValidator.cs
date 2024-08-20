using Application.Abstractions.Data;
using Contract.Services.Report.Creates;
using FluentValidation;

namespace Application.UserCases.Commands.Reports.Creates;

public class CreateReportRequestValidator : AbstractValidator<CreateReportRequest>
{
    public CreateReportRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Bạn phải nhập nội dung báo cáo.")
            .MaximumLength(700).WithMessage("Nội dung báo cáo không được vượt quá 700 kí tự");
        RuleFor(x => x.ReportType)
            .IsInEnum().WithMessage("Loại báo cáo chỉ có thể là 0,1,2,3.");
    }
}
