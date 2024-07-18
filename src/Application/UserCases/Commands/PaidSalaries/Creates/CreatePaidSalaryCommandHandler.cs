using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.PaidSalary.Creates;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.PaidSalaries.Creates;

public sealed class CreatePaidSalaryCommandHandler
    (IPaidSalaryRepository _paidSalaryRepository,
    IUserRepository _userRepository,
    IValidator<CreatePaidSalaryRequest> _validator,
    IUnitOfWork _unitOfWork
    ) : ICommandHandler<CreatePaidSalaryCommand>
{
    public async Task<Result.Success> Handle(CreatePaidSalaryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.createReq);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var paidSalary = PaidSalary.Create(request.createReq, request.CreatedBy);
        _paidSalaryRepository.AddPaidSalary(paidSalary);

        var user = await _userRepository.GetUserByIdAsync(request.createReq.UserId);
        var AccountBalanceUpdate = user?.AccountBalance ?? 0 - request.createReq.Salary;

        user.UpdateAccountBalance(AccountBalanceUpdate);
        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Create();
    }
}
