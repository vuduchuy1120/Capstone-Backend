using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.PaidSalary.Updates;
using Domain.Abstractions.Exceptions;
using FluentValidation;

namespace Application.UserCases.Commands.PaidSalaries.Updates;

internal sealed class UpdatePaidSalaryCommandHandler
    (IPaidSalaryRepository _paidSalaryRepository,
    IUserRepository _userRepository,
    IValidator<UpdatePaidSalaryRequest> _validator,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<UpdatePaidSalaryCommand>
{
    public async Task<Result.Success> Handle(UpdatePaidSalaryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.updateReq);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var paidSalary = await _paidSalaryRepository.GetPaidSalaryById(request.updateReq.Id);

        var userId = paidSalary.UserId;
        var user = await _userRepository.GetUserByIdAsync(userId);

        var accountBalanceCurrent = user?.AccountBalance ?? 0;

        var AccountBalanceUpdate = accountBalanceCurrent + paidSalary.Salary - request.updateReq.Salary;

        paidSalary.Update(request.updateReq, request.UpdatedBy);
        _paidSalaryRepository.UpdatePaidSalary(paidSalary);

        user.UpdateAccountBalance(AccountBalanceUpdate);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success.Update();
    }
}
