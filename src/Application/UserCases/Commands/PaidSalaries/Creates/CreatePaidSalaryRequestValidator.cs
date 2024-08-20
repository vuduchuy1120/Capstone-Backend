using Application.Abstractions.Data;
using Contract.Services.PaidSalary.Creates;
using FluentValidation;

namespace Application.UserCases.Commands.PaidSalaries.Creates;

public class CreatePaidSalaryRequestValidator : AbstractValidator<CreatePaidSalaryRequest>
{
    public CreatePaidSalaryRequestValidator(IUserRepository _userRepository)
    {
        RuleFor(x => x.UserId)
            .MustAsync(async (userId, cancellationToken) =>
            {
                return await _userRepository.IsUserActiveAsync(userId);
            }).WithMessage("Người dùng không tồn tại.");
        // salary greater than 0
        RuleFor(x => x.Salary)
            .GreaterThan(0).WithMessage("Lương phải lớn hơn 0.");
    }
}
