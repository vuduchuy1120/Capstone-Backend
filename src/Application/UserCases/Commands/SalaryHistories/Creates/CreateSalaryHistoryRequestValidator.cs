using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.SalaryHistory.Creates;
using FluentValidation;

namespace Application.UserCases.Commands.SalaryHistories.Creates
{
    public class CreateSalaryHistoryRequestValidator : AbstractValidator<CreateSalaryHistoryRequest>
    {
        public CreateSalaryHistoryRequestValidator(
            IUserRepository userRepository
        )
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("Bạn cần nhập UserId.")
                .MustAsync(async (userId, cancellationToken) =>
                {
                    return await userRepository.IsUserActiveAsync(userId);
                }).WithMessage("User không tồn tại hoặc đã bị khóa.");

            RuleFor(x => x.SalaryByDayRequest)
                .NotNull()
                .WithMessage("Bạn cần nhập SalaryByDayRequest.");

            RuleFor(x => x.SalaryByDayRequest.Salary)
                .NotEmpty()
                .WithMessage("Bạn cần nhập Salary.")
                .GreaterThan(0)
                .WithMessage("Lương phải lớn hơn 0.");

            RuleFor(x => x.SalaryByDayRequest.StartDate)
                .NotEmpty()
                .WithMessage("Bạn cần nhập ngày bắt đầu tính lương.");

            RuleFor(x => x.SalaryOverTimeRequest)
                .NotNull()
                .WithMessage("Bạn cần nhập SalaryOverTimeRequest.");

            RuleFor(x => x.SalaryOverTimeRequest.Salary)
                .NotEmpty()
                .WithMessage("Bạn cần nhập Salary.")
                .GreaterThan(0)
                .WithMessage("Lương phải lớn hơn 0.");

            RuleFor(x => x.SalaryOverTimeRequest.StartDate)
                .NotEmpty()
                .WithMessage("Bạn cần nhập ngày bắt đầu tính lương.");

        }
    }
}
