using Application.Abstractions.Data;
using Contract.Services.Report.Creates;
using FluentValidation;

namespace Application.UserCases.Commands.Reports.Creates;

public class CreateReportRequestValidator : AbstractValidator<CreateReportRequest>
{
    public CreateReportRequestValidator(IReportRepository _reportRepository, IUserRepository _userRepository)
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(700).WithMessage("Description must not exceed 700 characters");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .WithMessage("Status is invalid");
        RuleFor(x => x.ReportType)
            .NotEmpty().WithMessage("ReportType is required")
            .WithMessage("ReportType is invalid");
    }
}
