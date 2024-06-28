using Application.Abstractions.Data;
using Contract.Services.Report.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.Reports.Updates;

public class UpdateReportRequestValidator : AbstractValidator<UpdateReportRequest>
{
    public UpdateReportRequestValidator(IReportRepository _reportRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(async (Id, _) =>
            {
                return await _reportRepository.IsReportExisted(Id);
            }).WithMessage("Id is invalid");

        RuleFor(x => x.ReplyMessage)
            .NotEmpty().WithMessage("ReplyMessage is required")
            .MaximumLength(500).WithMessage("ReplyMessage must not exceed 500 characters");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status =>
            {
                return status == "Approved" || status == "Rejected";
            }).WithMessage("Status must be either 'Approved' or 'Rejected'");
    }
}
