using Application.Abstractions.Data;
using Contract.Services.MonthlyCompanySalary.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.MonthlyCompanySalaries.Updates;

public class UpdateMonthlyCompanySalaryRequestValidator : AbstractValidator<UpdateMonthlyCompanySalaryRequest>
{
    public UpdateMonthlyCompanySalaryRequestValidator(IMonthlyCompanySalaryRepository _monthlyCompanySalaryRepository)
    {
        RuleFor(x => x.Id)
            .MustAsync(async (id, cancellation) => await _monthlyCompanySalaryRepository.IsExistAsync(id))
            .WithMessage("Monthly company salary is not exist.");
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Trạng thái chỉ có thể là Chưa thanh toán (0) hoặc Đã thanh toán (1).");
    }
}
