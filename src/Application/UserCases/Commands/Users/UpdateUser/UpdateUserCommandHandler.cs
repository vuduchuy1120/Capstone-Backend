using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.SalaryHistory.ShareDtos;
using Contract.Services.User.UpdateUser;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;

namespace Application.UserCases.Commands.Users.UpdateUser;

internal sealed class UpdateUserCommandHandler(
    IUserRepository _userRepository,
    ISalaryHistoryRepository _salaryHistoryRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateUserRequest> _validator) : ICommandHandler<UpdateUserCommand>
{
    public async Task<Result.Success> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var updateRequest = request.UpdateUserRequest;

        var validationResult = await _validator.ValidateAsync(updateRequest);
        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var user = await _userRepository.GetUserByIdAsync(updateRequest.Id)
            ?? throw new UserNotFoundException(updateRequest.Id);

        user.Update(updateRequest, request.UpdatedBy);

        var salaryByDay = updateRequest.SalaryByDayRequest;
        var salaryOverTime = updateRequest.SalaryOverTimeRequest;

        var salaryByDayExist = await _salaryHistoryRepository
            .GetSalaryHistoryByUserIdDateAndSalaryType(
                user.Id,
                DateUtil.ConvertStringToDateTimeOnly(salaryByDay.StartDate),
                SalaryType.SALARY_BY_DAY);

        var salaryOverTimeExist = await _salaryHistoryRepository
            .GetSalaryHistoryByUserIdDateAndSalaryType(
                user.Id,
                DateUtil.ConvertStringToDateTimeOnly(salaryOverTime.StartDate),
                SalaryType.SALARY_OVER_TIME);

        var salaryHistoriesUpdate = new List<SalaryHistory>();
        var salaryHistoriesAdd = new List<SalaryHistory>();

        if (salaryByDayExist != null)
        {
            salaryByDayExist.Update(salaryByDay.Salary);
            salaryHistoriesUpdate.Add(salaryByDayExist);
        }
        else
        {
            var salary = SalaryHistory.Create(
                            user.Id,
                            salaryByDay.Salary,
                            DateUtil.ConvertStringToDateTimeOnly(salaryByDay.StartDate),
                            SalaryType.SALARY_BY_DAY);
            salaryHistoriesAdd.Add(salary);
        }

        if (salaryOverTimeExist != null)
        {
            salaryOverTimeExist.Update(salaryOverTime.Salary);
            salaryHistoriesUpdate.Add(salaryOverTimeExist);
        }
        else
        {
            var salary = SalaryHistory.Create(
                             user.Id,
                             salaryOverTime.Salary,
                             DateUtil.ConvertStringToDateTimeOnly(salaryOverTime.StartDate),
                             SalaryType.SALARY_OVER_TIME);
            salaryHistoriesAdd.Add(salary);
        }


        _userRepository.Update(user);
        if (salaryHistoriesUpdate.Any())
        {
            _salaryHistoryRepository.UpdateRangeSalaryHistory(salaryHistoriesUpdate);
        }
        if (salaryHistoriesAdd.Any())
        {
            _salaryHistoryRepository.AddRangeSalaryHistory(salaryHistoriesAdd);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Update();
    }
}
