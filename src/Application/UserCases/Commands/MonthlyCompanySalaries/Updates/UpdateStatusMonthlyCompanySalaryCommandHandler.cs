using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MonthlyCompanySalary.Updates;
using Domain.Abstractions.Exceptions;
using FluentValidation;

namespace Application.UserCases.Commands.MonthlyCompanySalaries.Updates;

public class UpdateStatusMonthlyCompanySalaryCommandHandler
    (IMonthlyCompanySalaryRepository _monthlyCompanySalaryRepository,
    IValidator<UpdateMonthlyCompanySalaryRequest> _validator,
    IUnitOfWork _unitOfWork
    )
     : ICommandHandler<UpdateStatusMonthlyCompanySalaryCommand>
{
    public async Task<Result.Success> Handle(UpdateStatusMonthlyCompanySalaryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request.updateReq, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var monthlyCompanySalary = await _monthlyCompanySalaryRepository.GetByIdAsync(request.updateReq.Id);
        monthlyCompanySalary.UpdateSatus(request.updateReq);

        _monthlyCompanySalaryRepository.Update(monthlyCompanySalary);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
}
