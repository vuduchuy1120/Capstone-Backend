using Application.Abstractions.Data;
using Contract.Services.PaidSalary.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.PaidSalaries.Updates;

public class UpdatePaidSalaryRequestValidator : AbstractValidator<UpdatePaidSalaryRequest>
{
    public UpdatePaidSalaryRequestValidator(IPaidSalaryRepository paidSalaryRepository)
    {
        RuleFor(x => x.Id)
            .MustAsync(async (id, cancellationToken) =>
            {
                return await paidSalaryRepository.IsPaidSalaryExistsAsync(id);
            }).WithMessage("Lương đã thanh toán không tồn tại.");
        RuleFor(x => x.Salary)
            .GreaterThan(0).WithMessage("Lương phải lớn hơn 0.");
    }
}
